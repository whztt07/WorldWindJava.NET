/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using System;
using SharpEarth.util;
using SharpEarth.retrieve;
using SharpEarth.ogc.wms;
using SharpEarth.geom;
using SharpEarth.exception;
using SharpEarth.avlist;
using System.Xml.Linq;
using System.Text;
using SharpEarth.java.io;
using SharpEarth.java.net;
using System.Collections.Generic;

namespace SharpEarth.terrain{



/**
 * @author tag
 * @version $Id: WMSBasicElevationModel.java 2050 2014-06-09 18:52:26Z tgaskins $
 */
public class WMSBasicElevationModel : BasicElevationModel
{
    private static readonly String[] formatOrderPreference = new String[]
        {
            "application/bil32", "application/bil16", "application/bil", "image/bil", "image/png", "image/tiff"
        };

    public WMSBasicElevationModel(AVList parameters)
        :base(parameters)
    {

    }

    public WMSBasicElevationModel(XElement domElement, AVList parameters)
        :this(wmsGetParamsFromDocument(domElement, parameters))
    {
    }

    public WMSBasicElevationModel(WMSCapabilities caps, AVList parameters)
        :this(wmsGetParamsFromCapsDoc(caps, parameters))
    {
    }

    public WMSBasicElevationModel(String restorableStateInXml)
        :base(wmsRestorableStateToParams(restorableStateInXml))
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

