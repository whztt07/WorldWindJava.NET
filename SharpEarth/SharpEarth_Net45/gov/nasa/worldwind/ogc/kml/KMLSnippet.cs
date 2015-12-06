/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events;
using SharpEarth.util.WWUtil;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Snippet</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLSnippet.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLSnippet extends KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLSnippet(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventAttribute(Attribute attr, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if ("maxLines".Equals(attr.getName().getLocalPart()))
            this.setMaxLines(WWUtil.makeInteger(attr.getValue()));
        else
            super.doAddEventAttribute(attr, ctx, event, args);
    }

    public Integer getMaxLines()
    {
        return (Integer) this.getField("maxLines");
    }

    public void setMaxLines(Integer o)
    {
        this.setField("maxLines", o);
    }
}
}
