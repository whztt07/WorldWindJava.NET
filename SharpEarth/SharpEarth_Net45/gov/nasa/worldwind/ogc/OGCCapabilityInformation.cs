/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events;
using javax.xml.stream.XMLStreamException;

using SharpEarth.util.xml;
namespace SharpEarth.ogc{



/**
 * Parses an OGC Capability element.
 *
 * @author tag
 * @version $Id: OGCCapabilityInformation.java 1171 2013-02-11 21:45:02Z dcollins $
 */
abstract public class OGCCapabilityInformation : AbstractXMLEventParser
{
    abstract protected bool isRequestName(XMLEventParserContext ctx, QName name);

    protected QName REQUEST;
    protected QName EXCEPTION;
    protected QName FORMAT;
    protected QName EXTENDED_CAPABILITIES;
    protected QName USER_DEFINED_SYMBOLIZATION;

    protected Set<String> exceptionFormats = new HashSet<String>();
    protected Set<OGCRequestDescription> requestDescriptions = new HashSet<OGCRequestDescription>();
    protected Map<String, String> userDefinedSymbolization;

    public OGCCapabilityInformation(String namespaceURI)
    {
        super(namespaceURI);

        this.initialize();
    }

    private void initialize()
    {
        REQUEST = new QName(this.getNamespaceURI(), "Request");
        EXCEPTION = new QName(this.getNamespaceURI(), "Exception");
        FORMAT = new QName(this.getNamespaceURI(), "Format");
        EXTENDED_CAPABILITIES = new QName(this.getNamespaceURI(), "ExtendedCapabilities");
        USER_DEFINED_SYMBOLIZATION = new QName(this.getNamespaceURI(), "UserDefinedSymbolization");
    }

    @Override
    public XMLEventParser allocate(XMLEventParserContext ctx, XMLEvent event)
    {
        XMLEventParser defaultParser = null;

        if (this.isRequestName(ctx, event.asStartElement().getName()))
            defaultParser = new OGCRequestDescription(this.getNamespaceURI());
        else if (ctx.isStartElement(event, EXCEPTION))
            defaultParser = new StringSetXMLEventParser(this.getNamespaceURI(), FORMAT);

        return ctx.allocate(event, defaultParser);
    }

    @Override
    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, EXCEPTION))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is StringSetXMLEventParser)
                    this.setExceptionFormats(((StringSetXMLEventParser) o).getStrings());
            }
        }
        else if (event.isStartElement() && this.isRequestName(ctx, event.asStartElement().getName()))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is OGCRequestDescription)
                    this.requestDescriptions.add((OGCRequestDescription) o);
            }
        }
        else if (ctx.isStartElement(event, USER_DEFINED_SYMBOLIZATION))
        {
            // Break out the parsing so that it can be overridden by subclasses.
            this.parseUserDefinedSymbolization(event);
        }
        else if (ctx.isStartElement(event, EXTENDED_CAPABILITIES))
        {
            // Break out the parsing so that it can be overridden by subclasses.
            this.parseExtendedCapabilities(ctx, event, args);
        }
    }

    protected void parseExtendedCapabilities(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        XMLEventParser parser = this.allocate(ctx, event);
        if (parser != null)
        {
            Object o = parser.parse(ctx, event, args);
            if (o != null)
                this.setExtendedCapabilities(o);
        }
    }

    @SuppressWarnings( {"UnusedDeclaration"})
    protected void setExtendedCapabilities(Object extendedCapabilities)
    {
        // Override in subclass to handle extended capabilities.
    }

    protected void parseUserDefinedSymbolization(XMLEvent event) throws XMLStreamException
    {
        Iterator iter = event.asStartElement().getAttributes();
        if (iter == null)
            return;

        while (iter.hasNext())
        {
            Attribute attr = (Attribute) iter.next();
            this.addUserDefinedSymbolization(attr.getName().getLocalPart(), attr.getValue());
        }
    }

    public Set<String> getExceptionFormats()
    {
        if (this.exceptionFormats != null)
            return exceptionFormats;
        else
            return Collections.emptySet();
    }

    protected void setExceptionFormats(Set<String> exceptionFormats)
    {
        this.exceptionFormats = exceptionFormats;
    }

    public Set<OGCRequestDescription> getRequestDescriptions()
    {
        return requestDescriptions;
    }

    protected void setRequestDescriptions(Set<OGCRequestDescription> requestDescriptions)
    {
        this.requestDescriptions = requestDescriptions;
    }

    public Map<String, String> getUserDefinedSymbolization()
    {
        if (this.userDefinedSymbolization != null)
            return userDefinedSymbolization;
        else
            return Collections.emptyMap();
    }

    protected void setUserDefinedSymbolization(Map<String, String> userDefinedSymbolization)
    {
        this.userDefinedSymbolization = userDefinedSymbolization;
    }

    protected void addUserDefinedSymbolization(String key, String value)
    {
        if (this.userDefinedSymbolization == null)
            this.userDefinedSymbolization = new HashMap<String, String>();

        this.userDefinedSymbolization.put(key, value);
    }

    @Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (String ef in this.getExceptionFormats())
        {
            sb.append("Exception format: ").append(ef).append("\n");
        }

        foreach (OGCRequestDescription rd in this.getRequestDescriptions())
        {
            sb.append(rd);
        }

        foreach (Map.Entry<String, String> uds in this.getUserDefinedSymbolization().entrySet())
        {
            sb.append(uds.getKey()).append("=").append(uds.getValue()).append("\n");
        }

        return sb.ToString();
    }
}
}
