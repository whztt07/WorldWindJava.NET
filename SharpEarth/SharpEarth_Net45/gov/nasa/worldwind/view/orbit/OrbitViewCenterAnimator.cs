/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.geom;
using SharpEarth.animation.MoveToPositionAnimator;
namespace SharpEarth.view.orbit{


/**
 * A position animator that has the ability to adjust the view to focus on the
 * terrain when it is stopped.
 *
 * @author jym
 * @version $Id: OrbitViewCenterAnimator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OrbitViewCenterAnimator : MoveToPositionAnimator
{
    private BasicOrbitView orbitView;
    bool endCenterOnSurface;
    public OrbitViewCenterAnimator(BasicOrbitView orbitView, Position startPosition, Position endPosition,
        double smoothing, PropertyAccessor.PositionAccessor propertyAccessor, bool endCenterOnSurface)
    {
        super(startPosition, endPosition, smoothing, propertyAccessor);
        this.endCenterOnSurface = endCenterOnSurface;
        this.orbitView = orbitView;
    }

    public Position nextPosition(double interpolant)
    {
        Position nextPosition = this.end;
        Position curCenter = this.propertyAccessor.getPosition();

        double latlonDifference = LatLon.greatCircleDistance(nextPosition, curCenter).degrees;
        double elevDifference = Math.Abs(nextPosition.getElevation() - curCenter.getElevation());
        bool stopMoving = Math.Max(latlonDifference, elevDifference) < this.positionMinEpsilon;
        if (!stopMoving)
        {
            interpolant = 1 - this.smoothing;
            nextPosition = new Position(
                Angle.mix(interpolant, curCenter.getLatitude(), this.end.getLatitude()),
                Angle.mix(interpolant, curCenter.getLongitude(), this.end.getLongitude()),
                (1 - interpolant) * curCenter.getElevation() + interpolant * this.end.getElevation());
        }
        //TODO: What do we do about collisions?
        /*
        try
        {
            // Clear any previous collision state the view may have.
            view.hadCollisions();
            view.setEyePosition(nextCenter);
            // If the change caused a collision, update the target center position with the
            // elevation that resolved the collision.
            if (view.hadCollisions())
                this.eyePositionTarget = new Position(
                        this.eyePositionTarget, view.getEyePosition().getElevation());
            flagViewChanged();
            setViewOutOfFocus(true);
        }
        catch (Exception e)
        {
            String message = Logging.getMessage("generic.ExceptionWhileChangingView");
            Logging.logger().log(java.util.logging.Level.SEVERE, message, e);
            stopMoving = true;
        }
        */

        // If target is close, cancel future value changes.
        if (stopMoving)
        {
            this.stop();
            this.propertyAccessor.setPosition(nextPosition);
            if (endCenterOnSurface)
                this.orbitView.setViewOutOfFocus(true);
            return(null);
        }
        return nextPosition;
    }

    protected void setImpl(double interpolant)
    {
        Position newValue = this.nextPosition(interpolant);
        if (newValue == null)
           return;

        this.propertyAccessor.setPosition(newValue);
        this.orbitView.setViewOutOfFocus(true);
    }

    public void stop()
    {
        super.stop();
    }
}
}
