/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
using SharpEarth.util.WWUtil;
using SharpEarth.ogc.gml.GMLPos;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100LonLatEnvelope.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100LonLatEnvelope : AbstractXMLEventParser
{
    List<GMLPos> positions = new ArrayList<GMLPos>(2);
    List<String> timePositions = new ArrayList<String>(2);

    public WCS100LonLatEnvelope(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getSRSName()
    {
        return (String) this.getField("srsName");
    }

    public List<GMLPos> getPositions()
    {
        return this.positions;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "pos"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is GMLPos)
                    this.positions.add((GMLPos) o);
            }
        }
        else if (ctx.isStartElement(event, "timePosition"))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.timePositions.add(s);
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
