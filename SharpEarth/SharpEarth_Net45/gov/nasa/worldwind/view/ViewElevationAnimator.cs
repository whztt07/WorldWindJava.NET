/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.util;
using SharpEarth.globes;
using SharpEarth.geom;
using SharpEarth.animation;
namespace SharpEarth.view{


/**
 * An {@link SharpEarth.animation.Animator} for elevation values.  Calculates a mid-zoom value that
 * gives the effect of flying up and them back down again.
 *
 * @author jym
 * @version $Id: ViewElevationAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class ViewElevationAnimator : DoubleAnimator
{
    protected Globe globe;
    protected LatLon endLatLon;
    protected int altitudeMode;

    protected double midZoom;
    protected bool UseMidZoom = true;
    protected double trueEndZoom;

    /**
     * Create the animator. If the altitude mode is relative to surface elevation, the ending elevation will be
     * re-calculated as the animation runs to ensure that the final elevation is based on the most accurate elevation
     * data available.
     *
     * @param globe            Globe used to evaluate altitude mode and determine if mid-zoom is necessary. May be null.
     * @param beginZoom        Beginning elevation.
     * @param endZoom          Ending elevation.
     * @param beginLatLon      Beginning location.
     * @param endLatLon        Ending location.
     * @param altitudeMode     Altitude mode of ending elevation ({@link WorldWind#CLAMP_TO_GROUND},
     *                         {@link WorldWind#RELATIVE_TO_GROUND}, or {@link WorldWind#ABSOLUTE}. Altitude mode
     *                         is not used if {@code globe} is null.
     * @param propertyAccessor Accessor to set elevation.
     */
    public ViewElevationAnimator(Globe globe, double beginZoom, double endZoom, LatLon beginLatLon,
        LatLon endLatLon, int altitudeMode, PropertyAccessor.DoubleAccessor propertyAccessor) : base( null, beginZoom, endZoom, propertyAccessor )
    {
        this.endLatLon = endLatLon;
        this.altitudeMode = altitudeMode;

        if (globe == null)
        {
          UseMidZoom = false;
        }
        else
        {
            this.globe = globe;
            this.midZoom = computeMidZoom(globe, beginLatLon, endLatLon, beginZoom, endZoom);
        UseMidZoom = useMidZoom(beginZoom, endZoom, midZoom);
        }

        if ( UseMidZoom )
        {
            this.trueEndZoom = endZoom;
            this.End = this.midZoom;
        }
    }

    /**
     * return the true position to end the elevation animation at.
     * @return the true end elevation position.
     */
    public double getTrueEndZoom()
    {
        return(trueEndZoom);
    }

    /**
     * determines whether this Animator is using midZoom.
     * The mid-point zoom is an interpolated value between minimum(the lesser of beginZoom and endZoom,
     * and maximum zoom (3* the radius of the globe).
     * @return whether this Animator is using midZoom.
     */
    public bool getUseMidZoom()
    {
        return UseMidZoom;
    }

    /**
     * Set the animator's end zoom level. Setting the end zoom does not change the mid-zoom.
     *
     * @param end New end zoom.
     */
    public void setEnd(double end)
    {
        if (this.getUseMidZoom())
            this.trueEndZoom = end;
        else
            this.End = end;
    }

    /**
     * Set the value of the field being animated based on the given interpolant.
     * @param interpolant A value between 0 and 1.
     */
    public void set(double interpolant)
    {
        const int MAX_SMOOTHING = 1;
        const double ZOOM_START = 0.0;
        const double ZOOM_STOP = 1.0;
        if (interpolant >= 1.0)
            this.stop();
        double  zoomInterpolant;

        if (this.UseMidZoom)
        {
            double value;
            zoomInterpolant = this.zoomInterpolant(interpolant, ZOOM_START, ZOOM_STOP, MAX_SMOOTHING);
            if (interpolant <= .5)
            {
                value = nextDouble(zoomInterpolant, this.Begin, this.End);
            }
            else
            {
                value = nextDouble(zoomInterpolant, this.End, this.trueEndZoom);
            }
            this.propertyAccessor.setDouble(value);
        }
        else
        {
            zoomInterpolant = AnimationSupport.basicInterpolant(interpolant, ZOOM_START, ZOOM_STOP, MAX_SMOOTHING);
            base.set(zoomInterpolant);
        }

    }

