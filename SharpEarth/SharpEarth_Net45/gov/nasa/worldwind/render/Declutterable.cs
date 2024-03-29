/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.awt.geom;
namespace SharpEarth.render{


/**
 * Indicates whether an object participates in decluttering.
 *
 * @author tag
 * @version $Id: Declutterable.java 704 2012-07-21 03:16:21Z tgaskins $
 */
public interface Declutterable : OrderedRenderable
{
    /**
     * Indicates whether this object actually participates in decluttering.
     *
     * @return true if the object participates, otherwise false.
     */
    bool isEnableDecluttering();

    Rectangle2D getBounds(DrawContext dc);
}
}
