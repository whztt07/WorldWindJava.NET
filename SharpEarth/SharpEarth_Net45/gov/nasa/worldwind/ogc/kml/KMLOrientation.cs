/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.kml{

/**
 * Represents the KML <i>Orientation</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLOrientation.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLOrientation : KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLOrientation(String namespaceURI)
    {
        super(namespaceURI);
    }

    public Double getHeading()
    {
        return (Double) this.getField("heading");
    }

    public Double getTilt()
    {
        return (Double) this.getField("tilt");
    }

    public Double getRoll()
    {
        return (Double) this.getField("roll");
    }
}
}
