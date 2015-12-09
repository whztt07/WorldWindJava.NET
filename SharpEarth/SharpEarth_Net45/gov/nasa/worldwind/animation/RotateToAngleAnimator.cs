/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.util;
using SharpEarth.geom;
namespace SharpEarth.animation{


/**
 * @author jym
 * @version $Id: RotateToAngleAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class RotateToAngleAnimator : AngleAnimator
{
    private double minEpsilon = 1e-4;
    private double smoothing = .9
      ;
    public RotateToAngleAnimator(
       Angle begin, Angle end, double smoothing,
       PropertyAccessor.AngleAccessor propertyAccessor) : base(null, begin, end, propertyAccessor)
    {
        this.smoothing = smoothing;
    }

    public void next()
    {
        if (hasNext())
            set(1.0-smoothing);
    }

    protected void setImpl(double interpolant)
    {
        Angle newValue = this.nextAngle(interpolant);
        if (newValue == null)
           return;
        bool success = this.propertyAccessor.setAngle(newValue);
        if (!success)
        {
           flagLastStateInvalid();
        }
        if (interpolant >= 1)
            this.stop();
    }

    public Angle nextAngle(double interpolant)
    {


        Angle nextAngle = this.end;
        Angle curAngle = this.propertyAccessor.getAngle();

        double difference = Math.Abs(nextAngle.subtract(curAngle).degrees);
        bool stopMoving = difference < this.minEpsilon;

        if (stopMoving)
        {
            this.stop();
        }
        else
        {
            nextAngle = Angle.mix(interpolant, curAngle, this.end);
        }
        return(nextAngle);
    }
}
}
