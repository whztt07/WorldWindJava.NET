/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.events;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml.XMLEventParserContext;
using SharpEarth.util.WWUtil;
namespace SharpEarth.util.xml.atom{



/**
 * Parses the Atom Link element and provides access to it's contents.
 *
 * @author tag
 * @version $Id: AtomLink.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class AtomLink : AtomAbstractObject
{
    public AtomLink(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventAttribute(Attribute attr, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if ("length".Equals(attr.getName().getLocalPart()))
            this.setField(attr.getName(), WWUtil.makeInteger(attr.getValue()));
        else
            super.doAddEventAttribute(attr, ctx, event, args);
    }

    public String getHref()
    {
        return (String) this.getField("href");
    }

    public String getRel()
    {
        return (String) this.getField("rel");
    }

    public String getType()
    {
        return (String) this.getField("type");
    }

    public String getHreflang()
    {
        return (String) this.getField("hreflang");
    }

    public String getTitle()
    {
        return (String) this.getField("title");
    }

    public Integer getLength()
    {
        return (Integer) this.getField("length");
    }
}
}
