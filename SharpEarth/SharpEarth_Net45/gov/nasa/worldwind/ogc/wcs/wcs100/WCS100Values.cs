/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100Values.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100Values : AbstractXMLEventParser
{
    protected List<WCS100SingleValue> singleValues = new ArrayList<WCS100SingleValue>(1);
    private List<WCS100Interval> intervals = new ArrayList<WCS100Interval>(1);

    public WCS100Values(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<WCS100SingleValue> getSingleValues()
    {
        return this.singleValues;
    }

    public String getDefault()
    {
        return (String) this.getField("default");
    }

    public String getType()
    {
        return (String) this.getField("type");
    }

    public String getSemantic()
    {
        return (String) this.getField("semantic");
    }

    public List<WCS100Interval> getIntervals()
    {
        return this.intervals;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "singleValue"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is WCS100SingleValue)
                    this.singleValues.add((WCS100SingleValue) o);
            }
        }
        else if (ctx.isStartElement(event, "interval"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is WCS100Interval)
                    this.intervals.add((WCS100Interval) o);
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
