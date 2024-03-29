/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.geom.Sector;
namespace SharpEarth.data{


/**
 * @author dcollins
 * @version $Id: MipMappedBufferedImageRaster.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class MipMappedBufferedImageRaster : BufferedImageRaster
{
    protected BufferedImageRaster[] levelRasters;

    /**
     * Creates a mipmapped version of a BufferedImageRaster from a single BufferedImage instance.
     *
     * @param sector A sector
     * @param image  BufferedImage
     */
    public MipMappedBufferedImageRaster(Sector sector, java.awt.image.BufferedImage image)
    {
        base(sector, image);

        int maxLevel = ImageUtil.getMaxMipmapLevel(image.getWidth(), image.getHeight());
        java.awt.image.BufferedImage[] levelImages = ImageUtil.buildMipmaps(image,
            java.awt.image.BufferedImage.TYPE_INT_ARGB_PRE, maxLevel);

        this.levelRasters = new BufferedImageRaster[1 + maxLevel];
        for (int i = 0; i <= maxLevel; i++)
        {
            this.levelRasters[i] = new BufferedImageRaster(sector, levelImages[i]);
        }
    }

    /**
     * Creates a mipmapped version of a BufferedImageRaster from multi-resolution array od BufferedImage instances.
     *
     * @param sector A sector
     * @param images An array of BufferedImages
     */
    public MipMappedBufferedImageRaster(Sector sector, java.awt.image.BufferedImage[] images)
    {
        base(sector, (null != images && images.length > 0) ? images[0] : null);

        if (null == sector)
        {
            String message = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (null == images || images.length == 0)
        {
            String message = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.levelRasters = new BufferedImageRaster[images.length];
        for (int i = 0; i < images.length; i++)
        {
            this.levelRasters[i] = new BufferedImageRaster(sector, images[i]);
        }
    }

    public long getSizeInBytes()
    {
        long sizeInBytes = 0L;
        foreach (BufferedImageRaster raster in this.levelRasters)
        {
            sizeInBytes += raster.getSizeInBytes();
        }

        return sizeInBytes;
    }

    public void dispose()
    {
        foreach (BufferedImageRaster raster in this.levelRasters)
        {
            raster.dispose();
        }
    }

    protected void doDrawOnTo(BufferedImageRaster canvas)
    {
        if (!this.getSector().intersects(canvas.getSector()))
        {
            return;
        }

        BufferedImageRaster raster = this.chooseRasterForCanvas(canvas);
        raster.doDrawOnTo(canvas);
    }

    protected BufferedImageRaster chooseRasterForCanvas(BufferedImageRaster canvas)
    {
        int level = this.computeMipmapLevel(
            this.getWidth(), this.getHeight(), this.getSector(),
            canvas.getWidth(), canvas.getHeight(), canvas.getSector());

        int maxLevel = this.levelRasters.length - 1;
        level = (int) WWMath.clamp(level, 0, maxLevel);

        return this.levelRasters[level];
    }

    protected int computeMipmapLevel(int sourceWidth, int sourceHeight, Sector sourceSector,
        int destWidth, int destHeight, Sector destSector)
    {
        double sy = ((double) sourceHeight / (double) destHeight)
            * (destSector.getDeltaLatDegrees() / sourceSector.getDeltaLatDegrees());
        double sx = ((double) sourceWidth / (double) destWidth)
            * (destSector.getDeltaLonDegrees() / sourceSector.getDeltaLonDegrees());
        double scale = Math.Max(sx, sy);

        if (scale < 1)
        {
            return 0;
        }

        return (int) WWMath.LogBase2(scale);
    }
}
}
