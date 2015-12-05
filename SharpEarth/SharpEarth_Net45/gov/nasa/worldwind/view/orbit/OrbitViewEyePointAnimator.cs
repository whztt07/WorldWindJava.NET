/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.globes;
using SharpEarth.geom;
using SharpEarth.avlist;
using SharpEarth.animation.Animator;
namespace SharpEarth.view.orbit{


/**
 * @author dcollins
 * @version $Id: OrbitViewEyePointAnimator.java 2204 2014-08-07 23:35:03Z dcollins $
 */
public class OrbitViewEyePointAnimator implements Animator
{
    protected static final double STOP_DISTANCE = 0.1;

    protected Globe globe;
    protected BasicOrbitView view;
    protected Vec4 eyePoint;
    protected double smoothing;
    protected bool hasNext;

    public OrbitViewEyePointAnimator(Globe globe, BasicOrbitView view, Vec4 eyePoint, double smoothing)
    {
        if (globe == null)
        {
            String msg = Logging.getMessage("nullValue.GlobeIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (view == null)
        {
            String msg = Logging.getMessage("nullValue.ViewIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (eyePoint == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.globe = globe;
        this.view = view;
        this.eyePoint = eyePoint;
        this.smoothing = smoothing;
        this.hasNext = true;
    }

    public void setEyePoint(Vec4 eyePoint)
    {
        this.eyePoint = eyePoint;
    }

    @Override
    public void start()
    {
        this.hasNext = true;
    }

    @Override
    public void stop()
    {
        this.hasNext = false;
    }

    @Override
    public bool hasNext()
    {
        return this.hasNext;
    }

    @Override
    public void set(double interpolant)
    {
        // Intentionally left blank.
    }

    @Override
    public void next()
    {
        Matrix modelview = this.view.getModelviewMatrix();
        Vec4 point = modelview.extractEyePoint();

        if (point.distanceTo3(this.eyePoint) > STOP_DISTANCE)
        {
            point = Vec4.mix3(1 - this.smoothing, point, this.eyePoint);
            setEyePoint(this.globe, this.view, point);
        }
        else
        {
            setEyePoint(this.globe, this.view, this.eyePoint);
            this.stop();
        }
    }

    public static void setEyePoint(Globe globe, BasicOrbitView view, Vec4 newEyePoint)
    {
        if (globe == null)
        {
            String msg = Logging.getMessage("nullValue.GlobeIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (view == null)
        {
            String msg = Logging.getMessage("nullValue.ViewIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (newEyePoint == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        // Translate the view's modelview matrix to the specified new eye point, and compute the new center point by
        // assuming that the view's zoom distance does not change.
        Vec4 translation = view.getModelviewMatrix().extractEyePoint().subtract3(newEyePoint);
        Matrix modelview = view.getModelviewMatrix().multiply(Matrix.fromTranslation(translation));
        Vec4 eyePoint = modelview.extractEyePoint();
        Vec4 forward = modelview.extractForwardVector();
        Vec4 centerPoint = eyePoint.add3(forward.multiply3(view.getZoom()));

        // Set the view's properties from the new modelview matrix.
        AVList parameters = modelview.extractViewingParameters(centerPoint, view.getRoll(), globe);
        view.setCenterPosition((Position) parameters.getValue(AVKey.ORIGIN));
        view.setHeading((Angle) parameters.getValue(AVKey.HEADING));
        view.setPitch((Angle) parameters.getValue(AVKey.TILT));
        view.setRoll((Angle) parameters.getValue(AVKey.ROLL));
        view.setZoom((Double) parameters.getValue(AVKey.RANGE));
        view.setViewOutOfFocus(true);
    }
}
}
