/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
namespace SharpEarth.ogc.kml{


/**
 * Represents the KML <i>SimpleData</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLSimpleData.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLSimpleData : AbstractXMLEventParser
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLSimpleData(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getName()
    {
        return (String) this.getField("name");
    }
}
}
