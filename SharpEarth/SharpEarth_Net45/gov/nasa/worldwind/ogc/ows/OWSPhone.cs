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
namespace SharpEarth.ogc.ows{



/**
 * @author tag
 * @version $Id: OWSPhone.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class OWSPhone : AbstractXMLEventParser
{
    protected List<String> voices = new ArrayList<String>(1);
    protected List<String> faxes = new ArrayList<String>(1);

    public OWSPhone(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<String> getVoices()
    {
        return this.voices;
    }

    public List<String> getFacsimiles()
    {
        return this.faxes;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "Voice") || ctx.isStartElement(event, "voice"))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.voices.add(s);
        }
        else if (ctx.isStartElement(event, "Facsimile") || ctx.isStartElement(event, "facsimile"))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.faxes.add(s);
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
