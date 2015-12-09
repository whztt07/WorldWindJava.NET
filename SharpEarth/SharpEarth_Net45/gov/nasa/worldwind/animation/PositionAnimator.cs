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
 * @version $Id: PositionAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PositionAnimator : BasicAnimator
{
    
    protected Position begin;
    protected Position end;
    protected readonly PropertyAccessor.PositionAccessor propertyAccessor;

    public PositionAnimator(
        Interpolator interpolator,
        Position begin,
        Position end,
        PropertyAccessor.PositionAccessor propertyAccessor) : base(interpolator)
    {
        if (interpolator == null)
        {
           this.interpolator = new ScheduledInterpolator(10000);
        }
        if (begin == null || end == null)
        {
           string message = Logging.getMessage("nullValue.PositionIsNull");
           Logging.logger().severe(message);
           throw new ArgumentException(message);
        }
        if (propertyAccessor == null)
        {
           string message = Logging.getMessage("nullValue.ViewPropertyAccessorIsNull");
           Logging.logger().severe(message);
           throw new ArgumentException(message);
        }

        this.begin = begin;
        this.end = end;
        this.propertyAccessor = propertyAccessor;
    }
    
    public void setBegin(Position begin)
    {
        this.begin = begin;
    }

    public void setEnd(Position end)
    {
        this.end = end;
    }

    public Position getBegin()
    {
        return this.begin;
    }

    public Position getEnd()
    {
        return this.end;
    }

    public PropertyAccessor.PositionAccessor getPropertyAccessor()
    {
        return this.propertyAccessor;
    }

    protected void setImpl(double interpolant)
    {
        Position newValue = this.nextPosition(interpolant);
        if (newValue == null)
           return;

        bool success = this.propertyAccessor.setPosition(newValue);
        if (!success)
        {
           this.flagLastStateInvalid();
        }
        if (interpolant >= 1)
        {
            this.stop();
        }
    }

    protected Position nextPosition(double interpolant)
    {
        return Position.interpolateGreatCircle(interpolant, this.begin, this.end);
    }
}
}
