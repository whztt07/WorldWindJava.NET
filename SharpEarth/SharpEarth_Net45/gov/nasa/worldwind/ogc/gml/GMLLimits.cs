/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
namespace SharpEarth.ogc.gml{



/**
 * @author tag
 * @version $Id: GMLLimits.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class GMLLimits extends AbstractXMLEventParser
{
    protected List<GMLGridEnvelope> gridEnvelopes = new ArrayList<GMLGridEnvelope>(1);

    public GMLLimits(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<GMLGridEnvelope> getGridEnvelopes()
    {
        return this.gridEnvelopes;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "GridEnvelope"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is GMLGridEnvelope)
                    this.gridEnvelopes.add((GMLGridEnvelope) o);
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
