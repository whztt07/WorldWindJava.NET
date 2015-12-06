/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.Calendar;
using SharpEarth.util;
using SharpEarth.geom;
using SharpEarth.formats.tiff.GeoTiff;
using SharpEarth.avlist;
using SharpEarth.Version;
namespace SharpEarth.data{



/**
 * @author dcollins
 * @version $Id: ByteBufferRaster.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class ByteBufferRaster extends BufferWrapperRaster
{
    private java.nio.ByteBuffer byteBuffer;

    public ByteBufferRaster(int width, int height, Sector sector, java.nio.ByteBuffer byteBuffer, AVList list)
    {
        super(width, height, sector, BufferWrapper.wrap(byteBuffer, list), list);

        this.byteBuffer = byteBuffer;

        this.validateParameters(list);
    }

    private void validateParameters(AVList list) throws ArgumentException
    {
        this.doValidateParameters(list);
    }

    protected void doValidateParameters(AVList list) throws ArgumentException
    {
    }

    public ByteBufferRaster(int width, int height, Sector sector, AVList parameters)
    {
        this(width, height, sector, createCompatibleBuffer(width, height, parameters), parameters);
    }

    public static java.nio.ByteBuffer createCompatibleBuffer(int width, int height, AVList parameters)
    {
        if (width < 1)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", "width < 1");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (height < 1)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", "height < 1");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (params == null)
        {
            String message = Logging.getMessage("nullValue.ParamsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Object dataType = parameters.getValue(AVKey.DATA_TYPE);

        int sizeOfDataType = 0;
        if (AVKey.INT8.Equals(dataType))
            sizeOfDataType = (Byte.SIZE / 8);
        else if (AVKey.INT16.Equals(dataType))
            sizeOfDataType = (Short.SIZE / 8);
        else if (AVKey.INT32.Equals(dataType))
            sizeOfDataType = (Integer.SIZE / 8);
        else if (AVKey.FLOAT32.Equals(dataType))
            sizeOfDataType = (Float.SIZE / 8);

        int sizeInBytes = sizeOfDataType * width * height;
        return java.nio.ByteBuffer.allocate(sizeInBytes);
    }

    public java.nio.ByteBuffer getByteBuffer()
    {
        return this.byteBuffer;
    }

    public static DataRaster createGeoreferencedRaster(AVList parameters)
    {
        if (null == parameters)
        {
            String msg = Logging.getMessage("nullValue.AVListIsNull");
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        if (!params.hasKey(AVKey.WIDTH))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.WIDTH);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        int width = (Integer) parameters.getValue(AVKey.WIDTH);

        if (!(width > 0))
        {
            String msg = Logging.getMessage("generic.InvalidWidth", width);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        if (!params.hasKey(AVKey.HEIGHT))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.HEIGHT);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        int height = (Integer) parameters.getValue(AVKey.HEIGHT);

        if (!(height > 0))
        {
            String msg = Logging.getMessage("generic.InvalidWidth", height);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        if (!params.hasKey(AVKey.SECTOR))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.SECTOR);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        Sector sector = (Sector) parameters.getValue(AVKey.SECTOR);
        if (null == sector)
        {
            String msg = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (!params.hasKey(AVKey.COORDINATE_SYSTEM))
        {
            // assume Geodetic Coordinate System
            parameters.setValue(AVKey.COORDINATE_SYSTEM, AVKey.COORDINATE_SYSTEM_GEOGRAPHIC);
        }

        String cs = parameters.getStringValue(AVKey.COORDINATE_SYSTEM);
        if (!params.hasKey(AVKey.PROJECTION_EPSG_CODE))
        {
            if (AVKey.COORDINATE_SYSTEM_GEOGRAPHIC.Equals(cs))
            {
                // assume WGS84
                parameters.setValue(AVKey.PROJECTION_EPSG_CODE, GeoTiff.GCS.WGS_84);
            }
            else
            {
                String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.PROJECTION_EPSG_CODE);
                Logging.logger().finest(msg);
                throw new ArgumentException(msg);
            }
        }

        // if PIXEL_WIDTH is specified, we are not overriding it because UTM images
        // will have different pixel size
        if (!params.hasKey(AVKey.PIXEL_WIDTH))
        {
            if (AVKey.COORDINATE_SYSTEM_GEOGRAPHIC.Equals(cs))
            {
                double pixelWidth = sector.getDeltaLonDegrees() / (double) width;
                parameters.setValue(AVKey.PIXEL_WIDTH, pixelWidth);
            }
            else
            {
                String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.PIXEL_WIDTH);
                Logging.logger().finest(msg);
                throw new ArgumentException(msg);
            }
        }

        // if PIXEL_HEIGHT is specified, we are not overriding it
        // because UTM images will have different pixel size
        if (!params.hasKey(AVKey.PIXEL_HEIGHT))
        {
            if (AVKey.COORDINATE_SYSTEM_GEOGRAPHIC.Equals(cs))
            {
                double pixelHeight = sector.getDeltaLatDegrees() / (double) height;
                parameters.setValue(AVKey.PIXEL_HEIGHT, pixelHeight);
            }
            else
            {
                String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.PIXEL_HEIGHT);
                Logging.logger().finest(msg);
                throw new ArgumentException(msg);
            }
        }

        if (!params.hasKey(AVKey.PIXEL_FORMAT))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.PIXEL_FORMAT);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }
        else
        {
            String pixelFormat = parameters.getStringValue(AVKey.PIXEL_FORMAT);
            if (!AVKey.ELEVATION.Equals(pixelFormat) && !AVKey.IMAGE.Equals(pixelFormat))
            {
                String msg = Logging.getMessage("generic.UnknownValueForKey", pixelFormat, AVKey.PIXEL_FORMAT);
                Logging.logger().severe(msg);
                throw new ArgumentException(msg);
            }
        }

        if (!params.hasKey(AVKey.DATA_TYPE))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.DATA_TYPE);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        // validate elevation parameters
        if (AVKey.ELEVATION.Equals(params.getValue(AVKey.PIXEL_FORMAT)))
        {
            String type = parameters.getStringValue(AVKey.DATA_TYPE);
            if (!AVKey.FLOAT32.Equals(type) && !AVKey.INT16.Equals(type))
            {
                String msg = Logging.getMessage("generic.UnknownValueForKey", type, AVKey.DATA_TYPE);
                Logging.logger().severe(msg);
                throw new ArgumentException(msg);
            }
        }

        if (!params.hasKey(AVKey.ORIGIN) && AVKey.COORDINATE_SYSTEM_GEOGRAPHIC.Equals(cs))
        {
            // set UpperLeft corner as the origin, if not specified
            LatLon origin = new LatLon(sector.getMaxLatitude(), sector.getMinLongitude());
            parameters.setValue(AVKey.ORIGIN, origin);
        }

        if (!params.hasKey(AVKey.BYTE_ORDER))
        {
            String msg = Logging.getMessage("generic.MissingRequiredParameter", AVKey.BYTE_ORDER);
            Logging.logger().finest(msg);
            throw new ArgumentException(msg);
        }

        if (!params.hasKey(AVKey.DATE_TIME))
        {
            // add NUL (\0) termination as required by TIFF v6 spec (20 bytes length)
            String timestamp = String.Format("%1$tY:%1$tm:%1$td %tT\0", Calendar.getInstance());
            parameters.setValue(AVKey.DATE_TIME, timestamp);
        }

        if (!params.hasKey(AVKey.VERSION))
        {
            parameters.setValue(AVKey.VERSION, Version.getVersion());
        }

        return new ByteBufferRaster(width, height, sector, parameters);
    }
}
}
