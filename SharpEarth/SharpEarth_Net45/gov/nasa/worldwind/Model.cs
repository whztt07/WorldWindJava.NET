/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.geom;
using SharpEarth.globes;
using SharpEarth.layers;

namespace SharpEarth{


/**
 * Aggregates a globe and a set of layers. Through the globe it also indirectly includes the elevation model and the
 * surface geometry tessellator. A default model is defined in <code>worldwind.xml</code> or its application-specified
 * alternate.
 *
 * @author Tom Gaskins
 * @version $Id: Model.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface Model : WWObject
{
    /**
     * Returns the bounding sphere in Cartesian world coordinates of the model.
     *
     * @return the model's bounding sphere in Cartesian coordinates, or null if the extent cannot be determined.
     */
    Extent getExtent();

    /**
     * Indicates the globe in this model.
     *
     * @return The globe associated with this model.
     */
    Globe getGlobe();

    /**
     * Indicates the layers associated with this model.
     *
     * @return List of layers in this model.
     */
    LayerList getLayers();

    /**
     * Specifies the model's globe.
     *
     * @param globe the model's new globe. May be null, in which case the current globe will be detached from the
     *              model.
     */
    void setGlobe(Globe globe);

    /**
     * Specifies the model's layers.
     *
     * @param layers the model's new layers. May be null, in which case the current layers will be detached from the
     *               model.
     */
    void setLayers(LayerList layers);

    /**
     * Specifies whether to display as wireframe the interior geometry of the tessellated globe surface.
     *
     * @param show true causes the geometry to be shown, false, the default, does not.
     */
    void setShowWireframeInterior(bool show);

    /**
     * Specifies whether to display as wireframe the exterior geometry of the tessellated globe surface.
     *
     * @param show true causes the geometry to be shown, false, the default, does not.
     */
    void setShowWireframeExterior( bool show );

    /**
     * Indicates whether the globe surface's interior geometry is to be drawn.
     *
     * @return true if it is to be drawn, otherwise false.
     */
    bool isShowWireframeInterior();

    /**
     * Indicates whether the globe surface's exterior geometry is to be drawn.
     *
     * @return true if it is to be drawn, otherwise false.
     */
    bool isShowWireframeExterior();

    /**
     * Indicates whether the bounding volumes of the tessellated globe's surface geometry should be displayed.
     *
     * @return true if the bounding volumes are to be drawn, otherwise false.
     */
    bool isShowTessellationBoundingVolumes();

    /**
     * Specifies whether the bounding volumes of the globes tessellated surface geometry is to be drawn.
     *
     * @param showTileBoundingVolumes true if the bounding volumes should be drawn, false, the default, if not.
     */
    void setShowTessellationBoundingVolumes( bool showTileBoundingVolumes );
}
}
