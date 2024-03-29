/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util.WWIO;
using SharpEarth.util;
using SharpEarth.util.ImageUtil;
using SharpEarth.geom.Sector;
using SharpEarth.formats.worldfile;
using SharpEarth.formats.tiff.GeotiffImageReaderSpi;
using SharpEarth.avlist.AVListImpl;
using SharpEarth.avlist.AVList;
using SharpEarth.avlist;
namespace SharpEarth.data{


/**
 * @author dcollins
 * @version $Id: ImageIORasterReader.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class ImageIORasterReader : AbstractDataRasterReader
{
    static
    {
        javax.imageio.spi.IIORegistry.getDefaultInstance().registerServiceProvider(GeotiffImageReaderSpi.inst());
    }

    private bool generateMipMaps;

    public ImageIORasterReader(boolean generateMipMaps)
    {
        base(javax.imageio.ImageIO.getReaderMIMETypes(), getImageIOReaderSuffixes());
        this.generateMipMaps = generateMipMaps;
    }

    public ImageIORasterReader()
    {
        this(false);
    }

    public bool isGenerateMipMaps()
    {
        return this.generateMipMaps;
    }

    public void setGenerateMipMaps(boolean generateMipMaps)
    {
        this.generateMipMaps = generateMipMaps;
    }

    protected bool doCanRead(Object source, AVList parameters)
    {
        // Determine whether or not the data source can be read.
        //if (!this.canReadImage(source))
        //    return false;

        // If the data source doesn't already have all the necessary metadata, then we determine whether or not
        // the missing metadata can be read.
        Object o = (params != null) ? parameters.getValue(AVKey.SECTOR) : null;
        if (o == null || !(o is Sector))
        {
            if (!this.canReadWorldFiles(source))
            {
                return false;
            }
        }

        if (null != parameters && !params.hasKey(AVKey.PIXEL_FORMAT))
        {
            parameters.setValue(AVKey.PIXEL_FORMAT, AVKey.IMAGE);
        }

        return true;
    }

    protected DataRaster[] doRead(Object source, AVList parameters) throws java.io.IOException
    {
        javax.imageio.stream.ImageInputStream iis = createInputStream(source);
        java.awt.image.BufferedImage image = javax.imageio.ImageIO.read(iis);
        image = ImageUtil.toCompatibleImage(image);

        // If the data source doesn't already have all the necessary metadata, then we attempt to read the metadata.
        Object o = (params != null) ? parameters.getValue(AVKey.SECTOR) : null;
        if (o == null || !(o is Sector))
        {
            AVList values = new AVListImpl();
            values.setValue(AVKey.IMAGE, image);
            this.readWorldFiles(source, values);
            o = values.getValue(AVKey.SECTOR);
        }

        return new DataRaster[]{this.createRaster((Sector) o, image)};
    }

    protected void doReadMetadata(Object source, AVList parameters) throws java.io.IOException
    {
        Object width = parameters.getValue(AVKey.WIDTH);
        Object height = parameters.getValue(AVKey.HEIGHT);
        if (width == null || height == null || !(width is Integer) || !(height is Integer))
        {
            this.readImageDimension(source, parameters);
        }

        Object sector = parameters.getValue(AVKey.SECTOR);
        if (sector == null || !(sector is Sector))
        {
            this.readWorldFiles(source, parameters);
        }

        if (!params.hasKey(AVKey.PIXEL_FORMAT))
        {
            parameters.setValue(AVKey.PIXEL_FORMAT, AVKey.IMAGE);
        }
    }

    protected DataRaster createRaster(Sector sector, java.awt.image.BufferedImage image)
    {
        if (this.isGenerateMipMaps())
        {
            return new MipMappedBufferedImageRaster(sector, image);
        }
        else
        {
            return new BufferedImageRaster(sector, image);
        }
    }

    //private bool canReadImage(DataSource source)
    //{
    //    javax.imageio.stream.ImageInputStream iis = null;
    //    javax.imageio.ImageReader reader = null;
    //    try
    //    {
    //        iis = createInputStream(source);
    //        reader = readerFor(iis);
    //        if (reader == null)
    //            return false;
    //    }
    //    catch (Exception e)
    //    {
    //        // Not interested in logging the exception, we only want to report the failure to read.
    //        return false;
    //    }
    //    finally
    //    {
    //        if (reader != null)
    //            reader.dispose();
    //        try
    //        {
    //            if (iis != null)
    //                iis.close();
    //        }
    //        catch (Exception e)
    //        {
    //            // Not interested in logging the exception.
    //        }
    //    }
    //
    //    return true;
    //}

    private bool canReadWorldFiles(Object source)
    {
        if (!(source is java.io.File))
        {
            return false;
        }

        try
        {
            java.io.File[] worldFiles = WorldFile.getWorldFiles((java.io.File) source);
            if (worldFiles == null || worldFiles.length == 0)
            {
                return false;
            }
        }
        catch (java.io.IOException e)
        {
            // Not interested in logging the exception, we only want to report the failure to read.
            return false;
        }

        return true;
    }

    private void readImageDimension(Object source, AVList parameters) throws java.io.IOException
    {
        javax.imageio.stream.ImageInputStream iis = createInputStream(source);
        javax.imageio.ImageReader reader = readerFor(iis);
        try
        {
            if (reader == null)
            {
                String message = Logging.getMessage("generic.UnrecognizedImageSourceType", source);
                Logging.logger().severe(message);
                throw new java.io.IOException(message);
            }

            reader.setInput(iis, true, true);
            int width = reader.getWidth(0);
            int height = reader.getHeight(0);
            parameters.setValue(AVKey.WIDTH, width);
            parameters.setValue(AVKey.HEIGHT, height);
        }
        finally
        {
            if (reader != null)
            {
                reader.dispose();
            }
            iis.close();
        }
    }

    private void readWorldFiles(Object source, AVList parameters) throws java.io.IOException
    {
        if (!(source is java.io.File))
        {
            String message = Logging.getMessage("DataRaster.CannotRead", source);
            Logging.logger().severe(message);
            throw new java.io.IOException(message);
        }

        // If an image is not specified in the metadata values, then attempt to construct the image size from other
        // parameters.
        Object o = parameters.getValue(AVKey.IMAGE);
        if (o == null || !(o is java.awt.image.BufferedImage))
        {
            o = parameters.getValue(WorldFile.WORLD_FILE_IMAGE_SIZE);
            if (o == null || !(o is int[]))
            {
                // If the image size is specified in the parameters WIDTH and HEIGHT, then translate them to the
                // WORLD_FILE_IMAGE_SIZE parameter.
                Object width = parameters.getValue(AVKey.WIDTH);
                Object height = parameters.getValue(AVKey.HEIGHT);
                if (width != null && height != null && width is Integer && height is Integer)
                {
                    int[] size = new int[]{(Integer) width, (Integer) height};
                    parameters.setValue(WorldFile.WORLD_FILE_IMAGE_SIZE, size);
                }
            }
        }

        java.io.File[] worldFiles = WorldFile.getWorldFiles((java.io.File) source);
        WorldFile.decodeWorldFiles(worldFiles, parameters);
    }

    private static javax.imageio.stream.ImageInputStream createInputStream(Object source) throws java.io.IOException
    {
        // ImageIO can create an ImageInputStream automatically from a File references or a standard I/O InputStream
        // reference. If the data source is a URL, or a string file path, then we must open an input stream ourselves.

        Object input = source;

        if (source is java.net.URL)
        {
            input = ((java.net.URL) source).openStream();
        }
        else if (source is CharSequence)
        {
            input = openInputStream(source.ToString());
        }

        return javax.imageio.ImageIO.createImageInputStream(input);
    }

    private static java.io.InputStream openInputStream(String path) throws java.io.IOException
    {
        Object streamOrException = WWIO.getFileOrResourceAsStream(path, null);
        if (streamOrException == null)
        {
            return null;
        }
        else if (streamOrException is java.io.IOException)
        {
            throw (java.io.IOException) streamOrException;
        }
        else if (streamOrException is Exception)
        {
            String message = Logging.getMessage("generic.ExceptionAttemptingToReadImageFile", path);
            Logging.logger().log(java.util.logging.Level.SEVERE, message, streamOrException);
            throw new java.io.IOException(message);
        }

        return (java.io.InputStream) streamOrException;
    }

    private static javax.imageio.ImageReader readerFor(javax.imageio.stream.ImageInputStream iis)
    {
        java.util.Iterator<javax.imageio.ImageReader> readers = javax.imageio.ImageIO.getImageReaders(iis);
        if (!readers.hasNext())
        {
            return null;
        }

        return readers.next();
    }

    private static String[] getImageIOReaderSuffixes()
    {
        java.util.Iterator<javax.imageio.spi.ImageReaderSpi> iter;
        try
        {
            iter = javax.imageio.spi.IIORegistry.getDefaultInstance().getServiceProviders(
                    javax.imageio.spi.ImageReaderSpi.class, true);
        }
        catch (Exception e)
        {
            return new String[0];
        }

        java.util.Set<String> set = new java.util.HashSet<String>();
        while (iter.hasNext())
        {
            javax.imageio.spi.ImageReaderSpi spi = iter.next();
            String[] names = spi.getFileSuffixes();
            set.addAll(java.util.Arrays.asList(names));
        }

        String[] array = new String[set.size()];
        set.toArray(array);
        return array;
    }
}
}
