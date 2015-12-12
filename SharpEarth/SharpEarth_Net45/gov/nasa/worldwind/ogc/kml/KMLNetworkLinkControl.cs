/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>NetworkLinkControl</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLNetworkLinkControl.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLNetworkLinkControl : AbstractXMLEventParser
{
    public KMLNetworkLinkControl(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractView)
            this.setField("AbstractView", o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public Double getMinRefreshPeriod()
    {
        return (Double) this.getField("minRefreshPeriod");
    }

    public Double getMaxSessionLength()
    {
        return (Double) this.getField("maxSessionLength");
    }

    public String getCookie()
    {
        return (String) this.getField("cookie");
    }

    public String getMessage()
    {
        return (String) this.getField("message");
    }

    public String getLinkName()
    {
        return (String) this.getField("linkName");
    }

    public String getLinkDescription()
    {
        return (String) this.getField("linkDescription");
    }

    public KMLSnippet getLinkSnippet()
    {
        return (KMLSnippet) this.getField("linkSnippet");
    }

    public String getExpires()
    {
        return (String) this.getField("expires");
    }

    public KMLUpdate getUpdate()
    {
        return (KMLUpdate) this.getField("Update");
    }

    public KMLAbstractView getView()
    {
        return (KMLAbstractView) this.getField("AbstractView");
    }
}
}
