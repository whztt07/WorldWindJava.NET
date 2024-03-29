/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml.XMLEventParserContext;
using SharpEarth.util;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Document</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLDocument.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLDocument : KMLAbstractContainer
{
    protected List<KMLSchema> schemas = new ArrayList<KMLSchema>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLDocument(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLSchema)
            this.addSchema((KMLSchema) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addSchema(KMLSchema o)
    {
        this.schemas.add(o);
    }

    public List<KMLSchema> getSchemas()
    {
        return this.schemas;
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLDocument))
        {
            String message = Logging.getMessage("KML.InvalidElementType", sourceValues.GetType().Name);
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        KMLDocument sourceDocument = (KMLDocument) sourceValues;

        if (sourceDocument.getSchemas() != null && sourceDocument.getSchemas().size() > 0)
            this.mergeSchemas(sourceDocument);
    }

    /**
     * Merge a list of incoming schemas with the current list. If an incoming schema has the same ID as an existing
     * one, replace the existing one, otherwise just add the incoming one.
     *
     * @param sourceDocument the incoming document.
     */
    protected void mergeSchemas(KMLDocument sourceDocument)
    {
        // Make a copy of the existing list so we can modify it as we traverse the copy.
        List<KMLSchema> schemaListCopy = new ArrayList<KMLSchema>(this.getSchemas().size());
        Collections.copy(schemaListCopy, this.getSchemas());

        foreach (KMLSchema sourceSchema in sourceDocument.getSchemas())
        {
            String id = sourceSchema.getId();
            if (!WWUtil.isEmpty(id))
            {
                foreach (KMLSchema existingSchema in schemaListCopy)
                {
                    String currentId = existingSchema.getId();
                    if (!WWUtil.isEmpty(currentId) && currentId.Equals(id))
                        this.getSchemas().remove(existingSchema);
                }
            }

            this.getSchemas().add(sourceSchema);
        }
    }
}
}
