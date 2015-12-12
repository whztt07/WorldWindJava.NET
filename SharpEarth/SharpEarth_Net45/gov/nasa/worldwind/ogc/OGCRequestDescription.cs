/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using javax.xml.namespace.QName;
using SharpEarth.util.xml;
using SharpEarth.util.WWUtil;
namespace SharpEarth.ogc{



/**
 * Parses an OGC Request element.
 *
 * @author tag
 * @version $Id: OGCRequestDescription.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OGCRequestDescription extends AbstractXMLEventParser
{
    protected QName FORMAT;
    protected QName DCPTYPE;

    protected String requestName;
    protected Set<String> formats;
    protected Set<OGCDCType> dcpTypes;

    public OGCRequestDescription(String namespaceURI)
    {
        super(namespaceURI);

        this.initialize();
    }

    private void initialize()
    {
        FORMAT = new QName(this.getNamespaceURI(), "Format");
        DCPTYPE = new QName(this.getNamespaceURI(), "DCPType");
    }

    @Override
    public XMLEventParser allocate(XMLEventParserContext ctx, XMLEvent event)
    {
        XMLEventParser defaultParser = null;

        if (ctx.isStartElement(event, DCPTYPE))
            defaultParser = new OGCDCType(this.getNamespaceURI());

        return ctx.allocate(event, defaultParser);
    }

    public Object parse(XMLEventParserContext ctx, XMLEvent rqstEvent, Object... args) throws XMLStreamException
    {
        if (this.formats != null)
            this.formats.clear();
        if (this.dcpTypes != null)
            this.dcpTypes.clear();

        if (rqstEvent.isStartElement())
            this.setRequestName(rqstEvent.asStartElement().getName().getLocalPart());

        return super.parse(ctx, rqstEvent, args);
    }

    @Override
    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, FORMAT))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.addFormat(s);
        }
        else if (ctx.isStartElement(event, DCPTYPE))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is OGCDCType)
                    this.addDCPType((OGCDCType) o);
            }
        }
    }

    public OGCOnlineResource getOnlineResouce(String protocol, String requestMethod)
    {
        foreach (OGCDCType dct  in  this.getDCPTypes())
        {
            OGCOnlineResource olr = dct.getOnlineResouce(protocol, requestMethod);
            if (olr != null)
                return olr;
        }

        return null;
    }

    public Set<String> getFormats()
    {
        if (this.formats != null)
            return formats;
        else
            return Collections.emptySet();
    }

    protected void setFormats(Set<String> formats)
    {
        this.formats = formats;
    }

    protected void addFormat(String format)
    {
        if (this.formats == null)
            this.formats = new HashSet<String>();

        this.formats.add(format);
    }

    protected void setDCPTypes(Set<OGCDCType> dcTypes)
    {
        this.dcpTypes = dcTypes;
    }

    public Set<OGCDCType> getDCPTypes()
    {
        if (this.dcpTypes != null)
            return dcpTypes;
        else
            return Collections.emptySet();
    }

    public void addDCPType(OGCDCType dct)
    {
        if (this.dcpTypes == null)
            this.dcpTypes = new HashSet<OGCDCType>();

        this.dcpTypes.add(dct);
    }

    public String getRequestName()
    {
        return requestName;
    }

    protected void setRequestName(String requestName)
    {
        this.requestName = requestName;
    }

    @Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (this.getRequestName() != null)
            sb.append(this.getRequestName()).append("\n");

        sb.append("\tFormats: ");
        foreach (String format  in  this.getFormats())
        {
            sb.append("\t").append(format).append(", ");
        }

        sb.append("\n\tDCPTypes:\n");
        foreach (OGCDCType dcpt  in  this.getDCPTypes())
        {
            sb.append("\t\t").append(dcpt.ToString()).append("\n");
        }

        return sb.ToString();
    }
}
}
