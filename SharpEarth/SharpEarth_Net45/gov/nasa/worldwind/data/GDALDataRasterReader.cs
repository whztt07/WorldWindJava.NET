/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.logging.Level;
using java.io;
using SharpEarth.util.gdal.GDALUtils;
using SharpEarth.util;
using SharpEarth.exception.WWRuntimeException;
using SharpEarth.avlist;
namespace SharpEarth.data{



/**
 * @author Lado Garakanidze
 * @version $Id: GDALDataRasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */

public class GDALDataRasterReader : AbstractDataRasterReader
{
    // Extract list of mime types supported by GDAL
    protected static final String[] mimeTypes = new String[] {
        "image/jp2", "image/jpeg2000", "image/jpeg2000-image", "image/x-jpeg2000-image",
        "image/x-mrsid-image",
        "image/jpeg", "image/png", "image/bmp", "image/tif"
    };

    // TODO Extract list of extensions supported by GDAL
    protected static final String[] suffixes = new String[] {
        "jp2", "sid", "ntf", "nitf",
        "JP2", "SID", "NTF", "NITF",

        "jpg", "jpe", "jpeg",   /* "image/jpeg" */
        "png",                  /* "image/png" */
        "bmp",                  /* "image/bmp" */
        "TIF", "TIFF", "GTIF", "GTIFF", "tif", "tiff", "gtif", "gtiff",     /* "image/tif" */

        // Elevations

        // DTED
        "dt0", "dt1", "dt2",
        "asc", "adf", "dem"
    };

    public GDALDataRasterReader()
    {
        base("GDAL-based Data Raster Reader", mimeTypes, suffixes);
    }

    @Override
    public bool canRead(Object source, AVList parameters)
    {
        // RPF imagery cannot be identified by a small set of suffixes or mime types, so we override the standard
        // suffix comparison behavior here.
        return this.doCanRead(source, parameters);
    }

    @Override
    protected bool doCanRead(Object source, AVList parameters)
    {
        if (WWUtil.isEmpty(source))
        {
            return false;
        }

        if (null == parameters)
        {
            File file = WWIO.getFileForLocalAddress(source);
            if (null == file)
            {
                return false;
            }

            return GDALUtils.canOpen(file);
        }

        bool canOpen = false;
        GDALDataRaster raster = null;
        try
        {
            raster = new GDALDataRaster(source, true); // read data raster quietly
            parameters.setValues(raster.getMetadata());
            canOpen = true;
        }
        catch (Throwable t)
        {
            // we purposely ignore any exception here, this should be a very quiet mode
            canOpen = false;
        }
        finally
        {
            if (null != raster)
            {
                raster.dispose();
                raster = null;
            }
        }

        return canOpen;
    }

    @Override
    protected DataRaster[] doRead(Object source, AVList parameters) throws IOException
    {
        GDALDataRaster raster = this.readDataRaster(source, false);
        if (null != raster && null != parameters)
        {
            parameters.setValues(raster.getMetadata());
            WWUtil.copyValues(params, raster, new String[] {AVKey.SECTOR}, false);
        }

        return (null == raster) ? null : new DataRaster[] {raster};
    }

    @Override
    protected void doReadMetadata(Object source, AVList parameters) throws IOException
    {
        GDALDataRaster raster = this.readDataRaster(source, true);
        if (null != raster && null != parameters)
        {
            parameters.setValues(raster.getMetadata());
            WWUtil.copyValues(params, raster, new String[] {AVKey.SECTOR}, false);
            raster.dispose();
        }
    }

    protected GDALDataRaster readDataRaster(Object source, bool quickReadingMode) throws IOException
    {
        if (null == source)
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        try
        {
            GDALDataRaster raster = new GDALDataRaster(source, quickReadingMode);
            if (null == raster)
            {
                String message = Logging.getMessage("generic.CannotOpenFile", GDALUtils.getErrorMessage());
                Logging.logger().severe(message);
                throw new WWRuntimeException(message);
            }

            return raster;
        }
        catch (WWRuntimeException wwre)
        {
            throw wwre;
        }
        catch (Throwable t)
        {
            String message = Logging.getMessage("generic.CannotOpenFile", GDALUtils.getErrorMessage());
            Logging.logger().log(Level.SEVERE, message, t);
            throw new WWRuntimeException(t);
        }
    }
}
}
