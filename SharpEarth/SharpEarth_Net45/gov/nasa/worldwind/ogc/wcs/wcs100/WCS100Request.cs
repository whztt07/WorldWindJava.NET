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
 * @version $Id: WCS100Request.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100Request : AbstractXMLEventParser
{
    private static final String[] rNames = new String[]
        {
            "GetCapabilities", "DescribeCoverage", "GetCoverage"
        };

    protected List<WCS100RequestDescription> requests = new ArrayList<WCS100RequestDescription>(2);

    public WCS100Request(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<WCS100RequestDescription> getRequests()
    {
        return this.requests;
    }

    public WCS100RequestDescription getRequest(String requestName)
    {
        foreach (WCS100RequestDescription description in this.requests)
        {
            if (description.getRequestName().equalsIgnoreCase(requestName))
                return description;
        }

        return null;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        String requestName = this.isRequestName(ctx, event);
        if (requestName != null)
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is WCS100RequestDescription)
                {
                    ((WCS100RequestDescription) o).setRequestName(requestName);
                    this.requests.add((WCS100RequestDescription) o);
                }
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }

    protected String isRequestName(XMLEventParserContext ctx, XMLEvent event)
    {
        foreach (String requestName in rNames)
        {
            if (ctx.isStartElement(event, requestName))
                return requestName;
        }

        return null;
    }
}
}
