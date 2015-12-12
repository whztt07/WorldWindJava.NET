/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.ogc.kml.KMLUpdate;
namespace SharpEarth.ogc.kml.gx{


/**
 * @author tag
 * @version $Id: GXAnimatedUpdate.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXAnimatedUpdate : GXAbstractTourPrimitive
{
    public GXAnimatedUpdate(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getDuration()
    {
        return (Double) this.getField("duration");
    }

    public KMLUpdate getUpdate()
    {
        return (KMLUpdate) this.getField("Update");
    }
}
}
