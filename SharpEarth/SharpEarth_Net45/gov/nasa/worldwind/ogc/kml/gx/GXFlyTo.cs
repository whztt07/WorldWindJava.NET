/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.ogc.kml;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.ogc.kml.gx{



/**
 * @author tag
 * @version $Id: GXFlyTo.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXFlyTo : GXAbstractTourPrimitive
{
    public GXFlyTo(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractView)
            this.setView((KMLAbstractView) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public Double getDuration()
    {
        return (Double) this.getField("duration");
    }

    public String getFlyToMode()
    {
        return (String) this.getField("flyToMode");
    }

    public KMLAbstractView getView()
    {
        return (KMLAbstractView) this.getField("AbstractView");
    }

    protected void setView(KMLAbstractView o)
    {
        this.setField("AbstractView", o);
    }
}
}
