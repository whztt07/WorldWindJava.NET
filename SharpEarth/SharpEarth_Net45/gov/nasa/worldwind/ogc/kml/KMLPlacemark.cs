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
using SharpEarth.render;
using SharpEarth.ogc.kml.impl;
using SharpEarth.events.Message;
using SharpEarth.avlist;
using SharpEarth.WorldWind;
namespace SharpEarth.ogc.kml{



/**
 * Represents the KML <i>Placemark</i> element and provides access to its contents.
 *
 * @author tag
 * @version $Id: KMLPlacemark.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLPlacemark : KMLAbstractFeature
{
    protected KMLAbstractGeometry geometry;
    protected List<KMLRenderable> renderables;

    /**
     * Construct an instance.
     *
     * @param namespaceURI the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public KMLPlacemark(String namespaceURI)
    {
        super(namespaceURI);
    }

    @Override
    protected void doAddEventContent(Object o, XMLEventParserContext ctx, XMLEvent event, Object... args)
        throws XMLStreamException
    {
        if (o is KMLAbstractGeometry)
            this.setGeometry((KMLAbstractGeometry) o);
        else
            super.doAddEventContent(o, ctx, event, args);
    }

    protected void setGeometry(KMLAbstractGeometry geometry)
    {
        this.geometry = geometry;
    }

    /**
     * Returns the placemark's geometry element.
     *
     * @return the placemark's geometry element, or null if there is none.
     */
    public KMLAbstractGeometry getGeometry()
    {
        return this.geometry;
    }

    public KMLSimpleData getSimpleData() // Included for test purposes only
    {
        return (KMLSimpleData) this.getField("SimpleData");
    }

    /**
     * Returns the {@link SharpEarth.ogc.kml.impl.KMLRenderable}s of this placemark.
     *
     * @return the placemark's renderables, or null if the placemark has no renderables.
     */
    public List<KMLRenderable> getRenderables()
    {
        return this.renderables;
    }

    protected void addRenderable(KMLRenderable r)
    {
        if (r != null)
            this.getRenderables().add(r);
    }

    /**
     * Pre-renders the placemark geometry represented by this <code>KMLPlacemark</code>. This initializes the placemark
     * geometry if necessary, prior to pre-rendering.
     *
     * @param tc the current KML traversal context.
     * @param dc the current draw context.
     */
    @Override
    protected void doPreRender(KMLTraversalContext tc, DrawContext dc)
    {
        if (this.getRenderables() == null)
            this.initializeGeometry(tc, this.getGeometry());

        List<KMLRenderable> rs = this.getRenderables();
        if (rs != null)
        {
            foreach (KMLRenderable r in rs)
            {
                r.preRender(tc, dc);
            }
        }
    }

    /**
     * Renders the placemark geometry represented by this <code>KMLGroundOverlay</code>.
     *
     * @param tc the current KML traversal context.
     * @param dc the current draw context.
     */
    @Override
    protected void doRender(KMLTraversalContext tc, DrawContext dc)
    {
        // We've already initialized the placemark's renderables during the preRender pass. Render the placemark's
        // renderable list without any further preparation.

        List<KMLRenderable> rs = this.getRenderables();
        if (rs != null)
        {
            foreach (KMLRenderable r in rs)
            {
                r.render(tc, dc);
            }
        }

        // Render the feature balloon (if any)
        this.renderBalloon(tc, dc);
    }

    protected void initializeGeometry(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        if (geom == null)
            return;

        if (this.getRenderables() == null)
            this.renderables = new ArrayList<KMLRenderable>(1); // most common case is one renderable

        if (geom is KMLPoint)
            this.addRenderable(this.selectPointRenderable(tc, geom));
        else if (geom is KMLLinearRing) // since LinearRing is a subclass of LineString, this test must precede
            this.addRenderable(this.selectLinearRingRenderable(tc, geom));
        else if (geom is KMLLineString)
            this.addRenderable(this.selectLineStringRenderable(tc, geom));
        else if (geom is KMLPolygon)
            this.addRenderable(this.selectPolygonRenderable(tc, geom));
        else if (geom is KMLMultiGeometry)
        {
            List<KMLAbstractGeometry> geoms = ((KMLMultiGeometry) geom).geometries;
            if (geoms != null)
            {
                foreach (KMLAbstractGeometry g in geoms)
                {
                    this.initializeGeometry(tc, g); // recurse
                }
            }
        }
        else if (geom is KMLModel)
            this.addRenderable(this.selectModelRenderable(tc, geom));
    }

