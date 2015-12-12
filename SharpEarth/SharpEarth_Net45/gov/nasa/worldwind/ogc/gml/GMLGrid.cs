/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util.xml.AbstractXMLEventParser;
namespace SharpEarth.ogc.gml{


/**
 * @author tag
 * @version $Id: GMLGrid.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class GMLGrid : AbstractXMLEventParser
{
    public GMLGrid(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getDimension()
    {
        return (String) this.getField("dimension");
    }

    public String getSRSName()
    {
        return (String) this.getField("srsName");
    }

    public GMLOrigin getOrigin()
    {
        return (GMLOrigin) this.getField("origin");
    }

    public GMLLimits getLimits()
    {
        return (GMLLimits) this.getField("limits");
    }
}
}
