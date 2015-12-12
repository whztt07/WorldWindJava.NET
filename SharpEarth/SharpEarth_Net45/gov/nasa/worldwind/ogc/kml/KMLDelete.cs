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
 * Represents the KML <i>Delete</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLDelete.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLDelete extends AbstractXMLEventParser implements KMLUpdateOperation
{
    protected List<KMLAbstractFeature> features = new ArrayList<KMLAbstractFeature>();

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLDelete(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractFeature)
            this.addFeature((KMLAbstractFeature) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addFeature(KMLAbstractFeature o)
    {
        this.features.add(o);
    }

    public List<KMLAbstractFeature> getFeatures()
    {
        return this.features;
    }

    public void applyOperation(KMLRoot targetRoot)
    {
        foreach (KMLAbstractFeature feature  in  this.features)
        {
            String targetId = feature.getTargetId();
            if (WWUtil.isEmpty(targetId))
                continue;

            Object o = targetRoot.getItemByID(targetId);
            if (o == null || !(o is KMLAbstractFeature))
                continue;

            KMLAbstractFeature featureToDelete = (KMLAbstractFeature) o;

            Object p = featureToDelete.getParent();
            if (!(p is KMLAbstractContainer))
                continue;

            ((KMLAbstractContainer) p).removeFeature(featureToDelete);
        }
    }
}
}
