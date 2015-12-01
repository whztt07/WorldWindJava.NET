/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.WWUtil;
namespace SharpEarth.util.xml{



/**
 * Parse a Double from an XML event.
 *
 * @author tag
 * @version $Id: DoubleXMLEventParser.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class DoubleXMLEventParser extends AbstractXMLEventParser
{
    public DoubleXMLEventParser()
    {
    }

    public DoubleXMLEventParser(String namespaceUri)
    {
        super(namespaceUri);
    }

    public Object parse(XMLEventParserContext ctx, XMLEvent doubleEvent, Object... args) throws XMLStreamException
    {
        String s = this.parseCharacterContent(ctx, doubleEvent);
        return s != null ? WWUtil.convertStringToDouble(s) : null;
    }

    public Double parseDouble(XMLEventParserContext ctx, XMLEvent doubleEvent, Object... args) throws XMLStreamException
    {
        return (Double) this.parse(ctx, doubleEvent, args);
    }
}
}
