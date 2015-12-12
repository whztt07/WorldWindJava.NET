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
namespace SharpEarth.ogc.wcs.wcs100{



/**
 * @author tag
 * @version $Id: WCS100Exception.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100Exception : AbstractXMLEventParser
{
    protected List<String> formats = new ArrayList<String>(1);

    public WCS100Exception(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<String> getFormats()
    {
        return this.formats;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "Format"))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.formats.add(s);
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