    private double zoomInterpolant(double interpolant, double startInterpolant, double stopInterpolant,
            int maxSmoothing)
    {
        // Map interpolant in to range [start, stop].
        double normalizedInterpolant = AnimationSupport.interpolantNormalized(
            interpolant, startInterpolant, stopInterpolant);

        // During first half of iteration, zoom increases from begin to mid,
        // and decreases from mid to end during second half.
        if (normalizedInterpolant <= 0.5)
        {
            normalizedInterpolant = (normalizedInterpolant * 2.0);
        }
        else
        {
            normalizedInterpolant = ((normalizedInterpolant - .5) * 2.0);
        }

        return AnimationSupport.interpolantSmoothed(normalizedInterpolant, maxSmoothing);
    }

    
    public double? nextDouble(double interpolant)
    {
        return this.nextDouble(interpolant, this.Begin, this.End);
    }

    /**
     * Computes the value for the given interpolant.
     *
     * @param interpolant the interpolant to use for interpolating
     * @param start the lower end of the interpolated range.
     * @param end the upper end of the interpolated range.
     * @return the interpolated value.
     */
    protected double nextDouble(double interpolant, double start, double end)
    {
        double elevation =  AnimationSupport.mixDouble(
           interpolant,
           start,
           end);

        // Check the altitude mode. If the altitude mode depends on the surface elevation we will reevaluate the
        // end position altitude. When the animation starts we may not have accurate elevation data available for
        // the end position, so recalculating the elevation as we go ensures that the animation will end at the
        // correct altitude.
        double endElevation = 0.0;
        bool overrideEndElevation = false;

        if (this.globe != null && this.altitudeMode == WorldWind.CLAMP_TO_GROUND)
        {
            overrideEndElevation = true;
            endElevation = this.globe.getElevation(endLatLon.getLatitude(), endLatLon.getLongitude());
        }
        else if (this.globe != null && this.altitudeMode == WorldWind.RELATIVE_TO_GROUND)
        {
            overrideEndElevation = true;
            endElevation = this.globe.getElevation(endLatLon.getLatitude(), endLatLon.getLongitude()) + end;
        }

        if (overrideEndElevation)
        {
            elevation = (1 - interpolant) * start + interpolant * endElevation;
        }

        return elevation;
    }

    protected void setImpl(double interpolant)
    {
       double? newValue = this.nextDouble(interpolant);
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


    protected static double computeMidZoom(
        Globe globe,
        LatLon beginLatLon, LatLon endLatLon,
        double beginZoom, double endZoom)
    {
        // Scale factor is angular distance over 180 degrees.
        Angle sphericalDistance = LatLon.greatCircleDistance(beginLatLon, endLatLon);
        double scaleFactor = AnimationSupport.angularRatio(sphericalDistance, Angle.POS180);

        // Mid-point zoom is interpolated value between minimum and maximum zoom.
        double MIN_ZOOM = Math.Min(beginZoom, endZoom);
        double MAX_ZOOM = 3.0 * globe.getRadius();
        return AnimationSupport.mixDouble(scaleFactor, MIN_ZOOM, MAX_ZOOM);
    }

    /**
     * Determines if the animation will use mid-zoom.  Mid-zoom animation is used if the difference between the beginZoom
     * and endZoom values is less than the difference between the midZoom value and the larger of the beginZoom
     * or endZoom values.
     * @param beginZoom the begin zoom value
     * @param endZoom the end zoom value
     * @param midZoom the elevation at the middle of the animation
     * @return true if it is appropriate to use the midZoom value.
     */
    protected bool useMidZoom(double beginZoom, double endZoom, double midZoom)
    {
        double a = Math.Abs(endZoom - beginZoom);
        double b = Math.Abs(midZoom - Math.Max(beginZoom, endZoom));
        return a < b;
    }

}
}
