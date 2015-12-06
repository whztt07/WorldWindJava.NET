/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Pair</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLPair.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLPair extends KMLAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLPair(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractStyleSelector)
            this.setStyleSelector((KMLAbstractStyleSelector) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public String getKey()
    {
        return (String) this.getField("key");
    }

    public KMLStyleUrl getStyleUrl()
    {
        return (KMLStyleUrl) this.getField("styleUrl");
    }

    public KMLAbstractStyleSelector getStyleSelector()
    {
        return (KMLAbstractStyleSelector) this.getField("StyleSelector");
    }

    protected void setStyleSelector(KMLAbstractStyleSelector o)
    {
        this.setField("StyleSelector", o);
    }
}
}
