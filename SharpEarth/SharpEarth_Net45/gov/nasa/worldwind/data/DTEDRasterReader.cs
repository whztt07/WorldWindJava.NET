/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.io;
using SharpEarth.util;
using SharpEarth.formats.dted.DTED;
using SharpEarth.avlist;
namespace SharpEarth.data{



/**
 * @author Lado Garakanidze
 * @version $Id: DTEDRasterReader.java 3037 2015-04-17 23:08:47Z tgaskins $
 */

public class DTEDRasterReader : AbstractDataRasterReader
{
    protected static final String[] dtedMimeTypes = new String[] {
        "application/dted",
        "application/dt0", "application/dted-0",
        "application/dt1", "application/dted-1",
        "application/dt2", "application/dted-2",
    };

    protected static final String[] dtedSuffixes = new String[]
        {"dt0", "dt1", "dt2"};

    public DTEDRasterReader()
    {
        base(dtedMimeTypes, dtedSuffixes);
    }

    @Override
    protected bool doCanRead(Object source, AVList parameters)
    {
        File file = this.getFile(source);
        if (null == file)
        {
            return false;
        }

        // Assume that a proper suffix reliably identifies a DTED file. Otherwise the file will have to be loaded
        // to determine that, and there are often tens of thousands of DTED files, which causes raster server start-up
        // times to be excessive.
        if (this.canReadSuffix(source))
        {
            parameters.setValue(AVKey.PIXEL_FORMAT, AVKey.ELEVATION); // we know that DTED is elevation data
            return true;
        }

        bool canRead = false;
        try
        {
            AVList metadata = DTED.readMetadata(file);
            if (null != metadata)
            {
                if (null != parameters)
                {
                    parameters.setValues(metadata);
                }

                canRead = AVKey.ELEVATION.Equals(metadata.getValue(AVKey.PIXEL_FORMAT));
            }
        }
        catch (Throwable t)
        {
            Logging.logger().finest(t.getMessage());
            canRead = false;
        }

        return canRead;
    }

    @Override
    protected DataRaster[] doRead(Object source, AVList parameters) throws IOException
    {
        File file = this.getFile(source);
        if (null == file)
        {
            String message = Logging.getMessage("generic.UnrecognizedSourceTypeOrUnavailableSource", source);
            Logging.logger().severe(message);
            throw new IOException(message);
        }

        // This may be the first time the file has been opened, so pass the metadata list to the read method
        // in order to update that list with the file's metadata.
        DataRaster raster = DTED.read(file, parameters);
        if (raster is ByteBufferRaster)
            ElevationsUtil.rectify((ByteBufferRaster) raster);

        return new DataRaster[] {raster};
    }

    @Override
    protected void doReadMetadata(Object source, AVList parameters) throws IOException
    {
        File file = this.getFile(source);
        if (null == file)
        {
            String message = Logging.getMessage("generic.UnrecognizedSourceTypeOrUnavailableSource", source);
            Logging.logger().severe(message);
            throw new IOException(message);
        }

        AVList metadata = DTED.readMetadata(file);
        if (null != metadata && null != parameters)
        {
            parameters.setValues(metadata);
        }
    }

    protected File getFile(Object source)
    {
        if (null == source)
        {
            return null;
        }
        else if (source is java.io.File)
        {
            return (File) source;
        }
        else if (source is java.net.URL)
        {
            return WWIO.convertURLToFile((java.net.URL) source);
        }
        else
        {
            return null;
        }
    }

    protected String validateMetadata(Object source, AVList parameters)
    {
        // Don't validate anything so we can avoid reading the metadata at start-up. Assume that the
        // sector will come from the config file and that the pixel type is specified in doCanRead above.
        return null;
    }
}
}
