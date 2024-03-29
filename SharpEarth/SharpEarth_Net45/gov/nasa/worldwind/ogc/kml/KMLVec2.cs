/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.events;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
using SharpEarth.util.WWUtil;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Vec2</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLVec2.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLVec2 : KMLAbstractObject
{
    protected Double x;
    protected Double y;

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLVec2(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventAttribute(Attribute attr, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if ("x".Equals(attr.getName().getLocalPart()))
            this.setX(WWUtil.makeDouble(attr.getValue()));
        else if ("y".Equals(attr.getName().getLocalPart()))
            this.setY(WWUtil.makeDouble(attr.getValue()));
        else
            super.doAddEventAttribute(attr, ctx, event, args);
    }

    protected void setX(Double o)
    {
        this.x = o;
    }

    public Double getX()
    {
        return this.x;
    }

    protected void setY(Double o)
    {
        this.y = o;
    }

    public Double getY()
    {
        return this.y;
    }

    public String getXunits()
    {
        return (String) this.getField("xunits");
    }

    public String getYunits()
    {
        return (String) this.getField("yunits");
    }
}
}
