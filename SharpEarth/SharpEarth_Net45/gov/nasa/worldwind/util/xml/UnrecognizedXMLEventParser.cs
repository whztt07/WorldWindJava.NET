/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.util.xml{

/**
 * Holds the content of unrecognized elements. There are no field-specific accessors because the field names are
 * unknown, but all fields can be accessed via the inherited {@link SharpEarth.util.xml.AbstractXMLEventParser#getField(javax.xml.namespace.QName)}
 * and {@link SharpEarth.util.xml.AbstractXMLEventParser#getFields()}.
 *
 * @author tag
 * @version $Id: UnrecognizedXMLEventParser.java 1981 2014-05-08 03:59:04Z tgaskins $
 */
public class UnrecognizedXMLEventParser : AbstractXMLEventParser
{
    public UnrecognizedXMLEventParser()
    {
    }

    public UnrecognizedXMLEventParser(String namespaceURI)
    {
        super(namespaceURI);
    }
}
}
