/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.ogc.kml.KMLAbstractObject;
using SharpEarth.geom.Position;
namespace SharpEarth.ogc.kml.gx{


/**
 * @author tag
 * @version $Id: GXLatLongQuad.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXLatLongQuad : KMLAbstractObject
{
    public GXLatLongQuad(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Position.PositionList getCoordinates()
    {
        return (Position.PositionList) this.getField("coordinates");
    }
}
}
