/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.java.util;
using SharpEarth.javax.xml.namespaces;
using SharpEarth.javax.xml.stream.events;
using SharpEarth.util;
using SharpEarth.util.xml;
using System.Text;

namespace SharpEarth.ogc{

/**
 * Parses an OGC OnlineResource element.
 *
 * @author tag
 * @version $Id: OGCOnlineResource.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OGCOnlineResource : AbstractXMLEventParser
{
    protected QName HREF;
    protected QName TYPE;

    protected string type;
    protected string href;

    public OGCOnlineResource(string namespaceURI)
      : base(namespaceURI)
    {
        this.initialize();
    }

    private void initialize()
    {
        HREF = new QName(WWXML.XLINK_URI, "href");
        TYPE = new QName(WWXML.XLINK_URI, "type");
    }

    protected void doParseEventAttributes(SharpEarth.util.xml.XMLEventParserContext ctx, XMLEvent @event, params object[] args)
    {
        Iterator<Attribute> iter = @event.asStartElement().getAttributes();
        if (iter == null)
            return;

        while (iter.hasNext())
        {
            Attribute attr = iter.next();
            if (ctx.isSameAttributeName(attr.getName(), HREF))
                this.setHref(attr.getValue());
            else if (ctx.isSameAttributeName(attr.getName(), TYPE))
                this.setType(attr.getValue());
        }
    }

    public string getType()
    {
        return type;
    }

    protected void setType(string type)
    {
        this.type = type;
    }

    public string getHref()
    {
        return href;
    }

    protected void setHref(string href)
    {
        this.href = href;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("href: ").Append(this.href != null ? this.href : "null");
        sb.Append(", type: ").Append(this.type != null ? this.type : "null");

        return sb.ToString();
    }
}
}
