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
 * @version $Id$
 */
public class WCS100ContentMetadata : AbstractXMLEventParser
{
    protected List<WCS100CoverageOfferingBrief> coverageOfferings = new ArrayList<WCS100CoverageOfferingBrief>(1);

    public WCS100ContentMetadata(String namespaceURI)
    {
        super(namespaceURI);
    }

    public List<WCS100CoverageOfferingBrief> getCoverageOfferings()
    {
        return this.coverageOfferings;
    }

    protected void doParseEventContent(XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (ctx.isStartElement(event, "CoverageOfferingBrief"))
        {
            XMLEventParser parser = this.allocate(ctx, event);
            if (parser != null)
            {
                Object o = parser.parse(ctx, event, args);
                if (o != null && o is WCS100CoverageOfferingBrief)
                    this.coverageOfferings.add((WCS100CoverageOfferingBrief) o);
            }
        }
        else
        {
            super.doParseEventContent(ctx, event, args);
        }
    }
}
}
