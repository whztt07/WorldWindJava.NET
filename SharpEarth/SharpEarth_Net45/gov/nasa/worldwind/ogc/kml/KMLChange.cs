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
 * Represents the KML <i>Change</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLChange.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLChange : AbstractXMLEventParser , KMLUpdateOperation
{
    protected List<KMLAbstractObject> objects = new ArrayList<KMLAbstractObject>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLChange(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractObject)
            this.addObject((KMLAbstractObject) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addObject(KMLAbstractObject o)
    {
        this.objects.add(o);
    }

    public List<KMLAbstractObject> getObjects()
    {
        return this.objects;
    }

    public void applyOperation(KMLRoot targetRoot)
    {
        for (KMLAbstractObject sourceValues : this.objects)
        {
            String targetId = sourceValues.getTargetId();
            if (WWUtil.isEmpty(targetId))
                continue;

            Object o = targetRoot.getItemByID(targetId);
            if (o == null || !(o is KMLAbstractObject))
                continue;

            KMLAbstractObject objectToChange = (KMLAbstractObject) o;

            objectToChange.applyChange(sourceValues);
        }
    }
}
}
