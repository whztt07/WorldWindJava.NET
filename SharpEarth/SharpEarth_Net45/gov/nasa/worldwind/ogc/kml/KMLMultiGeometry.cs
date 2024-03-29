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
 * Represents the KML <i>MultiGeometry</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLMultiGeometry.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLMultiGeometry : KMLAbstractGeometry
{
    protected List<KMLAbstractGeometry> geometries = new ArrayList<KMLAbstractGeometry>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLMultiGeometry(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractGeometry)
            this.addGeometry((KMLAbstractGeometry) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addGeometry(KMLAbstractGeometry o)
    {
        this.geometries.add(o);
    }

    public List<KMLAbstractGeometry> getGeometries()
    {
        return this.geometries;
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLMultiGeometry))
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        KMLMultiGeometry multiGeometry = (KMLMultiGeometry) sourceValues;

        if (multiGeometry.getGeometries() != null && multiGeometry.getGeometries().size() > 0)
            this.mergeGeometries(multiGeometry);

        super.applyChange(sourceValues);
    }

    /**
     * Merge a list of incoming geometries with the current list. If an incoming geometry has the same ID as
     * an existing one, replace the existing one, otherwise just add the incoming one.
     *
     * @param sourceMultiGeometry the incoming geometries.
     */
    protected void mergeGeometries(KMLMultiGeometry sourceMultiGeometry)
    {
        // Make a copy of the existing list so we can modify it as we traverse the copy.
        List<KMLAbstractGeometry> geometriesCopy = new ArrayList<KMLAbstractGeometry>(this.getGeometries().size());
        Collections.copy(geometriesCopy, this.getGeometries());

        foreach (KMLAbstractGeometry sourceGeometry in sourceMultiGeometry.getGeometries())
        {
            String id = sourceGeometry.getId();
            if (!WWUtil.isEmpty(id))
            {
                foreach (KMLAbstractGeometry existingGeometry in geometriesCopy)
                {
                    String currentId = existingGeometry.getId();
                    if (!WWUtil.isEmpty(currentId) && currentId.Equals(id))
                    {
                        this.getGeometries().remove(existingGeometry);
                    }
                }
            }

            this.getGeometries().add(sourceGeometry);
        }
    }
}
}
