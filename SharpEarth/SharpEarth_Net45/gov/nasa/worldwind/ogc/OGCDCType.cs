/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;

using SharpEarth.util.xml;
namespace SharpEarth.ogc{



/**
 * Parses an OGC DCPType element.
 *
 * @author tag
 * @version $Id: OGCDCType.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OGCDCType : AbstractXMLEventParser
{
    protected QName GET;
    protected QName POST;
    protected QName HTTP;
    protected QName ONLINE_RESOURCE;

    public static class DCPInfo
    {
        protected String protocol;
        protected String method;
        protected OGCOnlineResource onlineResource;

        public DCPInfo(String protocol)
        {
            this.protocol = protocol;
        }
    }

    protected List<DCPInfo> protocols = new ArrayList<DCPInfo>(1);

    public OGCDCType(String namespaceURI)
    {
        super(namespaceURI);

        this.initialize();
    }

    @Override
    public XMLEventParser allocate(XMLEventParserContext ctx, XMLEvent event)
    {
        XMLEventParser defaultParser = null;

        if (ctx.isStartElement(event, ONLINE_RESOURCE))
            defaultParser = new OGCOnlineResource(this.getNamespaceURI());

        return ctx.allocate(event, defaultParser);
    }

    private void initialize()
    {
        GET = new QName(this.getNamespaceURI(), "Get");
        POST = new QName(this.getNamespaceURI(), "Post");
        HTTP = new QName(this.getNamespaceURI(), "HTTP");
        ONLINE_RESOURCE = new QName(this.getNamespaceURI(), "OnlineResource");
    }

    @Override
    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, HTTP))
        {
            this.addProtocol(event.asStartElement().getName().getLocalPart());
        }
        else if (ctx.isStartElement(event, GET) || ctx.isStartElement(event, POST))
        {
            this.addRequestMethod(event.asStartElement().getName().getLocalPart());
        }
        else if (ctx.isStartElement(event, ONLINE_RESOURCE))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is OGCOnlineResource)
                    this.addOnlineResource((OGCOnlineResource) o);
            }
        }
    }

    public List<DCPInfo> getDCPInfos()
    {
        return this.protocols;
    }

    protected void addProtocol(String protocol)
    {
        this.protocols.add(new DCPInfo(protocol));
    }

    protected void addRequestMethod(String requestMethod)
    {
        DCPInfo dcpi = this.protocols.get(this.protocols.size() - 1);

        if (dcpi.method != null)
        {
            dcpi = new DCPInfo(dcpi.protocol);
            this.protocols.add(dcpi);
        }

        dcpi.method = requestMethod;
    }

    protected void addOnlineResource(OGCOnlineResource onlineResource)
    {
        DCPInfo dcpi = this.protocols.get(this.protocols.size() - 1);

        dcpi.onlineResource = onlineResource;
    }

    public OGCOnlineResource getOnlineResouce(String protocol, String requestMethod)
    {
        foreach (DCPInfo dcpi in this.getDCPInfos())
        {
            if (!dcpi.protocol.equalsIgnoreCase(protocol))
                continue;

            if (dcpi.method.equalsIgnoreCase(requestMethod))
                return dcpi.onlineResource;
        }

        return null;
    }

    @Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (DCPInfo dcpi in this.getDCPInfos())
        {
            sb.append(dcpi.protocol).append(", ");
            sb.append(dcpi.method).append(", ");
            sb.append(dcpi.onlineResource.ToString());
        }

        return sb.ToString();
    }
}
}
