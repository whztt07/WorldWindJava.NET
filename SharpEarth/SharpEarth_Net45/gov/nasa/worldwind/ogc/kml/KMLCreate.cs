/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml;
using SharpEarth.util.WWUtil;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Create</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLCreate.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLCreate : AbstractXMLEventParser , KMLUpdateOperation
{
    protected List<KMLAbstractContainer> containers = new ArrayList<KMLAbstractContainer>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLCreate(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractContainer)
            this.addContainer((KMLAbstractContainer) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addContainer(KMLAbstractContainer o)
    {
        this.containers.add(o);
    }

    public List<KMLAbstractContainer> getContainers()
    {
        return this.containers;
    }

    public void applyOperation(KMLRoot targetRoot)
    {
        foreach (KMLAbstractContainer container in this.containers)
        {
            String targetId = container.getTargetId();
            if (WWUtil.isEmpty(targetId))
                continue;

            Object o = targetRoot.getItemByID(targetId);
            if (o == null || !(o is KMLAbstractContainer))
                continue;

            KMLAbstractContainer receivingContainer = (KMLAbstractContainer) o;

            foreach (KMLAbstractFeature feature in container.getFeatures())
            {
                receivingContainer.addFeature(feature);
            }
        }
    }
}
}
