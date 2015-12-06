/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util;
using javax.xml.stream.events.XMLEvent;
using javax.xml.stream.XMLStreamException;
using SharpEarth.util.xml.XMLEventParserContext;
using SharpEarth.util.WWUtil;
using SharpEarth.avlist;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Update</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLUpdate.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLUpdate extends KMLAbstractObject
{
    protected List<KMLUpdateOperation> operations; // operations are performed in the order specified in the KML file
    protected bool updatesApplied;

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLUpdate(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLChange)
            this.addChange((KMLChange) o);
        else if (o is KMLCreate)
            this.addCreate((KMLCreate) o);
        else if (o is KMLDelete)
            this.addDelete((KMLDelete) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    public String getTargetHref()
    {
        return (String) this.getField("targetHref");
    }

    protected void addChange(KMLChange o)
    {
        if (this.operations == null)
            this.operations = new ArrayList<KMLUpdateOperation>();

        this.operations.add(o);
    }

    protected void addCreate(KMLCreate o)
    {
        if (this.operations == null)
            this.operations = new ArrayList<KMLUpdateOperation>();

        this.operations.add(o);
    }

    protected void addDelete(KMLDelete o)
    {
        if (this.operations == null)
            this.operations = new ArrayList<KMLUpdateOperation>();

        this.operations.add(o);
    }

    public bool isUpdatesApplied()
    {
        return updatesApplied;
    }

    public void applyOperations()
    {
        this.updatesApplied = true;

        if (WWUtil.isEmpty(this.getTargetHref()))
            return;

        if (this.operations == null || this.operations.size() == 0)
            return;

        Object o = this.getRoot().resolveReference(this.getTargetHref());

        if (o == null || !(o is KMLRoot))
            return;

        KMLRoot targetRoot = (KMLRoot) o;

        for (KMLUpdateOperation operation : this.operations)
        {
            operation.applyOperation(targetRoot);
        }
        targetRoot.firePropertyChange(AVKey.UPDATED, null, this);
    }
}
}
