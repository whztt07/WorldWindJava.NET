/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.util.xml{



/**
 * Provides services and resources used by XML @event parsers during @event reading and parsing.
 *
 * @author tag
 * @version $Id: XMLeventParserContext.java 1981 2014-05-08 03:59:04Z tgaskins $
 */
public interface XMLeventParserContext : SharpEarth.avlist.AVList
  {
    /**
     * Identifies the name of the parser handling unrecognized elements. Can be used to explicitly specify the context's
     * parser-table entry for unrecognized elements.
     */
    // TODO what are we doing about strings in the interface?
    //const string UNRECOGNIZED_ELEMENT_PARSER = "gov.nasa.worldwind.util.xml.UnknownElementParser";

    /**
     * Returns the @event reader associated with the context.
     *
     * @return the associated @event reader, or null if no reader is associated.
     */
    javax.xml.stream.events.XMLEventReader geteventReader();

    /**
     * Returns a new parser for a specified @event.
     *
     * @param @event indicates the element name for which a parser is created.
     *
     * @return the new parser, or null if no parser has been registered for the specified @event's element name.
     */
    XMLEventParser getParser(javax.xml.stream.events.XMLEvent @event);

    /**
     * Returns a new parser for a specified element name.
     *
     * @param eventName indicates the element name for which a parser is created.
     *
     * @return the new parser, or null if no parser has been registered for the specified element name.
     */
    XMLEventParser getParser(javax.xml.namespaces.QName eventName);

    /**
     * Determines whether an @event is a start @event for a specific @event type.
     *
     * @param @event       an @event identifying the @event type of interest.
     * @param elementName the @event name.
     *
     * @return true if the @event is a start @event for the named @event type.
     */
    bool isStartElement(javax.xml.stream.events.XMLEvent @event, javax.xml.namespaces.QName elementName);

    /**
     * Determines whether an @event is a start @event for a specific @event type indicated by its local name.
     *
     * @param @event       an @event identifying the @event type of interest.
     * @param elementName the local part of the @event name to match.
     *
     * @return true if the @event is a start @event for the named @event type.
     */
    bool isStartElement(javax.xml.stream.events.XMLEvent @event, string elementName);

    /**
     * Determines whether an @event is the corresponding end element for a specified start @event.
     * <p/>
     * Note: Only the @event's element name and type are compared. The method returns true if the start and end events
     * are the corresponding @event types for an element of the same name.
     *
     * @param @event        the @event of interest.
     * @param startElement the start @event associated with the potential end @event.
     *
     * @return true if the @event is the corresponding end @event to the specified start @event, otherwise false.
     */
    bool isEndElement(javax.xml.stream.events.XMLEvent @event, javax.xml.stream.events.XMLEvent startElement);

    /**
     * Returns the text associated with the @event.
     *
     * @param @event the @event of interest.
     *
     * @return the @event's characters, or null if the @event is not a character @event.
     */
    string getCharacters(javax.xml.stream.events.XMLEvent @event);

    /**
     * Returns the default parser for a simple string.
     *
     * @return a string parser.
     */
    StringXMLEventParser getStringParser();

    /**
     * Returns a parser for a simple double.
     *
     * @return a double parser.
     */
    DoubleXMLEventParser getDoubleParser();

    /**
     * Returns the default parser for a simple boolean.
     *
     * @return a bool parser.
     */
    BooleanXMLEventParser getBooleanParser();

    /**
     * Returns the default parser for a simple bool integer (0 or 1).
     *
     * @return a bool integer parser.
     */
    BooleanIntegerXMLEventParser getBooleanIntegerParser();

    /**
     * Returns the default parser for a simple  integer.
     *
     * @return an integer parser.
     */
    IntegerXMLEventParser getIntegerParser();

    /**
     * Registers a parser for a specified element name. A parser of the same type and namespace is returned when {@link
     * #getParser(javax.xml.stream.events.XMLEvent)} is called for the same element name.
     *
     * @param elementName the element name for which to return a parser.
     * @param parser      the parser to register.
     */
    void registerParser(javax.xml.namespaces.QName elementName, XMLEventParser parser);

    /**
     * Indicates whether the @event stream associated with this context contains another @event.
     *
     * @return true if the stream contains another @event, otherwise false.
     *
     * @see javax.xml.stream.XMLEventReader#hasNext()
     */
    bool hasNext();

    /**
     * Returns the next @event in the @event stream associated with this context.
     *
     * @return the next @event,
     *
     * @throws XMLStreamException if there is an error with the underlying XML.
     * @see javax.xml.stream.XMLEventReader#nextevent()
     */
    javax.xml.stream.events.XMLEvent nextevent();

    /**
     * Returns the context's default namespace URI.
     *
     * @return the context's default namespace URI.
     *
     * @see #setDefaultNamespaceURI(string)
     */
    string getDefaultNamespaceURI();

    /**
     * Specifies the context's default namespace URI. Must be called prior to initiating the parser table if this
     * context's parsers will be qualified for the default namespace.
     *
     * @param defaultNamespaceURI the default namespace URI.
     *
     * @see #getDefaultNamespaceURI()
     * @see #isSameName(javax.xml.namespace.QName, javax.xml.namespace.QName)
     */
    void setDefaultNamespaceURI(string defaultNamespaceURI);

    /**
     * Determines whether two element names are the same.
     *
     * @param qa first element name
     * @param qb second element name
     *
     * @return true if both names have the same namespace (or no namespace) and local name, or if either name has no
     *         namespace but the namespace of the other is the context's default namespace.
     */
    bool isSameName(javax.xml.namespaces.QName qa, javax.xml.namespaces.QName qb);

    /**
     * Create a parser for a specified @event's element name, if a parser for that name is registered with the context.
     *
     * @param @event         the @event whose element name identifies the parser to create.
     * @param defaultParser a parser to return if no parser is registered for the specified name. May be null.
     *
     * @return a new parser, or the specified default parser if no parser has been registered for the element name.
     */
    XMLEventParser allocate(javax.xml.stream.events.XMLEvent @event, XMLEventParser defaultParser);

    /**
     * Create a parser for a specified @event's element name, if a parser for that name is registered with the context.
     *
     * @param @event the @event whose element name identifies the parser to create.
     *
     * @return a new parser, or the specified default parser if no parser has been registered for the element name.
     */
    XMLEventParser allocate(javax.xml.stream.events.XMLEvent @event);

    /**
     * Determines whether two fully qualified attribute names are the same.
     *
     * @param qa the first attribute name.
     * @param qb the second attribute name.
     *
     * @return true if the names are the same, otherwise false.
     */
    bool isSameAttributeName(javax.xml.namespaces.QName qa, javax.xml.namespaces.QName qb);

    /**
     * Returns the table associating objects with their <i>id</i> attribute as specified in the object's KML file.
     *
     * @return the mapping table.
     */
    System.Collections.Generic.IDictionary<string, object> getIdTable();

    /**
     * Adds a mapping of an <i>id</i> attribute to its associated KML object.
     *
     * @param id the object id. If null, this method returns without creating a mapping.
     * @param o  the object to associate with the id.
     */
    void addId(string id, object o);

    /**
     * Resolves references to elements in the same KML file. Certain KML elements such as <i>styleUrl</i> may contain
     * references to other elements within the same KML file. (A leading "#" indicates a reference to an element within
     * the same file.) This method searches the elements in the file for these references and adds the object for the
     * element they refer to the referring object's field table.
     *
     * @param referenceName the element name of the elements whose references this method resolves. An example is
     *                      <i>styleUrl</i>. Resolution is performed for only elements of this name.
     * @param fieldName     the key used to identify the resolved object in a parser's field table. After this method
     *                      resolves references, the referenced object can be obtained by calling the parsers {@link
     *                      SharpEarth.util.xml.AbstractXMLeventParser#getField(javax.xml.namespace.QName)}
     *                      method with the <code>fieldName</code> specified here as the name argument.
     * @param parser        the parser whose references to resolve.
     *
     * @deprecated Reference resolution is handled by parsers specific to a certain document type. For example, {@link
     *             SharpEarth.ogc.kml.KMLRoot} handles resolution of references in KML files.
     */
    void resolveInternalReferences(string referenceName, string fieldName, AbstractXMLEventParser parser);

    /**
     * Specify the object to receive notifications, which are sent when exceptions occur during parsing and when
     * unrecognized element types are encountered. See {@link SharpEarth.util.xml.XMLParserNotification} for
     * more information.
     * <p/>
     * The parser context may have only one notification listener. That listener may be changed at any time.
     *
     * @param listener the object to receive notification events.
     */
    void setNotificationListener(XMLParserNotificationListener listener);

    /**
     * Indicates whether the specified namespace URI is the default namespace URI used by this parser context.
     *
     * @param namespaceURI the namespace URI to check.
     *
     * @return true if the specified namespace is the default namespace, otherwise false.
     */
    bool isDefaultNamespace(string namespaceURI);

    /**
     * Returns a parser to handle unrecognized elements. The default unrecognized @event parser is {@link
     * SharpEarth.util.xml.UnrecognizedXMLeventParser}.
     *
     * @return a parser to handle unrecognized elements.
     */
    XMLEventParser getUnrecognizedElementParser();

    /**
     * Add string list parsers for a list of element types and qualified for a specified namespace.
     *
     * @param namespace    the namespace URI.
     * @param stringFields the string list parser names.
     */
    void addStringParsers(string namespaces, string[] stringFields);

    /**
     * Add double parsers for a list of element types and qualified for a specified namespace.
     *
     * @param namespace    the namespace URI.
     * @param doubleFields the string parsers.
     */
    void addDoubleParsers(string namespaces, string[] doubleFields);

    /**
     * Add integer parsers for a list of element types and qualified for a specified namespace.
     *
     * @param namespace     the namespace URI.
     * @param integerFields the string parsers.
     */
    void addIntegerParsers(string namespaces, string[] integerFields);

    /**
     * Add bool parsers for a list of element types and qualified for a specified namespace.
     *
     * @param namespace     the namespace URI.
     * @param booleanFields the string parsers.
     */
    void addBooleanParsers(string namespaces, string[] booleanFields);

    /**
     * Add bool integer parsers for a list of element types and qualified for a specified namespace.
     *
     * @param namespace            the namespace URI.
     * @param booleanIntegerFields the string parser.
     */
    void addBooleanIntegerParsers(string namespaces, string[] booleanIntegerFields);
}
}
