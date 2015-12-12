/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util.PropertyAccessor;
using SharpEarth.animation.MoveToDoubleAnimator;
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
    {
        super(end, smoothing, propertyAccessor);
        this.orbitView = orbitView;
        this.endCenterOnSurface = endCenterOnSurface;
    }

    protected void setImpl(double interpolant)
    {
       Double newValue = this.nextDouble(interpolant);
       if (newValue == null)
           return;

       this.propertyAccessor.setDouble(newValue);
    }

    public Double nextDouble(double interpolant)
    {
        double newValue = (1 - interpolant) * propertyAccessor.getDouble() + interpolant * this.end;
        if (Math.Abs(newValue - propertyAccessor.getDouble()) < minEpsilon)
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
