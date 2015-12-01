/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.List;
using javax.xml.namespace.QName;
using SharpEarth.util.xml.StringListXMLEventParser;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100SupportedFormats.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100SupportedFormats extends StringListXMLEventParser
{
    public WCS100SupportedFormats(String namespaceURI)
    {
        super(namespaceURI, new QName(namespaceURI, "formats"));
    }

    public String getNativeFormat()
    {
        return (String) this.getField("nativeFormat");
    }

    List<String> getSupportedFormats()
    {
        return this.getStrings();
    }
}
}
