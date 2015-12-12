/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using System;
using SharpEarth.util;
using SharpEarth.retrieve;
using SharpEarth.ogc.wcs.wcs100;
using SharpEarth.ogc.gml;
using SharpEarth.geom;
using SharpEarth.exception;
using SharpEarth.avlist;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using SharpEarth.java.net;

namespace SharpEarth.terrain{



/**
 * @author tag
 * @version $Id: WCSElevationModel.java 2154 2014-07-17 21:32:34Z pabercrombie $
 */
public class WCSElevationModel : BasicElevationModel
{
    public WCSElevationModel(XElement domElement, AVList parameters)
        :base(wcsGetParamsFromDocument(domElement, parameters))
    {

    }

    public WCSElevationModel(WCS100Capabilities caps, AVList parameters)
        :base(wcsGetParamsFromCapsDoc(caps, parameters))
    {

    }

    /**
     * Create a new elevation model from a serialized restorable state string.
     *
     * @param restorableStateInXml XML string in World Wind restorable state format.
     *
     * @see #getRestorableState()
     */
    public WCSElevationModel(String restorableStateInXml)
        :base(wcsRestorableStateToParams(restorableStateInXml))
    {

        RestorableSupport rs;
        try
        {
            rs = RestorableSupport.parse(restorableStateInXml);
        }
        catch (Exception e)
        {
            // Parsing the document specified by stateInXml failed.
            String message = Logging.getMessage("generic.ExceptionAttemptingToParseStateXml", restorableStateInXml);
            Logging.logger().severe(message);
            throw new ArgumentException(message, e);
        }

        this.doRestoreState(rs, null);
    }

    protected static AVList wcsGetParamsFromDocument(XElement domElement, AVList parameters)
    {
        if (domElement == null)
        {
            String message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
            parameters = new AVListImpl();

        DataConfigurationUtils.getWCSConfigParams(domElement, parameters);
        BasicElevationModel.getBasicElevationModelConfigParams(domElement, parameters);
        wcsSetFallbacks(parameters);

        parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(parameters.getStringValue(AVKey.WCS_VERSION), parameters));

        return parameters;
    }

