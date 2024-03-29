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
 * Represents the KML <i>Polygon</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLPolygon.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLPolygon : KMLAbstractGeometry
{
    protected List<KMLLinearRing> innerBoundaries;

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLPolygon(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLBoundary && event.asStartElement().getName().getLocalPart().Equals("innerBoundaryIs"))
            this.addInnerBoundary(((KMLBoundary) o).getLinearRing());
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void addInnerBoundary(KMLLinearRing o)
    {
        if (this.innerBoundaries == null)
            this.innerBoundaries = new ArrayList<KMLLinearRing>();

        this.innerBoundaries.add(o);
    }

    public bool isExtrude()
    {
        return this.getExtrude() == Boolean.TRUE;
    }

    public Boolean getExtrude()
    {
        return (Boolean) this.getField("extrude");
    }

    public Boolean getTessellate()
    {
        return (Boolean) this.getField("tessellate");
    }

    public String getAltitudeMode()
    {
        return (String) this.getField("altitudeMode");
    }

    public Iterable<? extends KMLLinearRing> getInnerBoundaries()
    {
        return this.innerBoundaries;
    }

    public KMLLinearRing getOuterBoundary()
    {
        Object o = this.getField("outerBoundaryIs");
        return o != null ? ((KMLBoundary) o).getLinearRing() : null;
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLPolygon))
        {
            String message = Logging.getMessage("nullValue.SourceIsNull");
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        KMLPolygon sourcePolygon = (KMLPolygon) sourceValues;

        if (sourcePolygon.getInnerBoundaries() != null)
            this.innerBoundaries = sourcePolygon.innerBoundaries;

        super.applyChange(sourceValues);
    }
}
}
