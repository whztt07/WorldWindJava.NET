/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.ogc.kml.KMLAbstractFeature;
namespace SharpEarth.ogc.kml.gx{


/**
 * @author tag
 * @version $Id: GXTour.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXTour : KMLAbstractFeature
{
    public GXTour(String namespaceURI)
    {
        super(namespaceURI);
    }

    public GXPlaylist getPlaylist()
    {
        return (GXPlaylist) this.getField("Playlist");
    }
}
}