    protected static AVList wcsGetParamsFromCapsDoc(WCS100Capabilities caps, AVList parameters)
    {
        if (caps == null)
        {
            String message = Logging.getMessage("nullValue.WCSCapabilities");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
        {
            String message = Logging.getMessage("nullValue.ElevationModelConfigParams");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        WCS100DescribeCoverage coverage = (WCS100DescribeCoverage) parameters.getValue(AVKey.DOCUMENT);
        if (coverage == null)
        {
            String message = Logging.getMessage("nullValue.WCSDescribeCoverage");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        getWCSElevationModelConfigParams(caps, coverage, parameters);

        wcsSetFallbacks(parameters);
        determineNumLevels(coverage, parameters);

        parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(caps.getVersion(), parameters));

        if (parameters.getValue(AVKey.ELEVATION_EXTREMES_FILE) == null)
        {
            // Use the default extremes file if there are at least as many levels in this new elevation model as the
            // level of the extremes file, which is level 5.
            int numLevels = (int) parameters.getValue(AVKey.NUM_LEVELS);
            if (numLevels >= 6)
                parameters.setValue(AVKey.ELEVATION_EXTREMES_FILE, "config/SRTM30Plus_ExtremeElevations_5.bil");
        }

        return parameters;
    }

    protected static void wcsSetFallbacks(AVList parameters)
    {
        if (parameters.getValue(AVKey.LEVEL_ZERO_TILE_DELTA) == null)
        {
            Angle delta = Angle.fromDegrees(20);
            parameters.setValue(AVKey.LEVEL_ZERO_TILE_DELTA, new LatLon(delta, delta));
        }

        if (parameters.getValue(AVKey.TILE_WIDTH) == null)
            parameters.setValue(AVKey.TILE_WIDTH, 150);

        if (parameters.getValue(AVKey.TILE_HEIGHT) == null)
            parameters.setValue(AVKey.TILE_HEIGHT, 150);

        if (parameters.getValue(AVKey.FORMAT_SUFFIX) == null)
            parameters.setValue(AVKey.FORMAT_SUFFIX, ".tif");

        if (parameters.getValue(AVKey.MISSING_DATA_SIGNAL) == null)
            parameters.setValue(AVKey.MISSING_DATA_SIGNAL, -9999d);

        if (parameters.getValue(AVKey.NUM_LEVELS) == null)
            parameters.setValue(AVKey.NUM_LEVELS, 18); // approximately 20 cm per pixel

        if (parameters.getValue(AVKey.NUM_EMPTY_LEVELS) == null)
            parameters.setValue(AVKey.NUM_EMPTY_LEVELS, 0);

        if (parameters.getValue(AVKey.ELEVATION_MIN) == null)
            parameters.setValue(AVKey.ELEVATION_MIN, -11000.0);

        if (parameters.getValue(AVKey.ELEVATION_MAX) == null)
            parameters.setValue(AVKey.ELEVATION_MAX, 8850.0);
    }

    protected static void determineNumLevels(WCS100DescribeCoverage coverage, AVList parameters)
    {
        List<GMLRectifiedGrid> grids =
            coverage.getCoverageOfferings().get(0).getDomainSet().getSpatialDomain().getRectifiedGrids();
        if (grids.Count < 1 || grids[0].getOffsetVectors().Count < 2)
        {
            parameters.setValue(AVKey.NUM_LEVELS, 18);
            return;
        }

        double xRes = Math.Abs(grids[0].getOffsetVectors().get(0).x);
        double yRes = Math.Abs(grids[0].getOffsetVectors().get(1).y);
        double dataResolution = Math.Min(xRes, yRes);

        int tileSize = (int) parameters.getValue(AVKey.TILE_WIDTH);
        LatLon level0Delta = (LatLon) parameters.getValue(AVKey.LEVEL_ZERO_TILE_DELTA);

        double n = Math.Log(level0Delta.getLatitude().degrees / (dataResolution * tileSize)) / Math.Log(2);
        parameters.setValue(AVKey.NUM_LEVELS, (int) (Math.Ceiling(n) + 1));
    }

    public static AVList getWCSElevationModelConfigParams(WCS100Capabilities caps, WCS100DescribeCoverage coverage,
        AVList parameters)
    {
        DataConfigurationUtils.getWCSConfigParameters(caps, coverage, parameters); // checks for null args

        // Ensure that we found all the necessary information.
        if (parameters.getStringValue(AVKey.DATASET_NAME) == null)
        {
            Logging.logger().warning(Logging.getMessage("WCS.NoCoverageName"));
            throw new WWRuntimeException(Logging.getMessage("WCS.NoCoverageName"));
        }

        if (parameters.getStringValue(AVKey.SERVICE) == null)
        {
            Logging.logger().warning(Logging.getMessage("WCS.NoGetCoverageURL"));
            throw new WWRuntimeException(Logging.getMessage("WCS.NoGetCoverageURL"));
        }

        if (parameters.getStringValue(AVKey.DATA_CACHE_NAME) == null)
        {
            Logging.logger().warning(Logging.getMessage("nullValue.DataCacheIsNull"));
            throw new WWRuntimeException(Logging.getMessage("nullValue.DataCacheIsNull"));
        }

        if (parameters.getStringValue(AVKey.IMAGE_FORMAT) == null)
        {
            Logging.logger().severe("WCS.NoImageFormats");
            throw new WWRuntimeException(Logging.getMessage("WCS.NoImageFormats"));
        }

        if (parameters.getValue(AVKey.SECTOR) == null)
        {
            Logging.logger().severe("WCS.NoLonLatEnvelope");
            throw new WWRuntimeException(Logging.getMessage("WCS.NoLonLatEnvelope"));
        }

        if (parameters.getStringValue(AVKey.COORDINATE_SYSTEM) == null)
        {
            String msg = Logging.getMessage("WCS.RequiredCRSNotSupported", "EPSG:4326");
            Logging.logger().severe(msg);
            throw new WWRuntimeException(msg);
        }

        return parameters;
    }

    protected class URLBuilder : TileUrlBuilder
    {
        protected readonly String layerNames;
        private readonly String imageFormat;
        protected readonly String serviceVersion;
        protected String URLTemplate = null;

        protected URLBuilder(String version, AVList parameters)
        {
            this.serviceVersion = version;
            this.layerNames = parameters.getStringValue(AVKey.COVERAGE_IDENTIFIERS);
            this.imageFormat = parameters.getStringValue(AVKey.IMAGE_FORMAT);
        }

        public Url getURL(Tile tile, String altImageFormat)
        {
            StringBuilder sb;
            if (this.URLTemplate == null)
            {
                sb = new StringBuilder( tile.getLevel().getService());

                if (!sb.ToString().ToLower().Contains("service=wcs"))
                    sb.Append("service=WCS");
                sb.Append("&request=GetCoverage");
                sb.Append("&version=");
                sb.Append(this.serviceVersion);
                sb.Append("&crs=EPSG:4326");
                sb.Append("&coverage=");
                sb.Append(this.layerNames);
                sb.Append("&format=");
                if (altImageFormat == null)
                    sb.Append(this.imageFormat);
                else
                    sb.Append(altImageFormat);

                this.URLTemplate = sb.ToString();
            }
            else
            {
                sb = new StringBuilder(this.URLTemplate);
            }

            sb.Append("&width=");
            sb.Append(tile.getWidth());
            sb.Append("&height=");
            sb.Append(tile.getHeight());

            Sector s = tile.getSector();
            sb.Append("&bbox=");
            sb.Append(s.getMinLongitude().getDegrees());
            sb.Append(",");
            sb.Append(s.getMinLatitude().getDegrees());
            sb.Append(",");
            sb.Append(s.getMaxLongitude().getDegrees());
            sb.Append(",");
            sb.Append(s.getMaxLatitude().getDegrees());

            sb.Append("&"); // terminate the query string

            return new java.net.URL(sb.ToString().Replace(" ", "%20"));
        }
    }

    /**
     * Appends WCS elevation model configuration elements to the superclass configuration document.
     *
     * @param parameters configuration parameters describing this WCS basic elevation model.
     *
     * @return a WCS basic elevation model configuration document.
     */
    protected XDocument createConfigurationDocument(AVList parameters)
    {
        XDocument doc = base.createConfigurationDocument(parameters);
        if (doc == null || doc.getDocumentElement() == null)
            return doc;

        DataConfigurationUtils.createWCSLayerConfigElements(parameters, doc.getDocumentElement());

        return doc;
    }

    public void composeElevations(Sector sector, List<T> latlons, int tileWidth, double[] buffer) where T : LatLon
    {
        if (sector == null)
        {
            String msg = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (latlons == null)
        {
            String msg = Logging.getMessage("nullValue.LatLonListIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (buffer == null)
        {
            String msg = Logging.getMessage("nullValue.ElevationsBufferIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (buffer.Length < latlons.Count || tileWidth > latlons.Count)
        {
            String msg = Logging.getMessage("ElevationModel.ElevationsBufferTooSmall", latlons.Count);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        WMSBasicElevationModel.ElevationCompositionTile tile = new WMSBasicElevationModel.ElevationCompositionTile(
            sector, this.getLevels().getLastLevel(),
            tileWidth, latlons.Count / tileWidth);

        this.downloadElevations(tile);
        tile.setElevations(this.readElevations(tile.getFile().toURI().toURL()), this);

        for (int i = 0; i < latlons.Count; i++)
        {
            LatLon ll = latlons[i];
            if (ll == null)
                continue;

            double value = this.lookupElevation(ll.getLatitude(), ll.getLongitude(), tile);

            // If an elevation at the given location is available, then write that elevation to the destination buffer.
            // Otherwise do nothing.
            if (value != this.getMissingDataSignal())
                buffer[i] = value;
        }
    }

    protected void downloadElevations(WMSBasicElevationModel.ElevationCompositionTile tile)
    {
        URL url = tile.getResourceURL();

        Retriever retriever = new HTTPRetriever(url,
            new WMSBasicElevationModel.CompositionRetrievalPostProcessor(tile.getFile()));
        retriever.setConnectTimeout(10000);
        retriever.setReadTimeout(60000);
        retriever.call();
    }

    //**************************************************************//
    //********************  Restorable Support  ********************//
    //**************************************************************//
    
    public override void getRestorableStateForAVPair(String key, Object value,
        RestorableSupport rs, RestorableSupport.StateObject context)
    {
        if (value is URLBuilder)
        {
            rs.addStateValueAsString(context, AVKey.WCS_VERSION, ((URLBuilder) value).serviceVersion);
        }
        else if (!(value is WCS100DescribeCoverage))
        {
            // Don't pass DescribeCoverage to superclass. The DescribeCoverage parameters will already be present in the
            // parameter list, so do nothing here.
            super.getRestorableStateForAVPair(key, value, rs, context);
        }
    }

    protected static AVList wcsRestorableStateToParams(String stateInXml)
    {
        if (stateInXml == null)
        {
            String message = Logging.getMessage("nullValue.StringIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        RestorableSupport rs;
        try
        {
            rs = RestorableSupport.parse(stateInXml);
        }
        catch (Exception e)
        {
            // Parsing the document specified by stateInXml failed.
            String message = Logging.getMessage("generic.ExceptionAttemptingToParseStateXml", stateInXml);
            Logging.logger().severe(message);
            throw new ArgumentException(message, e);
        }

        AVList parameters = new AVListImpl();
        wcsRestoreStateForParams(rs, null, parameters);
        return parameters;
    }

    protected static void wcsRestoreStateForParams(RestorableSupport rs, RestorableSupport.StateObject context,
        AVList parameters)
    {
        // Invoke the BasicElevationModel functionality.
        restoreStateForParams(rs, null, parameters);

        String s = rs.getStateValueAsString(context, AVKey.IMAGE_FORMAT);
        if (s != null)
            parameters.setValue(AVKey.IMAGE_FORMAT, s);

        s = rs.getStateValueAsString(context, AVKey.TITLE);
        if (s != null)
            parameters.setValue(AVKey.TITLE, s);

        s = rs.getStateValueAsString(context, AVKey.DISPLAY_NAME);
        if (s != null)
            parameters.setValue(AVKey.DISPLAY_NAME, s);

        RestorableSupport.adjustTitleAndDisplayName(parameters);

        s = rs.getStateValueAsString(context, AVKey.COVERAGE_IDENTIFIERS);
        if (s != null)
            parameters.setValue(AVKey.COVERAGE_IDENTIFIERS, s);

        s = rs.getStateValueAsString(context, AVKey.WCS_VERSION);
        if (s != null)
            parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(s, parameters));
    }
}
}
