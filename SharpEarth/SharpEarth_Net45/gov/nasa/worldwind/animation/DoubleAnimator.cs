/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.util;
namespace SharpEarth.animation{


/**
 * An {@link Animator} implentation for animating values of type Double.
 *
 * @author jym
 * @version $Id: DoubleAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class DoubleAnimator : BasicAnimator
{
    public double Begin { get; set; }
    public double End { get; set; }
    public PropertyAccessor.DoubleAccessor propertyAccessor { get; protected set; }

    public DoubleAnimator(Interpolator interpolator,
       double begin, double end,
       PropertyAccessor.DoubleAccessor propertyAccessor) : base(interpolator)
    {
       if (interpolator == null)
       {
           this.interpolator = new ScheduledInterpolator(10000);
       }
       
       if (propertyAccessor == null)
       {
           string message = Logging.getMessage("nullValue.ViewPropertyAccessorIsNull");
           Logging.logger().severe(message);
           throw new ArgumentException(message);
       }

       Begin= begin;
       End = end;
       this.propertyAccessor = propertyAccessor;
    }

    protected void setImpl(double? interpolant)
    {
       double newValue = nextDouble(interpolant);
       if (newValue == null)
           return;

       bool success = this.propertyAccessor.setDouble(newValue);
       if (!success)
       {
           this.flagLastStateInvalid();
       }
       if (interpolant >= 1.0)
           this.stop();
    }

    public double nextDouble(double interpolant)
    {
       return AnimationSupport.mixDouble( interpolant, Begin, End);
    }
}
}
