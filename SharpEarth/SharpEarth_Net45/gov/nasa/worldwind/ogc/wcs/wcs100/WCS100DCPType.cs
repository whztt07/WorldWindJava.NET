/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
namespace SharpEarth.ogc.wcs.wcs100{


/**
 * @author tag
 * @version $Id$
 */
public class WCS100DCPType : AbstractXMLEventParser
{
    public WCS100DCPType(String namespaceURI)
    {
        super(namespaceURI);
    }

    public WCS100HTTP getHTTP()
    {
        return (WCS100HTTP) this.getField("HTTP");
    }
}
}
