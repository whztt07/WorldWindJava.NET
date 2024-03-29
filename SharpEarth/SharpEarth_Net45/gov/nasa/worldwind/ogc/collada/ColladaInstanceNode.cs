/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.render;
using SharpEarth.ogc.collada.impl;
using SharpEarth.geom.Box;
namespace SharpEarth.ogc.collada{


/**
 * Represents the COLLADA <i>instance_node</i> element and provides access to its contents.
 *
 * @author pabercrombie
 * @version $Id: ColladaInstanceNode.java 1696 2013-10-31 18:46:55Z tgaskins $
 */
public class ColladaInstanceNode : ColladaAbstractInstance<ColladaNode> , ColladaRenderable
{
    /**
     * Construct an instance.
     *
     * @param ns the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public ColladaInstanceNode(String ns)
    {
        super(ns);
    }

    public Box getLocalExtent(ColladaTraversalContext tc)
    {
        ColladaNode instance = this.get();

        return instance != null ? instance.getLocalExtent(tc) : null;
    }

    /** {@inheritDoc} Renders the target of the instance pointer, if the target can be resolved. */
    public void preRender(ColladaTraversalContext tc, DrawContext dc)
    {
        ColladaNode instance = this.get();
        if (instance != null)
            instance.preRender(tc, dc);
    }

    /** {@inheritDoc} Renders the target of the instance pointer, if the target can be resolved. */
    public void render(ColladaTraversalContext tc, DrawContext dc)
    {
        ColladaNode instance = this.get();
        if (instance != null)
            instance.render(tc, dc);
    }
}
}