    protected KMLRenderable selectModelRenderable(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        return new KMLModelPlacemarkImpl(tc, this, geom);
    }

    protected KMLRenderable selectPointRenderable(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        KMLPoint shape = (KMLPoint) geom;

        if (shape.getCoordinates() == null)
            return null;

        return new KMLPointPlacemarkImpl(tc, this, geom);
    }

    protected KMLRenderable selectLineStringRenderable(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        KMLLineString shape = (KMLLineString) geom;

        if (shape.getCoordinates() == null)
            return null;

        return new KMLLineStringPlacemarkImpl(tc, this, geom);
    }

    protected KMLRenderable selectLinearRingRenderable(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        KMLLinearRing shape = (KMLLinearRing) geom;

        if (shape.getCoordinates() == null)
            return null;

        KMLLineStringPlacemarkImpl impl = new KMLLineStringPlacemarkImpl(tc, this, geom);
        if (impl.getAltitudeMode() == WorldWind.CLAMP_TO_GROUND) // See note in google's version of KML spec
            impl.setPathType(AVKey.GREAT_CIRCLE);

        return impl;
    }

    protected KMLRenderable selectPolygonRenderable(KMLTraversalContext tc, KMLAbstractGeometry geom)
    {
        KMLPolygon shape = (KMLPolygon) geom;

        if (shape.getOuterBoundary().getCoordinates() == null)
            return null;

        if ("clampToGround".Equals(shape.getAltitudeMode()) || !this.isValidAltitudeMode(shape.getAltitudeMode()))
            return new KMLSurfacePolygonImpl(tc, this, geom);
        else if (shape.isExtrude())
            return new KMLExtrudedPolygonImpl(tc, this, geom);
        else
            return new KMLPolygonImpl(tc, this, geom);
    }

    /**
     * Indicates whether or not an altitude mode equals one of the altitude modes defined in the KML specification.
     *
     * @param altMode Altitude mode test.
     *
     * @return True if {@code altMode} is one of "clampToGround", "relativeToGround", or "absolute".
     */
    protected bool isValidAltitudeMode(String altMode)
    {
        return "clampToGround".Equals(altMode)
            || "relativeToGround".Equals(altMode)
            || "absolute".Equals(altMode);
    }

    @Override
    public void applyChange(KMLAbstractObject sourceValues)
    {
        if (!(sourceValues is KMLPlacemark))
        {
            String message = Logging.getMessage("KML.InvalidElementType", sourceValues.GetType().Name);
            Logging.logger().warning(message);
            throw new ArgumentException(message);
        }

        super.applyChange(sourceValues);

        KMLPlacemark placemark = (KMLPlacemark) sourceValues;

        if (placemark.getGeometry() != null) // the geometry changed so nullify the cached renderables
        {
            this.setGeometry(placemark.getGeometry());
            this.renderables = null;
        }

        if (placemark.hasStyle())
        {
            Message msg = new Message(KMLAbstractObject.MSG_STYLE_CHANGED, placemark);

            if (this.renderables != null)
            {
                foreach (KMLRenderable renderable in this.renderables)
                {
                    renderable.onMessage(msg);
                }
            }
        }
    }

    @Override
    public void onChange(Message msg)
    {
        if (KMLAbstractObject.MSG_GEOMETRY_CHANGED.Equals(msg.getName()))
        {
            this.renderables = null;
        }
        else if (KMLAbstractObject.MSG_STYLE_CHANGED.Equals(msg.getName()))
        {
            foreach (KMLRenderable renderable in this.renderables)
            {
                renderable.onMessage(msg);
            }
        }

        super.onChange(msg);
    }
}
}
