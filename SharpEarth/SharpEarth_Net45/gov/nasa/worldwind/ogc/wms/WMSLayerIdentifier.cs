/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.Iterator;
using javax.xml.stream.events;
using SharpEarth.util.xml;
namespace SharpEarth.ogc.wms{



/**
 * Parses a WMS layer Identifier element.
 *
 * @author tag
 * @version $Id: WMSLayerIdentifier.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class WMSLayerIdentifier : AbstractXMLEventParser
{
    protected String identifier;
    protected String authority;

    public WMSLayerIdentifier(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doParseEventAttributes(XMLEventParserContext ctx, XMLEvent event, Object... args)
    {
        Iterator iter = event.asStartElement().getAttributes();
        if (iter == null)
            return;

        while (iter.hasNext())
        {
            Attribute attr = (Attribute) iter.next();
            if (attr.getName().getLocalPart().Equals("authority") && attr.getValue() != null)
                this.setAuthority(attr.getValue());
        }
    }

    public String getIdentifier()
    {
        return this.getCharacters();
    }

    public String getAuthority()
    {
        return authority;
    }

    protected void setAuthority(String authority)
    {
        this.authority = authority;
    }
}
}
