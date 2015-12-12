/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
using SharpEarth.ogc.gml;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100SpatialDomain.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100SpatialDomain : AbstractXMLEventParser
{
    protected List<GMLEnvelope> envelopes = new ArrayList<GMLEnvelope>(1);
    protected List<GMLRectifiedGrid> rectifiedGrids = new ArrayList<GMLRectifiedGrid>(1);
    protected List<GMLGrid> grids = new ArrayList<GMLGrid>(1);

    public WCS100SpatialDomain(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<GMLEnvelope> getEnvelopes()
    {
        return this.envelopes;
    }

    public List<GMLRectifiedGrid> getRectifiedGrids()
    {
        return this.rectifiedGrids;
    }

    public List<GMLGrid> getGrids()
    {
        return this.grids;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "Envelope") || ctx.isStartElement(event, "EnvelopeWithTimePeriod"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is GMLEnvelope)
                    this.envelopes.add((GMLEnvelope) o);
            }
        }
        else if (ctx.isStartElement(event, "RectifiedGrid"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is GMLRectifiedGrid)
                    this.rectifiedGrids.add((GMLRectifiedGrid) o);
            }
        }
        else if (ctx.isStartElement(event, "Grid"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is GMLGrid)
                    this.grids.add((GMLGrid) o);
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
