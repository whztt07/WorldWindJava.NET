/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
namespace SharpEarth.util.xml{


/**
 * Parse a string from an XML event.
 *
 * @author tag
 * @version $Id: StringXMLEventParser.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class StringXMLEventParser : AbstractXMLEventParser
{
    public StringXMLEventParser()
    {
    }

    public StringXMLEventParser(String namespaceUri)
    {
        super(namespaceUri);
    }

    public Object parse(XMLEventParserContext ctx, XMLEvent stringEvent, Object... args) throws XMLStreamException
    {
        String s = this.parseCharacterContent(ctx, stringEvent, args);
        return s != null ? s.trim() : null;
    }

    public String parseString(XMLEventParserContext ctx, XMLEvent stringEvent, Object... args) throws XMLStreamException
    {
        return (String) this.parse(ctx, stringEvent, args);
    }
}
}
