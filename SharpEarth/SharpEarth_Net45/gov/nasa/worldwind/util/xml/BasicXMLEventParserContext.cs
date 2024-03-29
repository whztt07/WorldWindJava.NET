/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.avlist;

namespace SharpEarth.util.xml{



/**
 * Provides an implementation of {@link SharpEarth.util.xml.XMLEventParserContext}. This class is meant to be
 * the base class for schema-specific parsers.
 *
 * @author tag
 * @version $Id: BasicXMLEventParserContext.java 1981 2014-05-08 03:59:04Z tgaskins $
 */
public class BasicXMLEventParserContext : AVListImpl, XMLEventParserContext
{
    /** The parser name of the default double parser. */
    public static QName DOUBLE = new QName("Double");
    /** The parser name of the default integer parser. */
    public static QName INTEGER = new QName("Integer");
    /** The parser name of the default string parser. */
    public static QName STRING = new QName("String");
    /** The parser name of the default bool parser. */
    public static QName BOOLEAN = new QName("Boolean");
    /** The parser name of the default bool integer parser. */
    public static QName BOOLEAN_INTEGER = new QName("BooleanInteger");
    /** The parser name of the unrecognized-element parser. */
    public static QName UNRECOGNIZED = new QName(UNRECOGNIZED_ELEMENT_PARSER);

    protected XMLEventReader reader;
    protected StringXMLEventParser stringParser;
    protected DoubleXMLEventParser doubleParser;
    protected IntegerXMLEventParser integerParser;
    protected BooleanXMLEventParser booleanParser;
    protected BooleanIntegerXMLEventParser booleanIntegerParser;
    protected String defaultNamespaceURI = XMLConstants.NULL_NS_URI;
    protected XMLParserNotificationListener notificationListener;
    protected ConcurrentHashMap<String, Object> idTable = new ConcurrentHashMap<String, Object>();

    protected ConcurrentHashMap<QName, XMLEventParser> parsers = new ConcurrentHashMap<QName, XMLEventParser>();

    /** Construct an instance. Invokes {@link #initializeParsers()} and {@link #initialize()}. */
    public BasicXMLEventParserContext()
    {
        this.initializeParsers();
        this.initialize();
    }

    /**
     * Construct an instance for a specified event reader. Invokes {@link #initializeParsers()} and {@link
     * #initialize()}.
     *
     * @param eventReader the event reader to use for XML parsing.
     */
    public BasicXMLEventParserContext(XMLEventReader eventReader)
    {
        this.reader = eventReader;

        this.initializeParsers();
        this.initialize();
    }

    /**
     * Construct an instance for a specified event reader and default namespace. Invokes {@link #initializeParsers()}
     * and {@link #initialize()}.
     *
     * @param eventReader      the event reader to use for XML parsing.
     * @param defaultNamespace the namespace URI of the default namespace.
     */
    public BasicXMLEventParserContext(XMLEventReader eventReader, String defaultNamespace)
    {
        this.reader = eventReader;
        this.setDefaultNamespaceURI(defaultNamespace);

        this.initializeParsers();
        this.initialize();
    }

    public BasicXMLEventParserContext(BasicXMLEventParserContext ctx)
    {
        this.parsers = ctx.parsers;
        this.setDefaultNamespaceURI(ctx.getDefaultNamespaceURI());
        this.initialize();
    }

    protected void initialize()
    {
        this.initializeDefaultNotificationListener();
    }

    protected void initializeDefaultNotificationListener()
    {
        this.addPropertyChangeListener(new PropertyChangeListener()
        {
            public void propertyChange(PropertyChangeEvent propEvent)
            {
                XMLParserNotification notification = (XMLParserNotification) propEvent;

                if (notificationListener != null)
                {
                    notificationListener.notify(notification);
                    return;
                }

                String msg;
                if (notification.getEvent() != null)
                {
                    msg = Logging.getMessage(notification.getMessage(), notification.getEvent().ToString(),
                        notification.getEvent().getLocation().getLineNumber(),
                        notification.getEvent().getLocation().getColumnNumber(),
                        notification.getEvent().getLocation().getCharacterOffset());
                }
                else
                {
                    msg = Logging.getMessage(notification.getMessage(), "", "");
                }

                if (notification.getPropertyName().Equals(XMLParserNotification.EXCEPTION))
                    Logging.logger().log(Level.WARNING, msg);
                else if (notification.getPropertyName().Equals(XMLParserNotification.UNRECOGNIZED))
                    Logging.logger().log(Level.WARNING, msg);
            }
        });
    }

    /**
     * Initializes the parser table with the default parsers for the strings, integers, etc., qualified for the default
     * namespace.
     */
    protected void initializeParsers()
    {
        this.parsers.put(STRING, new StringXMLEventParser());
        this.parsers.put(DOUBLE, new DoubleXMLEventParser());
        this.parsers.put(INTEGER, new IntegerXMLEventParser());
        this.parsers.put(BOOLEAN, new BooleanXMLEventParser());
        this.parsers.put(BOOLEAN_INTEGER, new BooleanIntegerXMLEventParser());
        this.parsers.put(UNRECOGNIZED, new UnrecognizedXMLEventParser(null));
    }

