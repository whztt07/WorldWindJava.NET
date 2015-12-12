/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.XMLStreamException;
using javax.xml.stream.events.XMLEvent;
using SharpEarth.util.xml.XMLEventParserContext;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>ExtendedData</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLExtendedData.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLExtendedData : KMLAbstractObject
{
    protected List<KMLData> data = new ArrayList<KMLData>();
    protected List<KMLSchemaData> schemaData = new ArrayList<KMLSchemaData>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLExtendedData(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLData)
            this.addData((KMLData) o);
        else if (o is KMLSchemaData)
            this.addSchemaData((KMLSchemaData) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addData(KMLData o)
    {
        this.data.add(o);
    }

    public List<KMLData> getData()
    {
        return this.data;
    }

    protected void addSchemaData(KMLSchemaData o)
    {
        this.schemaData.add(o);
    }

    public List<KMLSchemaData> getSchemaData()
    {
        return this.schemaData;
    }
}
}
