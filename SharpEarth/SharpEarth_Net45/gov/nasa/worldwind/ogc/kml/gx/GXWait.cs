/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.kml.gx{

/**
 * @author tag
 * @version $Id: GXWait.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXWait : GXAbstractTourPrimitive
{
    public GXWait(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getDuration()
    {
        return (Double) this.getField("duration");
    }
}
}
