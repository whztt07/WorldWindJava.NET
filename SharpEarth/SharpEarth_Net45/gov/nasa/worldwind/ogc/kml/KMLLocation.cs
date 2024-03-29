/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.geom.Position;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>Location</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLLocation.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLLocation : KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLLocation(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getLongitude()
    {
        return (Double) this.getField("longitude");
    }

    public Double getLatitude()
    {
        return (Double) this.getField("latitude");
    }

    public Double getAltitude()
    {
        return (Double) this.getField("altitude");
    }

    /**
     * Retrieves this location as a {@link Position}. Fields that are not set are treated as zero.
     *
     * @return Position object representing this location.
     */
    public Position getPosition()
    {
        Double lat = this.getLatitude();
        Double lon = this.getLongitude();
        Double alt = this.getAltitude();

        return Position.fromDegrees(
            lat != null ? lat : 0,
            lon != null ? lon : 0,
            alt != null ? alt : 0);
    }
}
}