    @Override
    public void addStringParsers(String namespace, String[] stringFields)
    {
        StringXMLEventParser stringParser = this.getStringParser();
        foreach (String s in stringFields)
        {
            this.parsers.put(new QName(namespace, s), stringParser);
        }
    }

    @Override
    public void addDoubleParsers(String namespace, String[] doubleFields)
    {
        DoubleXMLEventParser doubleParser = this.getDoubleParser();
        foreach (String s in doubleFields)
        {
            this.parsers.put(new QName(namespace, s), doubleParser);
        }
    }

    @Override
    public void addIntegerParsers(String namespace, String[] integerFields)
    {
        IntegerXMLEventParser integerParser = this.getIntegerParser();
        foreach (String s in integerFields)
        {
            this.parsers.put(new QName(namespace, s), integerParser);
        }
    }

    @Override
    public void addBooleanParsers(String namespace, String[] booleanFields)
    {
        BooleanXMLEventParser booleanParser = this.getBooleanParser();
        foreach (String s in booleanFields)
        {
            this.parsers.put(new QName(namespace, s), booleanParser);
        }
    }

    @Override
    public void addBooleanIntegerParsers(String namespace, String[] booleanIntegerFields)
    {
        BooleanIntegerXMLEventParser booleanIntegerParser = this.getBooleanIntegerParser();
        foreach (String s in booleanIntegerFields)
        {
            this.parsers.put(new QName(namespace, s), booleanIntegerParser);
        }
    }

    /**
     * Returns the event reader used by this instance.
     *
     * @return the instance's event reader.
     */
    public XMLEventReader getEventReader()
    {
        return this.reader;
    }

