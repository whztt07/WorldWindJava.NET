/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util;
using SharpEarth.avlist.AVList;
namespace SharpEarth.data{


/**
 * Implements a {@link SharpEarth.data.DataRasterReaderFactory} with a default list of readers. The list
 * includes the following readers:
 * <pre>
 *  {@link SharpEarth.data.RPFRasterReader}
 *  {@link SharpEarth.data.DTEDRasterReader}
 *  {@link SharpEarth.data.GDALDataRasterReader}
 *  {@link SharpEarth.data.GeotiffRasterReader}
 *  {@link SharpEarth.data.BILRasterReader}
 *  {@link SharpEarth.data.ImageIORasterReader}

 * </pre>
 * <p/>
 * To specify a different factory, set the {@link SharpEarth.avlist.AVKey#DATA_RASTER_READER_FACTORY_CLASS_NAME}
 * value in {@link SharpEarth.Configuration}, either directly or via the World Wind configuration file. To add
 * readers to the default set, create a subclass of this class, override {@link #findReaderFor(Object,
 * SharpEarth.avlist.AVList)}, and specify the new class to the configuration.
 *
 * @author tag
 * @version $Id: BasicDataRasterReaderFactory.java 1511 2013-07-17 17:34:00Z dcollins $
 */
public class BasicDataRasterReaderFactory implements DataRasterReaderFactory
{
    /** The default list of readers. */
    protected DataRasterReader[] readers = new DataRasterReader[]
        {
            // NOTE: Update the javadoc above if this list changes.
            new RPFRasterReader(),
            new DTEDRasterReader(),
            new GDALDataRasterReader(),
            new GeotiffRasterReader(),
            new BILRasterReader(),
            new ImageIORasterReader(),
        };

    /** {@inheritDoc} */
    public DataRasterReader[] getReaders()
    {
        return readers;
    }

    /** {@inheritDoc} */
    public DataRasterReader findReaderFor(Object source, AVList parameters)
    {
        if (source == null)
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return findReaderFor(source, parameters, readers);
    }

    /** {@inheritDoc} */
    public DataRasterReader findReaderFor(Object source, AVList parameters, DataRasterReader[] readers)
    {
        if (source == null)
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (readers == null)
        {
            String message = Logging.getMessage("nullValue.ReaderIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        for (DataRasterReader reader : readers)
        {
            if (reader != null && reader.canRead(source, parameters))
                return reader;
        }

        return null;
    }
}
}
