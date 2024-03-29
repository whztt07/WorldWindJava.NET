/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.Map;
using SharpEarth.util;
using SharpEarth.geom.Sector;
using SharpEarth.avlist;
namespace SharpEarth.data{



/**
 * @author Lado Garakanidze
 * @version $Id: AbstractDataRaster.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public abstract class AbstractDataRaster : AVListImpl , DataRaster
{
    protected int width = 0;
    protected int height = 0;

    protected AbstractDataRaster()
    {
        base();
    }

    protected AbstractDataRaster(int width, int height, Sector sector) throws ArgumentException
    {
        base();

        if (width < 0)
        {
            String message = Logging.getMessage("generic.InvalidWidth", width);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (height < 0)
        {
            String message = Logging.getMessage("generic.InvalidHeight", height);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (sector == null)
        {
            String message = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().finest(message);
//            throw new ArgumentException(message);
        }

        // for performance reasons we are "caching" these parameters in addition to AVList
        this.width = width;
        this.height = height;

        if (null != sector)
        {
            this.setValue(AVKey.SECTOR, sector);
        }

        this.setValue(AVKey.WIDTH, width);
        this.setValue(AVKey.HEIGHT, height);
    }

    protected AbstractDataRaster(int width, int height, Sector sector, AVList list) throws ArgumentException
    {
        this(width, height, sector);

        if (null != list)
        {
            foreach (Map.Entry<String, Object> entry in list.getEntries())
            {
                this.setValue(entry.getKey(), entry.getValue());
            }
        }
    }

    public int getWidth()
    {
        return this.width;
    }

    public int getHeight()
    {
        return this.height;
    }

    public Sector getSector()
    {
        if (this.hasKey(AVKey.SECTOR))
        {
            return (Sector) this.getValue(AVKey.SECTOR);
        }
        return null;
    }

    @Override
    public Object setValue(String key, Object value)
    {
        if (null == key)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        // Do not allow to change existing WIDTH or HEIGHT

        if (this.hasKey(key))
        {
            if (AVKey.WIDTH.Equals(key) && this.getWidth() != (Integer) value)
            {
                String message = Logging.getMessage("generic.AttemptToChangeReadOnlyProperty", key);
                Logging.logger().finest(message);
                // relax restriction, just log and continue
//                throw new ArgumentException(message);
                return this;
            }
            else if (AVKey.HEIGHT.Equals(key) && this.getHeight() != (Integer) value)
            {
                String message = Logging.getMessage("generic.AttemptToChangeReadOnlyProperty", key);
                Logging.logger().finest(message);
                // relax restriction, just log and continue
//                throw new ArgumentException(message);
                return this;
            }
        }
        return base.setValue(key, value);
    }

    protected java.awt.Rectangle computeClipRect(Sector clipSector, DataRaster clippedRaster)
    {
        java.awt.geom.AffineTransform geographicToRaster = this.computeGeographicToRasterTransform(
            clippedRaster.getWidth(), clippedRaster.getHeight(), clippedRaster.getSector());

        java.awt.geom.Point2D geoPoint = new java.awt.geom.Point2D.Double();
        java.awt.geom.Point2D ul = new java.awt.geom.Point2D.Double();
        java.awt.geom.Point2D lr = new java.awt.geom.Point2D.Double();

        geoPoint.setLocation(clipSector.getMinLongitude().degrees, clipSector.getMaxLatitude().degrees);
        geographicToRaster.transform(geoPoint, ul);

        geoPoint.setLocation(clipSector.getMaxLongitude().degrees, clipSector.getMinLatitude().degrees);
        geographicToRaster.transform(geoPoint, lr);

        int x = (int) Math.Floor(ul.getX());
        int y = (int) Math.Floor(ul.getY());
        int width = (int) Math.Ceiling(lr.getX() - ul.getX());
        int height = (int) Math.Ceiling(lr.getY() - ul.getY());

        return new java.awt.Rectangle(x, y, width, height);
    }

    protected java.awt.geom.AffineTransform computeSourceToDestTransform(
        int sourceWidth, int sourceHeight, Sector sourceSector,
        int destWidth, int destHeight, Sector destSector)
    {
        // Compute the the transform from source to destination coordinates. In this computation a pixel is assumed
        // to cover a finite area.

        double ty = destHeight * -(sourceSector.getMaxLatitude().degrees - destSector.getMaxLatitude().degrees)
            / destSector.getDeltaLatDegrees();
        double tx = destWidth * (sourceSector.getMinLongitude().degrees - destSector.getMinLongitude().degrees)
            / destSector.getDeltaLonDegrees();

        double sy = ((double) destHeight / (double) sourceHeight)
            * (sourceSector.getDeltaLatDegrees() / destSector.getDeltaLatDegrees());
        double sx = ((double) destWidth / (double) sourceWidth)
            * (sourceSector.getDeltaLonDegrees() / destSector.getDeltaLonDegrees());

        java.awt.geom.AffineTransform transform = new java.awt.geom.AffineTransform();
        transform.translate(tx, ty);
        transform.scale(sx, sy);
        return transform;
    }

    protected java.awt.geom.AffineTransform computeGeographicToRasterTransform(int width, int height, Sector sector)
    {
        // Compute the the transform from geographic to raster coordinates. In this computation a pixel is assumed
        // to cover a finite area.

        double ty = -sector.getMaxLatitude().degrees;
        double tx = -sector.getMinLongitude().degrees;

        double sy = -(height / sector.getDeltaLatDegrees());
        double sx = (width / sector.getDeltaLonDegrees());

        java.awt.geom.AffineTransform transform = new java.awt.geom.AffineTransform();
        transform.scale(sx, sy);
        transform.translate(tx, ty);
        return transform;
    }

    public DataRaster getSubRaster(int width, int height, Sector sector, AVList parameters)
    {
        parameters = (null == parameters) ? new AVListImpl() : parameters;

        // copy parent raster keys/values; only those key/value will be copied that do exist in the parent raster
        // AND does NOT exist in the requested raster
        String[] keysToCopy = new String[] {
            AVKey.DATA_TYPE, AVKey.MISSING_DATA_SIGNAL, AVKey.BYTE_ORDER, AVKey.PIXEL_FORMAT, AVKey.ELEVATION_UNIT
        };
        WWUtil.copyValues(this, parameters, keysToCopy, false);

        parameters.setValue(AVKey.WIDTH, width);
        parameters.setValue(AVKey.HEIGHT, height);
        parameters.setValue(AVKey.SECTOR, sector);

        return this.getSubRaster(params);
    }

    /**
     * Reads the specified region of interest (ROI) with given extent, width, and height, and type
     *
     * @param parameters Required parameters are:
     *               <p/>
     *               AVKey.HEIGHT as Integer, specifies a height of the desired ROI AVKey.WIDTH as Integer, specifies a
     *               width of the desired ROI AVKey.SECTOR as Sector, specifies an extent of the desired ROI
     *               <p/>
     *               Optional parameters are:
     *               <p/>
     *               AVKey.BAND_ORDER as array of integers, examples: for RGBA image: new int[] { 0, 1, 2, 3 }, or for
     *               ARGB image: new int[] { 3, 0, 1, 2 } or if you want only RGB bands of the RGBA image: new int[] {
     *               0, 1, 2 } or only Intensity (4th) band of the specific aerial image: new int[] { 3 }
     *
     * @return DataRaster (BufferedImageRaster for imagery or ByteBufferDataRaster for elevations)
     */
    public DataRaster getSubRaster(AVList parameters)
    {
        if (null == parameters)
        {
            String message = Logging.getMessage("nullValue.ParamsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (!params.hasKey(AVKey.WIDTH))
        {
            String message = Logging.getMessage("generic.MissingRequiredParameter", AVKey.WIDTH);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        int roiWidth = (Integer) parameters.getValue(AVKey.WIDTH);
        if (roiWidth <= 0)
        {
            String message = Logging.getMessage("generic.InvalidWidth", roiWidth);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (!params.hasKey(AVKey.HEIGHT))
        {
            String message = Logging.getMessage("generic.MissingRequiredParameter", AVKey.HEIGHT);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        int roiHeight = (Integer) parameters.getValue(AVKey.HEIGHT);
        if (roiHeight <= 0)
        {
            String message = Logging.getMessage("generic.InvalidHeight", roiHeight);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (!params.hasKey(AVKey.SECTOR))
        {
            String message = Logging.getMessage("generic.MissingRequiredParameter", AVKey.SECTOR);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Sector roiSector = (Sector) parameters.getValue(AVKey.SECTOR);
        if (null == roiSector || Sector.EMPTY_SECTOR.Equals(roiSector))
        {
            String message = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (Sector.EMPTY_SECTOR.Equals(roiSector))
        {
            String message = Logging.getMessage("nullValue.SectorGeometryIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        // copy parent raster keys/values; only those key/value will be copied that do exist in the parent raster
        // AND does NOT exist in the requested raster
        String[] keysToCopy = new String[] {
            AVKey.DATA_TYPE, AVKey.MISSING_DATA_SIGNAL, AVKey.BYTE_ORDER, AVKey.PIXEL_FORMAT, AVKey.ELEVATION_UNIT
        };
        WWUtil.copyValues(this, parameters, keysToCopy, false);

        return this.doGetSubRaster(roiWidth, roiHeight, roiSector, parameters);
    }

    abstract DataRaster doGetSubRaster(int roiWidth, int roiHeight, Sector roiSector, AVList roiParams);
}
}
