/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.kml.gx{

/**
 * @author tag
 * @version $Id: GXTourControl.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXTourControl : GXAbstractTourPrimitive
{
    public GXTourControl(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getPlayMode()
    {
        return (String) this.getField("playMode");
    }
}
}
