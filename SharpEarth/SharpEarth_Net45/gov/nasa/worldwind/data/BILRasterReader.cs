/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.io.File;
using SharpEarth.util;
using SharpEarth.geom.Sector;
using SharpEarth.formats.worldfile;
using SharpEarth.avlist;
namespace SharpEarth.data{



/**
 * @author dcollins
 * @version $Id: BILRasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class BILRasterReader : AbstractDataRasterReader
{
    private static final String[] bilMimeTypes = new String[]
        {"image/bil", "application/bil", "application/bil16", "application/bil32"};

    private static final String[] bilSuffixes = new String[]
        {"bil", "bil16", "bil32", "bil.gz", "bil16.gz", "bil32.gz"};

    private bool mapLargeFiles = false;
    private long largeFileThreshold = 16777216L; // 16 megabytes

    public BILRasterReader()
    {
        super(bilMimeTypes, bilSuffixes);
    }

    public bool isMapLargeFiles()
    {
        return this.mapLargeFiles;
    }

    public void setMapLargeFiles(boolean mapLargeFiles)
    {
        this.mapLargeFiles = mapLargeFiles;
    }

    public long getLargeFileThreshold()
    {
        return this.largeFileThreshold;
    }

    public void setLargeFileThreshold(long largeFileThreshold)
    {
        if (largeFileThreshold < 0L)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", "largeFileThreshold < 0");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.largeFileThreshold = largeFileThreshold;
    }

    protected bool doCanRead(Object source, AVList parameters)
    {
        if (!(source is java.io.File) && !(source is java.net.URL))
        {
            return false;
        }

        // If the data source doesn't already have all the necessary metadata, then we determine whether or not
        // the missing metadata can be read.
        String error = this.validateMetadata(source, parameters);
        if (!WWUtil.isEmpty(error))
        {
            if (!WorldFile.hasWorldFiles(source))
            {
                Logging.logger().fine(error);
                return false;
            }
        }

        if (null != parameters)
        {
            if (!params.hasKey(AVKey.PIXEL_FORMAT))
            {
                parameters.setValue(AVKey.PIXEL_FORMAT, AVKey.ELEVATION);
            }
        }

        return true;
    }

    protected DataRaster[] doRead(Object source, AVList parameters) throws java.io.IOException
    {
        java.nio.ByteBuffer byteBuffer = this.readElevations(source);

        // If the parameter list is null, or doesn't already have all the necessary metadata, then we copy the parameter
        // list and attempt to populate the copy with any missing metadata.        
        if (this.validateMetadata(source, parameters) != null)
        {
            // Copy the parameter list to insulate changes from the caller.
            parameters = (params != null) ? parameters.copy() : new AVListImpl();
            parameters.setValue(AVKey.FILE_SIZE, byteBuffer.capacity());
            WorldFile.readWorldFiles(source, parameters);
        }

        int width = (Integer) parameters.getValue(AVKey.WIDTH);
        int height = (Integer) parameters.getValue(AVKey.HEIGHT);
        Sector sector = (Sector) parameters.getValue(AVKey.SECTOR);

        if (!params.hasKey(AVKey.PIXEL_FORMAT))
        {
            parameters.setValue(AVKey.PIXEL_FORMAT, AVKey.ELEVATION);
        }

        ByteBufferRaster raster = new ByteBufferRaster(width, height, sector, byteBuffer, parameters);
        ElevationsUtil.rectify(raster);
        return new DataRaster[] { raster };
    }

    protected void doReadMetadata(Object source, AVList parameters) throws java.io.IOException
    {
        if (this.validateMetadata(source, parameters) != null)
        {
            WorldFile.readWorldFiles(source, parameters);
        }
    }

    protected String validateMetadata(Object source, AVList parameters)
    {
        StringBuilder sb = new StringBuilder();

        String message = super.validateMetadata(source, parameters);
        if (message != null)
        {
            sb.append(message);
        }

        Object o = (params != null) ? parameters.getValue(AVKey.BYTE_ORDER) : null;
        if (o == null || !(o is String))
        {
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.NoByteOrderSpecified", source));
        }

        o = (params != null) ? parameters.getValue(AVKey.PIXEL_FORMAT) : null;
        if (o == null)
        {
            sb.append(sb.length() > 0 ? ", " : "").append(
                Logging.getMessage("WorldFile.NoPixelFormatSpecified", source));
        }
        else if (!AVKey.ELEVATION.Equals(o))
        {
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.InvalidPixelFormat", source));
        }

        o = (params != null) ? parameters.getValue(AVKey.DATA_TYPE) : null;
        if (o == null)
        {
            sb.append(sb.length() > 0 ? ", " : "").append(Logging.getMessage("WorldFile.NoDataTypeSpecified", source));
        }

        if (sb.length() == 0)
        {
            return null;
        }

        return sb.ToString();
    }

    private java.nio.ByteBuffer readElevations(Object source) throws java.io.IOException
    {
        if (!(source is java.io.File) && !(source is java.net.URL))
        {
            String message = Logging.getMessage("DataRaster.CannotRead", source);
            Logging.logger().severe(message);
            throw new java.io.IOException(message);
        }

        File file = (source is java.io.File) ? (File) source : null;
        java.net.URL url = (source is java.net.URL) ? (java.net.URL) source : null;

        if (null == file && "file".equalsIgnoreCase(url.getProtocol()))
        {
            file = new File(url.getFile());
        }

        if (null != file)
        {
            // handle .bil.zip, .bil16.zip, and .bil32.gz files
            if (file.getName().toLowerCase().endsWith(".zip"))
            {
                return WWIO.readZipEntryToBuffer(file, null);
            }
            // handle bil.gz, bil16.gz, and bil32.gz files
            else if (file.getName().toLowerCase().endsWith(".gz"))
            {
                return WWIO.readGZipFileToBuffer(file);
            }
            else if (!this.isMapLargeFiles() || (this.getLargeFileThreshold() > file.length()))
            {
                return WWIO.readFileToBuffer(file);
            }
            else
            {
                return WWIO.mapFile(file);
            }
        }
        else // (source is java.net.URL)
        {
            return WWIO.readURLContentToBuffer(url);
        }
    }
}
}
