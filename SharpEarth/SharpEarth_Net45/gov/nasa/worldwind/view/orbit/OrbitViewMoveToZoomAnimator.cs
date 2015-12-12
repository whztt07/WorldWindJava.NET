/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.animation;
using SharpEarth.util;
using SharpEarth.animation;
using SharpEarth.util;

namespace SharpEarth.view.orbit{


/**
 * @author jym
 * @version $Id: OrbitViewMoveToZoomAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OrbitViewMoveToZoomAnimator  : MoveToDoubleAnimator
{
    BasicOrbitView orbitView;
    bool endCenterOnSurface;

    OrbitViewMoveToZoomAnimator(BasicOrbitView orbitView, Double end, double smoothing,
        PropertyAccessor.DoubleAccessor propertyAccessor, bool endCenterOnSurface)
      : base( end, smoothing, propertyAccessor )
    {
        this.orbitView = orbitView;
        this.endCenterOnSurface = endCenterOnSurface;
    }

    protected void setImpl(double interpolant)
    {
       double? newValue = this.nextDouble(interpolant);
       if (newValue == null)
           return;

       this.propertyAccessor.setDouble(newValue);
    }

    public double? nextDouble(double interpolant)
    {
        double newValue = (1 - interpolant) * propertyAccessor.getDouble().Value + interpolant * this.End;
        if (Math.Abs(newValue - propertyAccessor.getDouble().Value) < minEpsilon)
        {
            this.stop();
            if (this.endCenterOnSurface)
                orbitView.setViewOutOfFocus(true);
            return(null);
        }
        return newValue;
    }
}
}
