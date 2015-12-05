/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.render;
using SharpEarth.events.MessageListener;
namespace SharpEarth.ogc.kml.impl{


/**
 * Interface for rendering KML elements.
 *
 * @author tag
 * @version $Id: KMLRenderable.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface KMLRenderable extends MessageListener
{
    /**
     * Pre-render this element.
     *
     * @param tc the current KML traversal context.
     * @param dc the current draw context.
     *
     * @throws ArgumentException if either the traversal context or the draw context is null.
     */
    void preRender(KMLTraversalContext tc, DrawContext dc);

    /**
     * Render this element.
     *
     * @param tc the current KML traversal context.
     * @param dc the current draw context.
     *
     * @throws ArgumentException if either the traversal context or the draw context is null.
     */
    void render(KMLTraversalContext tc, DrawContext dc);
}
}
