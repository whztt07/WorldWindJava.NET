/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.render;
using SharpEarth.geom.Box;
namespace SharpEarth.ogc.collada.impl{


/**
 * Interface for rendering COLLADA elements.
 *
 * @author pabercrombie
 * @version $Id: ColladaRenderable.java 1696 2013-10-31 18:46:55Z tgaskins $
 */
public interface ColladaRenderable
{
    /**
     * Returns this renderable's model coordinate extent.
     *
     * @param tc The traversal context to use when determining the extent.
     * @return The model coordinate extent.
     *
     * @throws ArgumentException if either the traversal context is null.
     */
    Box getLocalExtent(ColladaTraversalContext tc);

    /**
     * Pre-Render this element.
     *
     * @param tc the current COLLADA traversal context.
     * @param dc the current draw context.
     *
     * @throws ArgumentException if either the traversal context or the draw context is null.
     */
    void preRender(ColladaTraversalContext tc, DrawContext dc);

    /**
     * Render this element.
     *
     * @param tc the current COLLADA traversal context.
     * @param dc the current draw context.
     *
     * @throws ArgumentException if either the traversal context or the draw context is null.
     */
    void render(ColladaTraversalContext tc, DrawContext dc);
}
}
