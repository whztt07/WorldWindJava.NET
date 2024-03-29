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
 * @version $Id: WCS100HTTP.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100HTTP : AbstractXMLEventParser
{
    protected List<AttributesOnlyXMLEventParser> gets = new ArrayList<AttributesOnlyXMLEventParser>(1);
    protected List<AttributesOnlyXMLEventParser> posts = new ArrayList<AttributesOnlyXMLEventParser>(1);

    public WCS100HTTP(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<String> getGetAddresses()
    {
        if (this.gets == null)
            return null;

        List<String> addresses = new ArrayList<String>(this.gets.size());
        foreach (AttributesOnlyXMLEventParser parser in this.gets)
        {
            if (parser != null)
            {
                AttributesOnlyXMLEventParser onlineResource =
                    (AttributesOnlyXMLEventParser) parser.getField("OnlineResource");
                if (onlineResource != null)
                    addresses.add((String) onlineResource.getField("href"));
            }
        }

        return addresses;
    }

    public List<String> getPostAddresses()
    {
        if (this.posts == null)
            return null;

        List<String> addresses = new ArrayList<String>(this.posts.size());
        foreach (AttributesOnlyXMLEventParser parser in this.posts)
        {
            if (parser != null)
            {
                AttributesOnlyXMLEventParser onlineResource =
                    (AttributesOnlyXMLEventParser) parser.getField("OnlineResource");
                if (onlineResource != null)
                    addresses.add((String) onlineResource.getField("href"));
            }
        }

        return addresses;
    }

    public String getGetAddress()
    {
        List<String> addresses = this.getGetAddresses();
        Iterator<String> iter = addresses.iterator();

        return iter.hasNext() ? iter.next() : null;
    }

    public String getPostAddress()
    {
        List<String> addresses = this.getPostAddresses();
        Iterator<String> iter = addresses.iterator();

        return iter.hasNext() ? iter.next() : null;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "Get"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is AttributesOnlyXMLEventParser)
                    this.gets.add((AttributesOnlyXMLEventParser) o);
            }
        }
        else if (ctx.isStartElement(event, "Post"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is AttributesOnlyXMLEventParser)
                    this.posts.add((AttributesOnlyXMLEventParser) o);
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