    /**
     * Specify the event reader for the parser context to use to parse XML.
     *
     * @param reader the event reader to use.
     */
    public void setEventReader(XMLEventReader reader)
    {
        if (reader == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull"); // TODO
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.reader = reader;
    }

    public String getDefaultNamespaceURI()
    {
        return defaultNamespaceURI;
    }

    public void setDefaultNamespaceURI(String defaultNamespaceURI)
    {
        this.defaultNamespaceURI = defaultNamespaceURI;
    }

    public void setNotificationListener(XMLParserNotificationListener listener)
    {
        this.notificationListener = listener;
    }

    public Map<String, Object> getIdTable()
    {
        return this.idTable;
    }

    public void addId(String id, Object o)
    {
        if (id != null)
            this.getIdTable().put(id, o);
    }

    public bool hasNext()
    {
        return this.getEventReader().hasNext();
    }

    public XMLEvent nextEvent() throws XMLStreamException
    {
        while (this.hasNext())
        {
            XMLEvent event = this.getEventReader().nextEvent();

            if (event.isCharacters() && event.asCharacters().isWhiteSpace())
                continue;

            return event;
        }

        return null;
    }

    public XMLEventParser allocate(XMLEvent event, XMLEventParser defaultParser)
    {
        return this.getParser(event, defaultParser);
    }

    public XMLEventParser allocate(XMLEvent event)
    {
        return this.getParser(event, null);
    }

    public XMLEventParser getParser(XMLEvent event)
    {
        return this.getParser(event, null);
    }

    protected XMLEventParser getParser(XMLEvent event, XMLEventParser defaultParser)
    {
        if (event == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        QName elementName = event.asStartElement().getName();
        if (elementName == null)
            return null;

        XMLEventParser parser = this.getParser(elementName);

        return parser != null ? parser : defaultParser;
    }

    public StringXMLEventParser getStringParser()
    {
        if (this.stringParser == null)
            this.stringParser = (StringXMLEventParser) this.getParser(STRING);

        return this.stringParser;
    }

    public DoubleXMLEventParser getDoubleParser()
    {
        if (this.doubleParser == null)
            this.doubleParser = (DoubleXMLEventParser) this.getParser(DOUBLE);

        return this.doubleParser;
    }

    public IntegerXMLEventParser getIntegerParser()
    {
        if (this.integerParser == null)
            this.integerParser = (IntegerXMLEventParser) this.getParser(INTEGER);

        return this.integerParser;
    }

    public BooleanXMLEventParser getBooleanParser()
    {
        if (this.booleanParser == null)
            this.booleanParser = (BooleanXMLEventParser) this.getParser(BOOLEAN);

        return this.booleanParser;
    }

    public BooleanIntegerXMLEventParser getBooleanIntegerParser()
    {
        if (this.booleanIntegerParser == null)
            this.booleanIntegerParser = (BooleanIntegerXMLEventParser) this.getParser(BOOLEAN_INTEGER);

        return this.booleanIntegerParser;
    }

    /**
     * Returns a parser to handle unrecognized elements. The default unrecognized event parser is {@link
     * SharpEarth.util.xml.UnrecognizedXMLEventParser}, and may be replaced by calling {@link
     * #registerParser(javax.xml.namespace.QName, XMLEventParser)} and specifying {@link #UNRECOGNIZED} as the parser
     * name.
     *
     * @return a parser to handle unrecognized elements.
     */
    public XMLEventParser getUnrecognizedElementParser()
    {
        return this.getParser(UNRECOGNIZED);
    }

    public String getCharacters(XMLEvent event)
    {
        if (event == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return event.isCharacters() ? event.asCharacters().getData() : null;
    }

    @SuppressWarnings({"SimplifiableIfStatement"})
    public bool isSameName(QName qa, QName qb)
    {
        if (qa.Equals(qb))
            return true;

        if (!qa.getLocalPart().Equals(qb.getLocalPart()))
            return false;

        if (qa.getNamespaceURI().Equals(XMLConstants.NULL_NS_URI))
            return qb.getNamespaceURI().Equals(this.getDefaultNamespaceURI());

        if (qb.getNamespaceURI().Equals(XMLConstants.NULL_NS_URI))
            return qa.getNamespaceURI().Equals(this.getDefaultNamespaceURI());

        return false;
    }

    @SuppressWarnings({"SimplifiableIfStatement"})
    public bool isSameAttributeName(QName qa, QName qb)
    {
        return qa != null && qb != null && qa.getLocalPart() != null && qa.getLocalPart().Equals(qb.getLocalPart());
    }

    public bool isStartElement(XMLEvent event, QName elementName)
    {
        if (event == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (elementName == null)
        {
            String message = Logging.getMessage("nullValue.ElementNameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return (event.isStartElement() && this.isSameName(event.asStartElement().getName(), elementName));
    }

    public bool isStartElement(XMLEvent event, String elementName)
    {
        if (event == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (elementName == null)
        {
            String message = Logging.getMessage("nullValue.ElementNameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return (event.isStartElement() && event.asStartElement().getName().getLocalPart().Equals(elementName));
    }

    public bool isEndElement(XMLEvent event, XMLEvent startElement)
    {
        if (event == null || startElement == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return isEndElementEvent(event, startElement);
    }

    public static bool isEndElementEvent(XMLEvent event, XMLEvent startElement)
    {
        if (event == null || startElement == null)
        {
            String message = Logging.getMessage("nullValue.EventIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return (event.isEndElement()
            && event.asEndElement().getName().Equals(startElement.asStartElement().getName()));
    }

    public void registerParser(QName elementName, XMLEventParser parser)
    {
        if (parser == null)
        {
            String message = Logging.getMessage("nullValue.ParserIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (elementName == null)
        {
            String message = Logging.getMessage("nullValue.ElementNameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.parsers.put(elementName, parser);
    }

    public XMLEventParser getParser(QName name)
    {
        if (name == null)
        {
            String message = Logging.getMessage("nullValue.ElementNameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        XMLEventParser factoryParser = this.parsers.get(name);
        if (factoryParser == null)
        {
            // Try alternate forms that assume a default namespace in either the input name or the table key.
            if (isNullNamespace(name.getNamespaceURI()))
            {
                // input name has no namespace but table key has the default namespace
                QName altName = new QName(this.getDefaultNamespaceURI(), name.getLocalPart());
                factoryParser = this.parsers.get(altName);
            }
            else if (this.isDefaultNamespace(name.getNamespaceURI()))
            {
                // input name has the default namespace but table name has no namespace
                QName altName = new QName(name.getLocalPart());
                factoryParser = this.parsers.get(altName);
            }
        }

        try
        {
            if (factoryParser == null)
                return null;

            return factoryParser.newInstance();
        }
        catch (Exception e)
        {
            String message = Logging.getMessage("XML.ParserCreationException", name);
            Logging.logger().log(java.util.logging.Level.WARNING, message, e);
            return null;
        }
    }

    protected static bool isNullNamespace(String namespaceURI)
    {
        return namespaceURI == null || XMLConstants.NULL_NS_URI.Equals(namespaceURI);
    }

    public bool isDefaultNamespace(String namespaceURI)
    {
        return this.getDefaultNamespaceURI() != null && this.getDefaultNamespaceURI().Equals(namespaceURI);
    }

    @Deprecated
    public void resolveInternalReferences(String referenceName, String fieldName, AbstractXMLEventParser parser)
    {
        if (parser == null || !parser.hasFields())
            return;

        Map<String, Object> newFields = null;

        foreach (Map.Entry<String, Object> p in parser.getFields().getEntries())
        {
            String key = p.getKey();
            if (key == null || key.Equals("id"))
                continue;

            Object v = p.getValue();
            if (v == null)
                continue;

            if (v is String)
            {
                String value = (String) v;

                if (value.startsWith("#") && key.endsWith(referenceName))
                {
                    Object o = this.getIdTable().get(value.substring(1, value.length()));
                    if (/*o is KMLStyle &&*/ !parser.hasField(fieldName))
                    {
                        if (newFields == null)
                            newFields = new HashMap<String, Object>();
                        newFields.put(fieldName, o);
                    }
                }
            }
        }

        if (newFields != null)
            parser.setFields(newFields);
    }
}
}
