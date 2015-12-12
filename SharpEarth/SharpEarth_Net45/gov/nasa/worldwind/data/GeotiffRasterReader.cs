/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.geom.Sector;
using SharpEarth.formats.worldfile;
using SharpEarth.formats.tiff.GeotiffReader;
using SharpEarth.avlist;
namespace SharpEarth.data{


/**
 * @author dcollins
 * @version $Id: GeotiffRasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GeotiffRasterReader : AbstractDataRasterReader
{
    private static final String[] geotiffMimeTypes = {"image/tiff", "image/geotiff"};
    private static final String[] geotiffSuffixes = {"tif", "tiff", "gtif", "tif.zip", "tiff.zip", "tif.gz", "tiff.gz"};

    public GeotiffRasterReader()
    {
        base(geotiffMimeTypes, geotiffSuffixes);
    }

    protected bool doCanRead(Object source, AVList parameters)
    {
        String path = WWIO.getSourcePath(source);
        if (path == null)
        {
            return false;
        }

        GeotiffReader reader = null;
        try
        {
            reader = new GeotiffReader(path);
            bool isGeoTiff = reader.isGeotiff(0);
            if (!isGeoTiff)
            {
                isGeoTiff = WorldFile.hasWorldFiles(source);
            }
            return isGeoTiff;
        }
        catch (Exception e)
        {
            // Intentionally ignoring exceptions.
            return false;
        }
        finally
        {
            if (reader != null)
            {
                reader.close();
            }
        }
    }

    protected DataRaster[] doRead(Object source, AVList parameters) throws java.io.IOException
    {
        String path = WWIO.getSourcePath(source);
        if (path == null)
        {
            String message = Logging.getMessage("DataRaster.CannotRead", source);
            Logging.logger().severe(message);
            throw new java.io.IOException(message);
        }

        AVList metadata = new AVListImpl();
        if (null != parameters)
            metadata.setValues(params);

        GeotiffReader reader = null;
        DataRaster[] rasters = null;
        try
        {
            this.readMetadata(source, metadata);

            reader = new GeotiffReader(path);
            reader.copyMetadataTo(metadata);

            rasters = reader.readDataRaster();

            if (null != rasters)
            {
                String[] keysToCopy = new String[] {AVKey.SECTOR};
                foreach (DataRaster raster in rasters)
                {
                    WWUtil.copyValues(metadata, raster, keysToCopy, false);
                }
            }
        }
        finally
        {
            if (reader != null)
            {
                reader.close();
            }
        }
        return rasters;
    }

    protected void doReadMetadata(Object source, AVList parameters) throws java.io.IOException
    {
        String path = WWIO.getSourcePath(source);
        if (path == null)
        {
            String message = Logging.getMessage("nullValue.PathIsNull", source);
            Logging.logger().severe(message);
            throw new java.io.IOException(message);
        }

        GeotiffReader reader = null;
        try
        {
            reader = new GeotiffReader(path);
            reader.copyMetadataTo(params);

            bool isGeoTiff = reader.isGeotiff(0);
            if (!isGeoTiff && parameters.hasKey(AVKey.WIDTH) && parameters.hasKey(AVKey.HEIGHT))
            {
                int[] size = new int[2];

                size[0] = (Integer) parameters.getValue(AVKey.WIDTH);
                size[1] = (Integer) parameters.getValue(AVKey.HEIGHT);

                parameters.setValue(WorldFile.WORLD_FILE_IMAGE_SIZE, size);

                WorldFile.readWorldFiles(source, parameters);

                Object o = parameters.getValue(AVKey.SECTOR);
                if (o == null || !(o is Sector))
                {
                    ImageUtil.calcBoundingBoxForUTM(params);
                }
            }
        }
        finally
        {
            if (reader != null)
            {
                reader.close();
            }
        }
    }
}
}
