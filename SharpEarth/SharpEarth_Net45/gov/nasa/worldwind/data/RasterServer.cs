/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.nio.ByteBuffer;
using SharpEarth.geom.Sector;
using SharpEarth.avlist.AVList;
namespace SharpEarth.data{



/**
 * @author tag
 * @version $Id: RasterServer.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface RasterServer
{
    /**
     * Composes a Raster and returns as ByteBuffer in the requested format (image or elevation)
     *
     * @param parameters Required parameters in parameters:
     *               <p/>
     *               AVKey.WIDTH - the height of the requested raster AVKey.HEIGHT - the height of the requested raster
     *               AVKey.SECTOR - a regular Geographic Sector defined by lat/lon coordinates of corners
     *
     * @return ByteBuffer of the requested file format
     */
    ByteBuffer getRasterAsByteBuffer(AVList parameters);

    /**
     * Returns a Geographic extend (coverage) of the composer
     *
     * @return returns a Geographic extend (coverage) of the composer as a Sector
     */
    public Sector getSector();
}
}
