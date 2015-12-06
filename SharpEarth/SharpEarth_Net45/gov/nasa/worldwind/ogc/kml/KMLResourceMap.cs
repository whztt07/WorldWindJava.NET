/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>ResourceMap</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLResourceMap.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLResourceMap extends KMLAbstractObject
{
    protected List<KMLAlias> aliases = new ArrayList<KMLAlias>();

    public KMLResourceMap(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAlias)
            this.addAlias((KMLAlias) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addAlias(KMLAlias o)
    {
        this.aliases.add(o);
    }

    public List<KMLAlias> getAliases()
    {
        return this.aliases;
    }
}
}
