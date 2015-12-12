/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.geom.Position;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>LineString</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLLineString.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLLineString : KMLAbstractGeometry
{
    public KMLLineString(String namespaceURI)
    {
        super(namespaceURI);
    }

    public bool isExtrude()
    {
        return this.getExtrude() == Boolean.TRUE;
    }

    public Boolean getExtrude()
    {
        return (Boolean) this.getField("extrude");
    }

    public bool isTessellate()
    {
        return this.getTessellate() == Boolean.TRUE;
    }

    public Boolean getTessellate()
    {
        return (Boolean) this.getField("tessellate");
    }

    public String getAltitudeMode()
    {
        return (String) this.getField("altitudeMode");
    }

    public Position.PositionList getCoordinates()
    {
        return (Position.PositionList) this.getField("coordinates");
    }
}
}
