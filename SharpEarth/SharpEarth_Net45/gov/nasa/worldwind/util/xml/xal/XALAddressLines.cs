/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.util.xml.xal{



/**
 * @author tag
 * @version $Id: XALAddressLines.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class XALAddressLines extends XALAbstractObject
{
    protected List<XALAddressLine> addressLines;

    public XALAddressLines(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o instanceof XALAddressLine)
            this.addAddressLine((XALAddressLine) o);
    }

    public List<XALAddressLine> getAddressLines()
    {
        return this.addressLines;
    }

    protected void addAddressLine(XALAddressLine o)
    {
        if (this.addressLines == null)
            this.addressLines = new ArrayList<XALAddressLine>();

        this.addressLines.add(o);
    }
}
}