    protected static AVList wmsGetParamsFromDocument(XElement domElement, AVList parameters)
    {
        if (domElement == null)
        {
            String message = Logging.getMessage("nullValue.DocumentIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
            parameters = new AVListImpl();

        DataConfigurationUtils.getWMSLayerConfigParams(domElement, parameters);
        BasicElevationModel.getBasicElevationModelConfigParams(domElement, parameters);
        wmsSetFallbacks(parameters);

        parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(parameters.getStringValue(AVKey.WMS_VERSION), parameters));

        return parameters;
    }

    protected static AVList wmsGetParamsFromCapsDoc(WMSCapabilities caps, AVList parameters)
    {
        if (caps == null)
        {
            String message = Logging.getMessage("nullValue.WMSCapabilities");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
        {
            String message = Logging.getMessage("nullValue.ElevationModelConfigParams");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        String wmsVersion;
        try
        {
            wmsVersion = caps.getVersion();
            getWMSElevationModelConfigParams(caps, formatOrderPreference, parameters);
        }
        catch (ArgumentException e)
        {
            String message = Logging.getMessage("WMS.MissingLayerParameters");
            Logging.logger().severe(message, e);
            throw new ArgumentException(message, e);
        }
        catch (WWRuntimeException e)
        {
            String message = Logging.getMessage("WMS.MissingCapabilityValues");
            Logging.logger().severe(message, e);
            throw new ArgumentException(message, e);
        }

        wmsSetFallbacks(parameters);

        parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(wmsVersion, parameters));

        return parameters;
    }

    protected static void wmsSetFallbacks(AVList parameters)
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
            parameters.setValue(AVKey.FORMAT_SUFFIX, ".bil");

        if (parameters.getValue(AVKey.MISSING_DATA_SIGNAL) == null)
            parameters.setValue(AVKey.MISSING_DATA_SIGNAL, -9999d);

        if (parameters.getValue(AVKey.NUM_LEVELS) == null)
            parameters.setValue(AVKey.NUM_LEVELS, 18); // approximately 20 cm per pixel

        if (parameters.getValue(AVKey.NUM_EMPTY_LEVELS) == null)
            parameters.setValue(AVKey.NUM_EMPTY_LEVELS, 0);
    }

    // TODO: consolidate common code in WMSTiledImageLayer.URLBuilder and WMSBasicElevationModel.URLBuilder
    protected class URLBuilder : TileUrlBuilder
    {
        protected static readonly String MAX_VERSION = "1.3.0";

        private readonly String layerNames;
        private readonly String styleNames;
        private readonly String imageFormat;
        private readonly String wmsVersion;
        private readonly String crs;
        protected String URLTemplate = null;

        protected URLBuilder(String version, AVList parameters)
        {
            Double d = (Double) parameters.getValue(AVKey.MISSING_DATA_SIGNAL);

            this.layerNames = parameters.getStringValue(AVKey.LAYER_NAMES);
            this.styleNames = parameters.getStringValue(AVKey.STYLE_NAMES);
            this.imageFormat = parameters.getStringValue(AVKey.IMAGE_FORMAT);

            String coordSystemKey;
            String defaultCS;
            if (version == null || WWUtil.compareVersion(version, "1.3.0") >= 0) // version 1.3.0 or greater
            {
                this.wmsVersion = MAX_VERSION;
                coordSystemKey = "&crs=";
                defaultCS = "CRS:84"; // would like to do EPSG:4326 but that's incompatible with our old WMS server, see WWJ-474
            }
            else
            {
                this.wmsVersion = version;
                coordSystemKey = "&srs=";
                defaultCS = "EPSG:4326";
            }

            String coordinateSystem = parameters.getStringValue(AVKey.COORDINATE_SYSTEM);
            this.crs = coordSystemKey + (coordinateSystem != null ? coordinateSystem : defaultCS);
        }

        public URL getURL(Tile tile, String altImageFormat)
        {
            StringBuilder sb;
            if (this.URLTemplate == null)
            {
                sb = new StringBuilder( tile.getLevel().getService());

                if (!sb.ToString().ToLower().Contains("service=wms"))
                    sb.Append("service=WMS");
                sb.Append("&request=GetMap");
                sb.Append("&version=");
                sb.Append(this.wmsVersion);
                sb.Append(this.crs);
                sb.Append("&layers=");
                sb.Append(this.layerNames);
                sb.Append("&styles=");
                sb.Append(this.styleNames != null ? this.styleNames : "");
                sb.Append("&format=");
                if (altImageFormat == null)
                    sb.Append(this.imageFormat);
                else
                    sb.Append(altImageFormat);

                this.URLTemplate = sb.ToString();
            }
            else
            {
                sb = new StringBuilder( this.URLTemplate);
            }

            sb.Append("&width=");
            sb.Append(tile.getWidth());
            sb.Append("&height=");
            sb.Append(tile.getHeight());

            Sector s = tile.getSector();
            sb.Append("&bbox=");
            // The order of the coordinate specification matters, and it changed with WMS 1.3.0.
            if (WWUtil.compareVersion(this.wmsVersion, "1.1.1") <= 0 || this.crs.Contains("CRS:84"))
            {
                // 1.1.1 and earlier and CRS:84 use lon/lat order
                sb.Append(s.getMinLongitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMinLatitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMaxLongitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMaxLatitude().getDegrees());
            }
            else
            {
                // 1.3.0 uses lat/lon ordering
                sb.Append(s.getMinLatitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMinLongitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMaxLatitude().getDegrees());
                sb.Append(",");
                sb.Append(s.getMaxLongitude().getDegrees());
            }

            sb.Append("&"); // terminate the query string

            return new java.net.URL(sb.ToString().Replace(" ", "%20"));
        }
    }

    //**************************************************************//
    //********************  Configuration  *************************//
    //**************************************************************//

    /**
     * Parses WMSBasicElevationModel configuration parameters from a specified WMS Capabilities source. This writes
     * output as key-value pairs to parameters. Supported key and parameter names are: <table>
     * <th><td>Parameter</td><td>Value</td><td>Type</td></th> <tr><td>{@link AVKey#ELEVATION_MAX}</td><td>WMS layer's
     * maximum extreme elevation</td><td>Double</td></tr> <tr><td>{@link AVKey#ELEVATION_MIN}</td><td>WMS layer's
     * minimum extreme elevation</td><td>Double</td></tr> <tr><td>{@link AVKey#DATA_TYPE}</td><td>Translate WMS layer's
     * image format to a matching data type</td><td>String</td></tr> </table> This also parses common WMS layer
     * parameters by invoking {@link DataConfigurationUtils#getWMSLayerConfigParams(gov.nasa.worldwind.ogc.wms.WMSCapabilities,
     * String[], SharpEarth.avlist.AVList)}.
     *
     * @param caps                  the WMS Capabilities source to parse for WMSBasicElevationModel configuration
     *                              parameters.
     * @param formatOrderPreference an ordered array of preferred image formats, or null to use the default format.
     * @param parameters                the output key-value pairs which recieve the WMSBasicElevationModel configuration
     *                              parameters.
     *
     * @return a reference to parameters.
     *
     * @throws ArgumentException if either the document or parameters are null, or if parameters does not contain the
     *                                  required key-value pairs.
     * @throws SharpEarth.exception.WWRuntimeException
     *                                  if the Capabilities document does not contain any of the required information.
     */
    public static AVList getWMSElevationModelConfigParams(WMSCapabilities caps, String[] formatOrderPreference,
        AVList parameters)
    {
        if (caps == null)
        {
            String message = Logging.getMessage("nullValue.WMSCapabilities");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (parameters == null)
        {
            String message = Logging.getMessage("nullValue.ElevationModelConfigParams");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        // Get common WMS layer parameters.
        DataConfigurationUtils.getWMSLayerConfigParams(caps, formatOrderPreference, parameters);

        // Attempt to extract the WMS layer names from the specified parameters.
        String layerNames = parameters.getStringValue(AVKey.LAYER_NAMES);
        if (layerNames == null || layerNames.Length == 0)
        {
            String message = Logging.getMessage("nullValue.WMSLayerNames");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        String[] names = layerNames.Split(',');
        if (names == null || names.Length == 0)
        {
            String message = Logging.getMessage("nullValue.WMSLayerNames");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        // Get the layer's extreme elevations.
        Double[] extremes = caps.getLayerExtremeElevations(names);

        Double d = (Double) parameters.getValue(AVKey.ELEVATION_MIN);
        if (d == null && extremes != null && extremes[0] != null)
            parameters.setValue(AVKey.ELEVATION_MIN, extremes[0]);

        d = (Double) parameters.getValue(AVKey.ELEVATION_MAX);
        if (d == null && extremes != null && extremes[1] != null)
            parameters.setValue(AVKey.ELEVATION_MAX, extremes[1]);

        // Compute the internal pixel type from the image format.
        if (parameters.getValue(AVKey.DATA_TYPE) == null && parameters.getValue(AVKey.IMAGE_FORMAT) != null)
        {
            String s = WWIO.makeDataTypeForMimeType(parameters.getValue(AVKey.IMAGE_FORMAT).ToString());
            if (s != null)
                parameters.setValue(AVKey.DATA_TYPE, s);
        }

        // Use the default data type.
        if (parameters.getValue(AVKey.DATA_TYPE) == null)
            parameters.setValue(AVKey.DATA_TYPE, AVKey.INT16);

        // Use the default byte order.
        if (parameters.getValue(AVKey.BYTE_ORDER) == null)
            parameters.setValue(AVKey.BYTE_ORDER, AVKey.LITTLE_ENDIAN);

        return parameters;
    }

    /**
     * Appends WMS basic elevation model configuration elements to the superclass configuration document.
     *
     * @param parameters configuration parameters describing this WMS basic elevation model.
     *
     * @return a WMS basic elevation model configuration document.
     */
    protected XDocument createConfigurationDocument(AVList parameters)
    {
        XDocument doc = base.createConfigurationDocument(parameters);
        if (doc == null || doc.getDocumentElement() == null)
            return doc;

        DataConfigurationUtils.createWMSLayerConfigElements(parameters, doc.getDocumentElement());

        return doc;
    }

    //**************************************************************//
    //********************  Composition  ***************************//
    //**************************************************************//

    protected class ElevationCompositionTile : ElevationTile
    {
        private int width;
        private int height;
        private File file;

        public ElevationCompositionTile(Sector sector, Level level, int width, int height)
            : base(sector, level, -1, -1) // row and column aren't used and need to signal that
        {
            this.width = width;
            this.height = height;

            this.file = File.createTempFile(WWIO.DELETE_ON_EXIT_PREFIX, level.getFormatSuffix());
        }

        public override int getWidth()
        {
            return this.width;
        }

        public override int getHeight()
        {
            return this.height;
        }

        public override String getPath()
        {
            return this.file.getPath();
        }

        public File getFile()
        {
            return this.file;
        }
    }

    public void composeElevations(Sector sector, List<T> latlons, int tileWidth, double[] buffer)
      where T : LatLon
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

        if (buffer.length < latlons.size() || tileWidth > latlons.size())
        {
            String msg = Logging.getMessage("ElevationModel.ElevationsBufferTooSmall", latlons.size());
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        ElevationCompositionTile tile = new ElevationCompositionTile(sector, this.getLevels().getLastLevel(),
            tileWidth, latlons.size() / tileWidth);

        this.downloadElevations(tile);
        tile.setElevations(this.readElevations(tile.getFile().toURI().toURL()), this);

        for (int i = 0; i < latlons.size(); i++)
        {
            LatLon ll = latlons.get(i);
            if (ll == null)
                continue;

            double value = this.lookupElevation(ll.getLatitude(), ll.getLongitude(), tile);

            // If an elevation at the given location is available, then write that elevation to the destination buffer.
            // Otherwise do nothing.
            if (value != this.getMissingDataSignal())
                buffer[i] = value;
        }
    }

    protected void downloadElevations(ElevationCompositionTile tile)
    {
        URL url = tile.getResourceURL();

        Retriever retriever = new HTTPRetriever(url, new CompositionRetrievalPostProcessor(tile.getFile()));
        retriever.setConnectTimeout(10000);
        retriever.setReadTimeout(60000);
        retriever.call();
    }

    protected class CompositionRetrievalPostProcessor : AbstractRetrievalPostProcessor
    {
        // Note: Requested data is never marked as absent because the caller may want to continually re-try retrieval
        protected File outFile;

        public CompositionRetrievalPostProcessor(File outFile)
        {
            this.outFile = outFile;
        }

        protected File doGetOutputFile()
        {
            return this.outFile;
        }

        protected override bool overwriteExistingFile()
        {
            return true;
        }

        protected override bool isDeleteOnExit(File outFile)
        {
            return outFile.getPath().contains(WWIO.DELETE_ON_EXIT_PREFIX);
        }
    }

    //**************************************************************//
    //********************  Restorable Support  ********************//
    //**************************************************************//

    public void getRestorableStateForAVPair(String key, Object value,
        RestorableSupport rs, RestorableSupport.StateObject context)
    {
        if (value is URLBuilder)
        {
            rs.addStateValueAsString(context, "wms.Version", ((URLBuilder) value).wmsVersion);
            rs.addStateValueAsString(context, "wms.Crs", ((URLBuilder) value).crs);
        }
        else
        {
            base.getRestorableStateForAVPair(key, value, rs, context);
        }
    }

    protected static AVList wmsRestorableStateToParams(String stateInXml)
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
        wmsRestoreStateForParams(rs, null, parameters);
        return parameters;
    }

    protected static void wmsRestoreStateForParams(RestorableSupport rs, RestorableSupport.StateObject context,
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

        s = rs.getStateValueAsString(context, AVKey.LAYER_NAMES);
        if (s != null)
            parameters.setValue(AVKey.LAYER_NAMES, s);

        s = rs.getStateValueAsString(context, AVKey.STYLE_NAMES);
        if (s != null)
            parameters.setValue(AVKey.STYLE_NAMES, s);

        s = rs.getStateValueAsString(context, "wms.Version");
        parameters.setValue(AVKey.TILE_URL_BUILDER, new URLBuilder(s, parameters));
    }
}
}
