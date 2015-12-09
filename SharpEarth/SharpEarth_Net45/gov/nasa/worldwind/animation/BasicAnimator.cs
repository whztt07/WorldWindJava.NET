/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.animation{

/**
 * @author jym
 * @version $Id: BasicAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */

/**
 * A base class for an interpolating <code>Animator</code>.
 */
public class BasicAnimator : Animator
{
    private bool _stopOnInvalidState = false;
    private bool _lastStateValid = true;
    private bool _hasNext = true;

    /**
     * Used to drive the animators next value based on the interpolant returned by the
     * <code>Interpolator</code>'s next interpolant
     */
    protected Interpolator interpolator;

    /**
     * Constructs a <code>BasicAnimator</code>.  Sets the <code>Animator</code>'s <code>Interpolator</code> to
     * <code>null</code>. 
     */
    public BasicAnimator()
    {
        interpolator = null;
    }

    /**
     * Constructs a <code>BasicAnimator</code>.  The <code>next</code> method will use the passed
     * <code>Interpolator</code> to retrieve the <code>interpolant</code>
     *
     * @param interpolator The <code>Interpolator</code> to be used to get the interpolant for
     * setting the next value.
     */
    public BasicAnimator(Interpolator interpolator)
    {
        this.interpolator = interpolator;
    }

    /**
     * Calls the <code>set</code> method with the next <code>interpolant</code> as determined
     * by the <code>interpolator</code> member.
     */
    public void next()
    {
        set(interpolator.nextInterpolant());
    }

    /**
     * Calls the setImpl method with the interpolant value.  Deriving classes are expected to
     * implement the desired action of a set operation in thier <code>setImpl</code> method.
     *
     * @param interpolant A value between 0 and 1.
     */
    public void set(double interpolant)
    {
        setImpl(interpolant);
        if (isStopOnInvalidState() && !isLastStateValid())
        {
            this.stop();
        }
    }

    /**
     * Returns <code>true</code> if the <code>Animator</code> has more elements.
     *
     * @return <code>true</code> if the <code>Animator</code> has more elements
     */
    public bool hasNext()
    {
        return _hasNext;
    }

    /**
     * Starts the <code>Animator</code>, <code>hasNext</code> will now return <code>true</code>
     */
    public void start()
    {
      _hasNext = true;
    }

    /**
     * Stops the <code>Animator</code>, <code>hasNext</code> will now return <code>false</code>
     */
    public void stop()
    {
      _hasNext = false;
    }

    /**
     * No-op intended to be overrided by deriving classes.  Deriving classes are expected to
     * implement the desired action of a set operation in this method.
     *
     * @param interpolant A value between 0 and 1.
     */
    protected virtual void setImpl(double interpolant)
    {

    }

    public void setStopOnInvalidState(bool stop)
    {
       _stopOnInvalidState = stop;
    }

    public bool isStopOnInvalidState()
    {
       return _stopOnInvalidState;
    }

    protected void flagLastStateInvalid()
    {
       _lastStateValid = false;
    }

    protected bool isLastStateValid()
    {
       return _lastStateValid;
    }
}
}
