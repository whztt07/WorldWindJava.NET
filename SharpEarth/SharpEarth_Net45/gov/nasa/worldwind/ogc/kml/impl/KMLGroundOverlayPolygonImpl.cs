/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.io.IOException;
using java.awt;
using SharpEarth.util;
using SharpEarth.render.Polygon;
using SharpEarth.render;
using SharpEarth.ogc.kml;
using SharpEarth.geom;
using SharpEarth.avlist;
using SharpEarth.WorldWind;
namespace SharpEarth.ogc.kml.impl{



/**
 * Implementation of {@link Polygon} to render KML <i>GroundOverlay</i>.
 *
 * @author pabercrombie
 * @version $Id: KMLGroundOverlayPolygonImpl.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class KMLGroundOverlayPolygonImpl : Polygon , KMLRenderable
{
    protected final KMLGroundOverlay parent;

    protected bool attributesResolved;

    /**
     * Create an instance.
     *
     * @param tc      the current {@link KMLTraversalContext}.
     * @param overlay the {@link SharpEarth.ogc.kml.KMLGroundOverlay} to render as a polygon.
     *
     * @throws NullPointerException     if the geomtry is null.
     * @throws ArgumentException if the parent placemark or the traversal context is null.
     */
    public KMLGroundOverlayPolygonImpl(KMLTraversalContext tc, KMLGroundOverlay overlay)
    {
        if (tc == null)
        {
            String msg = Logging.getMessage("nullValue.TraversalContextIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (overlay == null)
        {
            String msg = Logging.getMessage("nullValue.ParentIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.parent = overlay;

        String altMode = overlay.getAltitudeMode();
        if (!WWUtil.isEmpty(altMode))
        {
            if ("relativeToGround".Equals(altMode))
                this.setAltitudeMode(WorldWind.RELATIVE_TO_GROUND);
            else if ("absolute".Equals(altMode))
            {
                this.setAltitudeMode(WorldWind.ABSOLUTE);
            }
        }

        // Positions are specified either as a kml:LatLonBox or a gx:LatLonQuad
        Position.PositionList corners = overlay.getPositions();
        this.setOuterBoundary(corners.list);

        // Apply rotation if the overlay includes a LatLonBox
        KMLLatLonBox box = overlay.getLatLonBox();
        if (box != null)
        {
            this.setRotation(box.getRotation());
        }

        if (overlay.getName() != null)
            this.setValue(AVKey.DISPLAY_NAME, overlay.getName());

        if (overlay.getDescription() != null)
            this.setValue(AVKey.BALLOON_TEXT, overlay.getDescription());

        if (overlay.getSnippetText() != null)
            this.setValue(AVKey.SHORT_DESCRIPTION, overlay.getSnippetText());

        // If no image is specified, draw a filled rectangle
        if (this.parent.getIcon() == null || this.parent.getIcon().getHref() == null)
        {
            String colorStr = overlay.getColor();
            if (!WWUtil.isEmpty(colorStr))
            {
                Color color = WWUtil.decodeColorABGR(colorStr);

                ShapeAttributes attributes = new BasicShapeAttributes();
                attributes.setDrawInterior(true);
                attributes.setInteriorMaterial(new Material(color));
                this.setAttributes(attributes);
            }
        }
    }

    /** {@inheritDoc} */
    public void preRender(KMLTraversalContext tc, DrawContext dc)
    {
        // Intentionally left blank; KML polygon does nothing during the preRender phase.
    }

    protected bool mustResolveHref()
    {
        return this.getTextureImageSource() == null
            && this.parent.getIcon() != null
            && this.parent.getIcon().getHref() != null;
    }

    /** {@inheritDoc} */
    public void render(KMLTraversalContext tc, DrawContext dc)
    {
        if (this.mustResolveHref()) // resolve the href to either a local file or a remote URL
        {
            // The icon reference may be to a support file within a KMZ file, so check for that. If it's not, then just
            // let the normal Polygon code resolve the reference.
            String href = this.parent.getIcon().getHref();
            String localAddress = null;
            try
            {
                localAddress = this.parent.getRoot().getSupportFilePath(href);
            }
            catch (IOException ignored)
            {
            }

            float[] texCoords = new float[] {0, 0, 1, 0, 1, 1, 0, 1};
            this.setTextureImageSource((localAddress != null ? localAddress : href), texCoords, 4);
        }

        this.render(dc);
    }

    /** {@inheritDoc} */
    public bool isHighlighted()
    {
        return false;
    }
}
}
