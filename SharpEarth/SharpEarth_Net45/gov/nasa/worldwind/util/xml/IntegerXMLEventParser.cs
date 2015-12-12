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
 * @author tag
 * @version $Id: IntegerXMLEventParser.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class IntegerXMLEventParser : AbstractXMLEventParser
{
    public IntegerXMLEventParser()
    {
    }

    public IntegerXMLEventParser(String namespaceUri)
    {
        super(namespaceUri);
    }

    public Object parse(XMLEventParserContext ctx, XMLEvent integerEvent, Object... args) throws XMLStreamException
    {
        String s = this.parseCharacterContent(ctx, integerEvent);
        return s != null ? WWUtil.convertStringToInteger(s) : null;
    }
}
}
