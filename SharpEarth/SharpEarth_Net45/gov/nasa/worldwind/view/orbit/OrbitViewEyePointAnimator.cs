/*
 * Copyright (C) 2014 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SharpEarth.animation;
using SharpEarth.avlist;
using SharpEarth.geom;
using SharpEarth.globes;
using SharpEarth.util;

namespace SharpEarth.view.orbit
{
/**
 * @author dcollins
 * @version $Id: OrbitViewEyePointAnimator.java 2204 2014-08-07 23:35:03Z dcollins $
 */

  public class OrbitViewEyePointAnimator : Animator
  {
    protected static readonly double STOP_DISTANCE = 0.1;
    protected Vec4 eyePoint;
    protected Globe globe;
    protected bool HasNext;
    protected double smoothing;
    protected BasicOrbitView view;

    public OrbitViewEyePointAnimator( Globe globe, BasicOrbitView view, Vec4 eyePoint, double smoothing )
    {
      if ( globe == null )
      {
        var msg = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      if ( view == null )
      {
        var msg = Logging.getMessage( "nullValue.ViewIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      if ( eyePoint == null )
      {
        var msg = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      this.globe = globe;
      this.view = view;
      this.eyePoint = eyePoint;
      this.smoothing = smoothing;
      HasNext = true;
    }

    public void start()
    {
      HasNext = true;
    }

    public void stop()
    {
      HasNext = false;
    }

    public bool hasNext()
    {
      return HasNext;
    }

    public void set( double interpolant )
    {
      // Intentionally left blank.
    }

    public void next()
    {
      var modelview = view.getModelviewMatrix();
      var point = modelview.extractEyePoint();

      if ( point.distanceTo3( eyePoint ) > STOP_DISTANCE )
      {
        point = Vec4.mix3( 1 - smoothing, point, eyePoint );
        setEyePoint( globe, view, point );
      }
      else
      {
        setEyePoint( globe, view, eyePoint );
        stop();
      }
    }

    public void setEyePoint( Vec4 eyePoint )
    {
      this.eyePoint = eyePoint;
    }

    public static void setEyePoint( Globe globe, BasicOrbitView view, Vec4 newEyePoint )
    {
      if ( globe == null )
      {
        var msg = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      if ( view == null )
      {
        var msg = Logging.getMessage( "nullValue.ViewIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      if ( newEyePoint == null )
      {
        var msg = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      // Translate the view's modelview matrix to the specified new eye point, and compute the new center point by
      // assuming that the view's zoom distance does not change.
      var translation = view.getModelviewMatrix().extractEyePoint().subtract3( newEyePoint );
      var modelview = view.getModelviewMatrix().multiply( Matrix.fromTranslation( translation ) );
      var eyePoint = modelview.extractEyePoint();
      var forward = modelview.extractForwardVector();
      var centerPoint = eyePoint.add3( forward.multiply3( view.getZoom() ) );

      // Set the view's properties from the new modelview matrix.
      var parameters = modelview.extractViewingParameters( centerPoint, view.getRoll(), globe );
      view.setCenterPosition( (Position)parameters.getValue( AVKey.ORIGIN ) );
      view.setHeading( (Angle)parameters.getValue( AVKey.HEADING ) );
      view.setPitch( (Angle)parameters.getValue( AVKey.TILT ) );
      view.setRoll( (Angle)parameters.getValue( AVKey.ROLL ) );
      view.setZoom( (double)parameters.getValue( AVKey.RANGE ) );
      view.setViewOutOfFocus( true );
    }
  }
}