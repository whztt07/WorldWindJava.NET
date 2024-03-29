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
 * @version $Id: WCS100Service.java 2061 2014-06-19 19:59:40Z tgaskins $
 */
public class WCS100Service : AbstractXMLEventParser
{
    protected List<String> accessConstraints = new ArrayList<String>(1);

    public WCS100Service(String namespaceURI)
    {
        super(namespaceURI);
    }

    public String getDescription()
    {
        return (String) this.getField("description");
    }

    public String getName()
    {
        return (String) this.getField("name");
    }

    public String getLabel()
    {
        return (String) this.getField("label");
    }

    public List<String> getAccessConstraints()
    {
        return this.accessConstraints;
    }

    public String getFees()
    {
        return (String) this.getField("fees");
    }

    public WCS100MetadataLink getMetadataLink()
    {
        return (WCS100MetadataLink) this.getField("metadataLink");
    }

    public List<String> getKeywords()
    {
        return ((StringListXMLEventParser) this.getField("keywords")).getStrings();
    }

    public WCS100ResponsibleParty getResponsibleParty()
    {
        return (WCS100ResponsibleParty) this.getField("responsibleParty");
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "accessConstraints"))
        {
            String s = ctx.getStringParser().parseString(ctx, event);
            if (!WWUtil.isEmpty(s))
                this.accessConstraints.add(s);
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
