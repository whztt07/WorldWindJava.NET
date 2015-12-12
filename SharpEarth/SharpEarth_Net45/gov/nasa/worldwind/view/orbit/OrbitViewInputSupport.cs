/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SharpEarth.geom;
using SharpEarth.globes;
using SharpEarth.java.lang;
using SharpEarth.terrain;
using SharpEarth.util;

namespace SharpEarth.view.orbit
{
/**
 * @author dcollins
 * @version $Id: OrbitViewInputSupport.java 1171 2013-02-11 21:45:02Z dcollins $
 */

  public class OrbitViewInputSupport
  {
    public static Matrix computeTransformMatrix( Globe globe, Position center, Angle heading, Angle pitch, Angle roll,
      double zoom )
    {
      if ( globe == null )
      {
        var message = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( center == null )
      {
        var message = Logging.getMessage( "nullValue.CenterIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( heading == null )
      {
        var message = Logging.getMessage( "nullValue.HeadingIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( pitch == null )
      {
        var message = Logging.getMessage( "nullValue.PitchIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // Construct the model-view transform matrix for the specified coordinates.
      // Because this is a model-view transform, matrices are applied in reverse order.
      Matrix transform;
      // Zoom, heading, pitch.
      transform = computeHeadingPitchRollZoomTransform( heading, pitch, roll, zoom );
      // Center position.
      transform = transform.multiply( computeCenterTransform( globe, center ) );

      return transform;
    }

    public static OrbitViewState computeOrbitViewState( Globe globe, Vec4 eyePoint, Vec4 centerPoint, Vec4 up )
    {
      if ( globe == null )
      {
        var message = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( eyePoint == null )
      {
        var message = "nullValue.EyePointIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( centerPoint == null )
      {
        var message = "nullValue.CenterPointIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( up == null )
      {
        var message = "nullValue.UpIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      var modelview = Matrix.fromViewLookAt( eyePoint, centerPoint, up );
      return computeOrbitViewState( globe, modelview, centerPoint );
    }

    public static OrbitViewState computeOrbitViewState( Globe globe, Matrix modelTransform, Vec4 centerPoint )
    {
      if ( globe == null )
      {
        var message = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( modelTransform == null )
      {
        var message = "nullValue.ModelTransformIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( centerPoint == null )
      {
        var message = "nullValue.CenterPointIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // Compute the center position.
      var centerPos = globe.computePositionFromPoint( centerPoint );
      // Compute the center position transform.
      var centerTransform = computeCenterTransform( globe, centerPos );
      var centerTransformInv = centerTransform.getInverse();
      if ( centerTransformInv == null )
      {
        var message = Logging.getMessage( "generic.NoninvertibleMatrix" );
        Logging.logger().severe( message );
        throw new IllegalStateException( message );
      }

      // Compute the heading-pitch-zoom transform.
      var hpzTransform = modelTransform.multiply( centerTransformInv );
      // Extract the heading, pitch, and zoom values from the transform.
      var heading = ViewUtil.computeHeading( hpzTransform );
      var pitch = ViewUtil.computePitch( hpzTransform );
      var zoom = computeZoom( hpzTransform );
      if ( heading == null || pitch == null )
        return null;

      return new OrbitViewState( centerPos, heading, pitch, zoom );
    }

    protected static Matrix computeCenterTransform( Globe globe, Position center )
    {
      if ( globe == null )
      {
        var message = Logging.getMessage( "nullValue.GlobeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( center == null )
      {
        var message = Logging.getMessage( "nullValue.CenterIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // The view eye position will be the same as the center position.
      // This is only the case without any zoom, heading, and pitch.
      var eyePoint = globe.computePointFromPosition( center );
      // The view forward direction will be colinear with the
      // geoid surface normal at the center position.
      var normal = globe.computeSurfaceNormalAtLocation( center.getLatitude(), center.getLongitude() );
      var lookAtPoint = eyePoint.subtract3( normal );
      // The up direction will be pointing towards the north pole.
      var north = globe.computeNorthPointingTangentAtLocation( center.getLatitude(), center.getLongitude() );
      // Creates a viewing matrix looking from eyePoint towards lookAtPoint,
      // with the given up direction. The forward, right, and up vectors
      // contained in the matrix are guaranteed to be orthogonal. This means
      // that the Matrix's up may not be equivalent to the specified up vector
      // here (though it will point in the same general direction).
      // In this case, the forward direction would not be affected.
      return Matrix.fromViewLookAt( eyePoint, lookAtPoint, north );
    }

    protected static Matrix computeHeadingPitchRollZoomTransform( Angle heading, Angle pitch, Angle roll, double zoom )
    {
      if ( heading == null )
      {
        var message = Logging.getMessage( "nullValue.HeadingIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( pitch == null )
      {
        var message = Logging.getMessage( "nullValue.PitchIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( roll == null )
      {
        var message = Logging.getMessage( "nullValue.RollIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      Matrix transform;
      // Zoom.
      transform = Matrix.fromTranslation( 0, 0, -zoom );
      // Roll is rotation around the Z axis
      transform = transform.multiply( Matrix.fromRotationZ( roll ) );
      // Pitch is treated clockwise as rotation about the X-axis. We flip the pitch value so that a positive
      // rotation produces a clockwise rotation (when facing the axis).
      transform = transform.multiply( Matrix.fromRotationX( pitch.multiply( -1.0 ) ) );
      // Heading.
      transform = transform.multiply( Matrix.fromRotationZ( heading ) );
      return transform;
    }

    protected static double computeZoom( Matrix headingPitchZoomTransform )
    {
      if ( headingPitchZoomTransform == null )
      {
        var message = "nullValue.HeadingPitchZoomTransformTransformIsNull";
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      var v = headingPitchZoomTransform.getTranslation();
      return v != null ? v.getLength3() : 0.0;
    }

    public static OrbitViewState getSurfaceIntersection( Globe globe, SectorGeometryList terrain, Position centerPosition,
      Angle heading, Angle pitch, double zoom )
    {
      if ( globe != null )
      {
        var modelview = computeTransformMatrix( globe, centerPosition,
          heading, pitch, Angle.ZERO, zoom );
        if ( modelview != null )
        {
          var modelviewInv = modelview.getInverse();
          if ( modelviewInv != null )
          {
            var eyePoint = Vec4.UNIT_W.transformBy4( modelviewInv );
            var centerPoint = globe.computePointFromPosition( centerPosition );
            var eyeToCenter = eyePoint.subtract3( centerPoint );
            var intersections = terrain.intersect( new Line( eyePoint, eyeToCenter.normalize3().multiply3( -1 ) ) );
            if ( intersections != null && intersections.Length >= 0 )
            {
              var newCenter = globe.computePositionFromPoint( intersections[0].getIntersectionPoint() );
              return ( new OrbitViewState( newCenter, heading, pitch, zoom ) );
            }
          }
        }
      }
      return null;
    }

    public class OrbitViewState // public to allow access from subclasses
    {
      private readonly Position center;
      private readonly Angle heading;
      private readonly Angle pitch;
      private readonly double zoom;

      public OrbitViewState( Position center, Angle heading, Angle pitch, double zoom )
      {
        if ( center == null )
        {
          var message = Logging.getMessage( "nullValue.CenterIsNull" );
          Logging.logger().severe( message );
          throw new ArgumentException( message );
        }
        if ( heading == null )
        {
          var message = Logging.getMessage( "nullValue.HeadingIsNull" );
          Logging.logger().severe( message );
          throw new ArgumentException( message );
        }
        if ( pitch == null )
        {
          var message = Logging.getMessage( "nullValue.PitchIsNull" );
          Logging.logger().severe( message );
          throw new ArgumentException( message );
        }

        this.center = center;
        this.heading = heading;
        this.pitch = pitch;
        this.zoom = zoom;
      }

      public Position getCenterPosition()
      {
        return center;
      }

      public Angle getHeading()
      {
        return heading;
      }

      public Angle getPitch()
      {
        return pitch;
      }

      public double getZoom()
      {
        return zoom;
      }
    }
  }
}