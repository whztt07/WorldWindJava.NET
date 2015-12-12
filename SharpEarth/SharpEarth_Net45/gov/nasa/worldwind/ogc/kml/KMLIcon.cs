/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.kml{

/**
 * Represents the KML <i>Icon</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLIcon.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLIcon : KMLLink
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLIcon(String namespaceURI)
    {
        super(namespaceURI);
    }
}
}
