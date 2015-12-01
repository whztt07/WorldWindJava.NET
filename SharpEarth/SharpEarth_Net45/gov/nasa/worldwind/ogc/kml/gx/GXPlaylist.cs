/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml.XMLEventParserContext;
using SharpEarth.ogc.kml.KMLAbstractObject;
namespace SharpEarth.ogc.kml.gx{



/**
 * @author tag
 * @version $Id: GXPlaylist.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class GXPlaylist extends KMLAbstractObject
{
    protected List<GXAbstractTourPrimitive> tourPrimitives = new ArrayList<GXAbstractTourPrimitive>();

    public GXPlaylist(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o instanceof GXAbstractTourPrimitive)
            this.addTourPrimitive((GXAbstractTourPrimitive) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addTourPrimitive(GXAbstractTourPrimitive o)
    {
        this.tourPrimitives.add(o);
    }

    public List<GXAbstractTourPrimitive> getTourPrimitives()
    {
        return this.tourPrimitives;
    }
}
}
