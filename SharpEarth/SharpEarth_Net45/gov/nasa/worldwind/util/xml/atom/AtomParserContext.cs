/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.concurrent.ConcurrentHashMap;
using java.util.Map;

using SharpEarth.util.xml;
namespace SharpEarth.util.xml.atom{



/**
 * @author tag
 * @version $Id: AtomParserContext.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class AtomParserContext : BasicXMLEventParserContext
{
    protected static final String[] StringFields = new String[]
        {
            "base",
            "email",
            "lang",
            "name",
            "uri",
        };

    public static Map<QName, XMLEventParser> getDefaultParsers()
    {
        ConcurrentHashMap<QName, XMLEventParser> parsers = new ConcurrentHashMap<QName, XMLEventParser>();

        String ans = AtomConstants.ATOM_NAMESPACE;
        parsers.put(new QName(ans, "author"), new AtomPerson(ans));
        parsers.put(new QName(ans, "link"), new AtomLink(ans));

        StringXMLEventParser stringParser = new StringXMLEventParser();
        foreach (String s in StringFields)
        {
            parsers.put(new QName(ans, s), stringParser);
        }

        return parsers;
    }
}
}
