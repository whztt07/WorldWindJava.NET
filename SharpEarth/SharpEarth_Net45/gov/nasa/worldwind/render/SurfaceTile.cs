/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.List;
using SharpEarth.geom;
namespace SharpEarth.render{



/**
 * @author tag
 * @version $Id: SurfaceTile.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface SurfaceTile
{
    bool bind(DrawContext dc);
    void applyInternalTransform(DrawContext dc, bool textureIdentityActive);
    Sector getSector();
    Extent getExtent(DrawContext dc);
    List<? extends LatLon> getCorners();
}
}
