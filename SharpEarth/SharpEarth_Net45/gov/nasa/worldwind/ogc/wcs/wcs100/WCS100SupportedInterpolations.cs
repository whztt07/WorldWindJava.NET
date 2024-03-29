/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.List;

using SharpEarth.util.xml.StringListXMLEventParser;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100SupportedInterpolations.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100SupportedInterpolations : StringListXMLEventParser
{
    public WCS100SupportedInterpolations(String namespaceURI)
    {
        super(namespaceURI, new QName(namespaceURI, "interpolationMethod"));
    }

    public String getDefault()
    {
        return (String) this.getField("default");
    }

    List<String> getSupportedInterpolations()
    {
        return this.getStrings();
    }
}
}
