/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using java.util.Arrays;
using SharpEarth.util;
namespace SharpEarth.animation{



/**
 * A group of two or more {@link Animator}s.  Can be used to animate more than one value at a time, driven by a
 * single {@link Interpolator}.
 *
 * @author jym
 * @version $Id: CompoundAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class CompoundAnimator : BasicAnimator
{
    protected Animator[] animators;

    /**
     * Construct a CompoundAnimator with the given {@link Interpolator}
     * @param interpolator the {@link Interpolator} to use to drive the animation.
     */
    public CompoundAnimator(Interpolator interpolator) : base(interpolator)
    {
        animators = null;
    }

    /**
     * Construct a CompoundAnimator with the given {@link Interpolator}, and the given {@link Animator}s.
     *
     * @param interpolator The {@link Interpolator} to use to drive the {@link Animator}s
     * @param animators The {@link Animator}s that will be driven by this {@link CompoundAnimator}
     */
    public CompoundAnimator(Interpolator interpolator, params Animator[] animators) : base(interpolator)
    {
    if (animators == null)
        {
            string message = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
      int numAnimators = animators.Length;
      this.animators = animators;
    }

    /**
     * Set the {@link Animator}s to be driven by this {@link CompoundAnimator}
     * @param animators the {@link Animator}s to be driven by this {@link CompoundAnimator}
     */
    public void setAnimators( params Animator[] animators )
    {
      this.animators = animators;
    }

    /**
     * Get an {@link Iterable} list of the {@link Animator}
     * @return the list of {@link Animator}s
     */
    public IEnumerable<Animator> getAnimators()
    {
      return animators;
    }

    /**
     * Set the values attached to each of the {@link Animator}s using the given interpolant.
     *
     * @param interpolant A value between 0 and 1.
     */
    protected void setImpl(double interpolant)
    {
        bool allStopped = true;
        foreach ( Animator a in animators.Where( a => a != null ).Where( a => a.hasNext() ) )
        {
          allStopped = false;
          a.set(interpolant);
        }
        if (allStopped)
        {
            this.stop();
        }
    }
}
}
