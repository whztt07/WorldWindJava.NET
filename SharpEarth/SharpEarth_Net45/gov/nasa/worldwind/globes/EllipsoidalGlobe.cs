/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using SharpEarth.util;
using SharpEarth.terrain;
using SharpEarth.render;
using SharpEarth.geom;
using SharpEarth.avlist;
using SharpEarth.globes;
using SharpEarth;
using SharpEarth.java.lang;

namespace SharpEarth.globes
{



/**
 * Defines a globe modeled as an <a href="http://mathworld.wolfram.com/Ellipsoid.html" target="_blank">ellipsoid</a>.
 * This globe uses a Cartesian coordinate system in which the Y axis points to the north pole. The Z axis points to the
 * intersection of the prime meridian and the equator, in the equatorial plane. The X axis completes a right-handed
 * coordinate system, and is 90 degrees east of the Z axis and also in the equatorial plane. Sea level is at z = zero.
 * By default the origin of the coordinate system lies at the center of the globe, but can be set to a different point
 * when the globe is constructed.
 *
 * @author Tom Gaskins
 * @version $Id: EllipsoidalGlobe.java 2295 2014-09-04 17:33:25Z tgaskins $
 */

  public class EllipsoidalGlobe : WWObjectImpl, Globe
  {
    protected readonly double equatorialRadius;
    protected readonly double polarRadius;
    protected readonly double es;
    private readonly Vec4 center;
    private ElevationModel _elevationModel;
    private Tessellator _tessellator;
    protected EGM96 egm96;

    /**
     * Create a new globe. The globe's center point will be (0, 0, 0). The globe will be tessellated using tessellator
     * defined by the {@link AVKey#TESSELLATOR_CLASS_NAME} configuration parameter.
     *
     * @param equatorialRadius Radius of the globe at the equator.
     * @param polarRadius      Radius of the globe at the poles.
     * @param es               Square of the globe's eccentricity.
     * @param em               Elevation model. May be null.
     */

    public EllipsoidalGlobe( double equatorialRadius, double polarRadius, double es, ElevationModel em )
      : this( equatorialRadius, polarRadius, es, em, Vec4.ZERO )
    {
    }

    /**
     * Create a new globe, and set the position of the globe's center. The globe will be tessellated using tessellator
     * defined by the {@link AVKey#TESSELLATOR_CLASS_NAME} configuration parameter.
     *
     * @param equatorialRadius Radius of the globe at the equator.
     * @param polarRadius      Radius of the globe at the poles.
     * @param es               Square of the globe's eccentricity.
     * @param em               Elevation model. May be null.
     * @param center           Cartesian coordinates of the globe's center point.
     */

    public EllipsoidalGlobe( double equatorialRadius, double polarRadius, double es, ElevationModel em, Vec4 center )
    {
      this.equatorialRadius = equatorialRadius;
      this.polarRadius = polarRadius;
      this.es = es; // assume it's consistent with the two radii
      this.center = center;
      this._elevationModel = em;
      this._tessellator = (Tessellator)WorldWind.createConfigurationComponent( AVKey.TESSELLATOR_CLASS_NAME );
    }

    protected class StateKey : GlobeStateKey
    {
      protected Globe globe;
      protected readonly Tessellator _tessellator;
      protected double verticalExaggeration;
      protected ElevationModel elevationModel;

      public StateKey( DrawContext dc, EllipsoidalGlobe ellipsoidalGlobe )
      {
        if ( dc == null )
        {
          string msg = Logging.getMessage( "nullValue.DrawContextIsNull" );
          Logging.logger().severe( msg );
          throw new ArgumentException( msg );
        }

        this.globe = dc.getGlobe();
        this._tessellator = ellipsoidalGlobe._tessellator;
        this.verticalExaggeration = dc.getVerticalExaggeration();
        this.elevationModel = this.globe.getElevationModel();
      }

      public StateKey( EllipsoidalGlobe ellipsoidalGlobe  )
      {
        this.globe = ellipsoidalGlobe;
        this._tessellator = ellipsoidalGlobe._tessellator;
        this.verticalExaggeration = 1;
        this.elevationModel = this.globe.getElevationModel();
      }

      public Globe getGlobe()
      {
        return this.globe;
      }

      public override bool Equals( object o )
      {
        if ( this == o )
          return true;
        if ( o == null || GetType() != o.GetType() )
          return false;

        StateKey stateKey = (StateKey)o;

        if ( Math.Abs( stateKey.verticalExaggeration - verticalExaggeration ) > 0.0000001 )
          return false;
        if ( elevationModel != null ? !elevationModel.Equals( stateKey.elevationModel ) :
          stateKey.elevationModel != null )
          return false;
        if ( globe != null ? !globe.Equals( stateKey.globe ) : stateKey.globe != null )
          return false;
        if ( _tessellator != null ? !_tessellator.Equals( stateKey._tessellator ) : stateKey._tessellator != null )
          return false;

        return true;
      }

      public override int GetHashCode()
      {
        int result;
        long temp;
        result = globe != null ? globe.GetHashCode() : 0;
        result = 31 * result + ( _tessellator != null ? _tessellator.GetHashCode() : 0 );
        temp = verticalExaggeration != +0.0d ? BitConverter.DoubleToInt64Bits( verticalExaggeration ) : 0L;
        result = 31 * result + (int)( temp ^ ( (int)( (uint)temp >> 32 ) ) );
        result = 31 * result + ( elevationModel != null ? elevationModel.GetHashCode() : 0 );
        return result;
      }
    }

    public object getStateKey( DrawContext dc )
    {
      return this.getGlobeStateKey( dc );
    }

    public GlobeStateKey getGlobeStateKey( DrawContext dc )
    {
      return new StateKey( dc, this );
    }

    public GlobeStateKey getGlobeStateKey()
    {
      return new StateKey( this );
    }

    public Tessellator getTessellator()
    {
      return _tessellator;
    }

    public void setTessellator( Tessellator tessellator )
    {
      this._tessellator = tessellator;
    }

    public ElevationModel getElevationModel()
    {
      return _elevationModel;
    }

    public void setElevationModel( ElevationModel elevationModel )
    {
      this._elevationModel = elevationModel;
    }

    public double getRadius()
    {
      return this.equatorialRadius;
    }

    public double getEquatorialRadius()
    {
      return this.equatorialRadius;
    }

    public double getPolarRadius()
    {
      return this.polarRadius;
    }

    public double getMaximumRadius()
    {
      return this.equatorialRadius;
    }

    public double getRadiusAt( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        String msg = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      // The radius for an ellipsoidal globe is a function of its latitude. The following solution was derived by
      // observing that the length of the ellipsoidal point at the specified latitude and longitude indicates the
      // radius at that location. The formula for the length of the ellipsoidal point was then converted into the
      // simplified form below.

      double sinLat = Math.Sin( latitude.radians );
      double rpm = this.equatorialRadius / Math.Sqrt( 1.0 - this.es * sinLat * sinLat );

      return rpm * Math.Sqrt( 1.0 + ( this.es * this.es - 2.0 * this.es ) * sinLat * sinLat );
    }

    public double getRadiusAt( LatLon location )
    {
      if ( location == null )
      {
        String msg = Logging.getMessage( "nullValue.LocationIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      return this.getRadiusAt( location.latitude, location.longitude );
    }

    public double getEccentricitySquared()
    {
      return this.es;
    }

    public double getDiameter()
    {
      return this.equatorialRadius * 2;
    }

    public Vec4 getCenter()
    {
      return this.center;
    }

    public double[] getElevations<T>( Sector sector, List<T> latlons, double[] targetResolution, double[] elevations ) where T : LatLon
    {
      throw new NotImplementedException();
    }

    public double getMaxElevation()
    {
      return this._elevationModel != null ? this._elevationModel.getMaxElevation() : 0;
    }

    public double getMinElevation()
    {
      // TODO: The value returned might not reflect the globe's actual minimum elevation if the elevation model does
      // not span the full globe. See WWJINT-435.
      return this._elevationModel != null ? this._elevationModel.getMinElevation() : 0;
    }

    public double[] getMinAndMaxElevations( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        String msg = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      return this._elevationModel != null ? this._elevationModel.getExtremeElevations( latitude, longitude )
        : new double[] { 0, 0 };
    }

    public double[] getMinAndMaxElevations( Sector sector )
    {
      if ( sector == null )
      {
        string message = Logging.getMessage( "nullValue.SectorIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentNullException( message );
      }

      return this._elevationModel != null ? this._elevationModel.getExtremeElevations( sector ) : new double[] { 0, 0 };
    }

    public Extent getExtent()
    {
      return this;
    }

    public double getEffectiveRadius( Plane plane )
    {
      return this.getRadius();
    }

    public bool intersects( Frustum frustum )
    {
      if ( frustum == null )
      {
        string message = Logging.getMessage( "nullValue.FrustumIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentNullException( message );
      }

      return frustum.intersects( new Sphere( Vec4.ZERO, this.getRadius() ) );
    }

    public Intersection[] intersect( Line line )
    {
      return this.intersect( line, this.equatorialRadius, this.polarRadius );
    }

    public Intersection[] intersect( Line line, double altitude )
    {
      return this.intersect( line, this.equatorialRadius + altitude, this.polarRadius + altitude );
    }

    protected Intersection[] intersect( Line line, double equRadius, double polRadius )
    {
      if ( line == null )
        return null;

      // Taken from Lengyel, 2Ed., Section 5.2.3, page 148.

      double m = equRadius / polRadius; // "ratio of the x semi-axis length to the y semi-axis length"
      double n = 1d; // "ratio of the x semi-axis length to the z semi-axis length"
      double m2 = m * m;
      double n2 = n * n;
      double r2 = equRadius * equRadius; // nominal radius squared //equRadius * polRadius;

      double vx = line.getDirection().getX();
      double vy = line.getDirection().getY();
      double vz = line.getDirection().getZ();
      double sx = line.getOrigin().getX();
      double sy = line.getOrigin().getY();
      double sz = line.getOrigin().getZ();

      double a = vx * vx + m2 * vy * vy + n2 * vz * vz;
      double b = 2d * ( sx * vx + m2 * sy * vy + n2 * sz * vz );
      double c = sx * sx + m2 * sy * sy + n2 * sz * sz - r2;

      double discriminant = Discriminant( a, b, c );
      if ( discriminant < 0 )
        return null;

      double discriminantRoot = Math.Sqrt( discriminant );
      if ( discriminant == 0 )
      {
        Vec4 p = line.getPointAt( ( -b - discriminantRoot ) / ( 2 * a ) );
        return new Intersection[] { new Intersection( p, true ) };
      }
      else // (discriminant > 0)
      {
        Vec4 near = line.getPointAt( ( -b - discriminantRoot ) / ( 2 * a ) );
        Vec4 far = line.getPointAt( ( -b + discriminantRoot ) / ( 2 * a ) );
        if ( c >= 0 ) // Line originates outside the Globe.
          return new Intersection[] { new Intersection( near, false ), new Intersection( far, false ) };
        else // Line originates inside the Globe.
          return new Intersection[] { new Intersection( far, false ) };
      }
    }

    private static double Discriminant( double a, double b, double c )
    {
      return b * b - 4 * a * c;
    }

    public Intersection[] intersect( Triangle t, double elevation )
    {
      if ( t == null )
        return null;

      bool bA = isPointAboveElevation( t.getA(), elevation );
      bool bB = isPointAboveElevation( t.getB(), elevation );
      bool bC = isPointAboveElevation( t.getC(), elevation );

      if ( !( bA ^ bB ) && !( bB ^ bC ) )
        return null; // all triangle points are either above or below the given elevation

      Intersection[] inter = new Intersection[2];
      int idx = 0;

      // Assumes that intersect(Line) returns only one intersection when the line
      // originates inside the ellipsoid at the given elevation.
      if ( bA ^ bB )
        if ( bA )
          inter[idx++] = intersect( new Line( t.getB(), t.getA().subtract3( t.getB() ) ), elevation )[0];
        else
          inter[idx++] = intersect( new Line( t.getA(), t.getB().subtract3( t.getA() ) ), elevation )[0];

      if ( bB ^ bC )
        if ( bB )
          inter[idx++] = intersect( new Line( t.getC(), t.getB().subtract3( t.getC() ) ), elevation )[0];
        else
          inter[idx++] = intersect( new Line( t.getB(), t.getC().subtract3( t.getB() ) ), elevation )[0];

      if ( bC ^ bA )
        if ( bC )
          inter[idx] = intersect( new Line( t.getA(), t.getC().subtract3( t.getA() ) ), elevation )[0];
        else
          inter[idx] = intersect( new Line( t.getC(), t.getA().subtract3( t.getC() ) ), elevation )[0];

      return inter;
    }

    public bool intersects( Line line )
    {
      //noinspection SimplifiableIfStatement
      if ( line == null )
        return false;

      return line.distanceTo( this.center ) <= this.equatorialRadius;
    }

    public bool intersects( Plane plane )
    {
      if ( plane == null )
        return false;

      double dq1 = plane.dot( this.center );
      return dq1 <= this.equatorialRadius;
    }

    /** {@inheritDoc} */

    public double getProjectedArea( View view )
    {
      if ( view == null )
      {
        string message = Logging.getMessage( "nullValue.ViewIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentNullException( message );
      }

      return WWMath.computeSphereProjectedArea( view, this.getCenter(), this.getRadius() );
    }

    public void applyEGMA96Offsets( string offsetsFilePath )
    {
      if ( offsetsFilePath != null )
        this.egm96 = new EGM96( offsetsFilePath );
      else
        this.egm96 = null;
    }

    public double getElevationsAndResolution<T>( Sector sector, List<T> latlons, double targetResolution, double[] elevations ) where T : LatLon
    {
      if ( _elevationModel == null )
        return 0;

      double resolution = _elevationModel.getElevations( sector, latlons, targetResolution, elevations );

      if ( egm96 == null )
        return resolution;

      for ( int i = 0; i < elevations.Length; i++ )
      {
        LatLon latLon = latlons[i];
        elevations[i] += + egm96.getOffset( latLon.getLatitude(), latLon.getLongitude() );
      }

      return resolution;
    }

    public double[] getElevationsAndResolutions<T>( Sector sector, List<T> latLons, double[] targetResolution, double[] elevations ) where T : LatLon
    {
      if ( _elevationModel == null )
        return new double[] { 0 };

      double[] resolution = _elevationModel.getElevations( sector, latLons, targetResolution, elevations );

      if(egm96 == null)
        return resolution;

      for ( int i = 0; i < elevations.Length; i++ )
      {
        LatLon latLon = latLons[i];
        elevations[i] += egm96.getOffset( latLon.getLatitude(), latLon.getLongitude() );
      }

      return resolution;
    }

    public double getElevation( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( _elevationModel == null )
        return 0;

      double elevation = _elevationModel.getElevation( latitude, longitude );

      if ( egm96 != null )
        elevation += egm96.getOffset( latitude, longitude );

      return elevation;
    }

    public Vec4 computePointFromPosition( Position position )
    {
      if ( position == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return geodeticToCartesian( position.getLatitude(), position.getLongitude(), position.getElevation() );
    }

    public Vec4 computePointFromLocation( LatLon location )
    {
      if ( location == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return geodeticToCartesian( location.getLatitude(), location.getLongitude(), 0 );
    }

    public Vec4 computePointFromPosition( LatLon latLon, double metersElevation )
    {
      if ( latLon == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return geodeticToCartesian( latLon.getLatitude(), latLon.getLongitude(), metersElevation );
    }

    public Vec4 computePointFromPosition( Angle latitude, Angle longitude, double metersElevation )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return geodeticToCartesian( latitude, longitude, metersElevation );
    }

    public Position computePositionFromPoint( Vec4 point )
    {
      if ( point == null )
      {
        string message = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return cartesianToGeodetic( point );
    }

    public void computePointsFromPositions( Sector sector, int numLat, int numLon, double[] metersElevation, Vec4[] outVector )
    {
      if ( sector == null )
      {
        string message = Logging.getMessage( "nullValue.SectorIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( numLat <= 0 || numLon <= 0 )
      {
        string message = Logging.getMessage( "generic.ArgumentOutOfRange", "numLat <= 0 or numLon <= 0" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( metersElevation == null )
      {
        string message = Logging.getMessage( "nullValue.ElevationsIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( outVector == null)
      {
        string message = Logging.getMessage( "nullValue.OutputIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      geodeticToCartesian( sector, numLat, numLon, metersElevation, outVector );
    }

    /**
     * Returns the normal to the Globe at the specified position.
     *
     * @param latitude  the latitude of the position.
     * @param longitude the longitude of the position.
     *
     * @return the Globe normal at the specified position.
     */

    public Vec4 computeSurfaceNormalAtLocation( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return computeEllipsoidalNormalAtLocation( latitude, longitude );
    }

    /**
     * Returns the normal to the Globe at the specified cartiesian point.
     *
     * @param point the cartesian point.
     *
     * @return the Globe normal at the specified point.
     */

    public Vec4 computeSurfaceNormalAtPoint( Vec4 point )
    {
      if ( point == null )
      {
        string message = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double eqSquared = equatorialRadius * equatorialRadius;
      double polSquared = polarRadius * polarRadius;

      double x = ( point.x() - center.x() ) / eqSquared;
      double y = ( point.y() - center.y() ) / polSquared;
      double z = ( point.z() - center.z() ) / eqSquared;

      return new Vec4( x, y, z ).normalize3();
    }

    public Vec4 computeNorthPointingTangentAtLocation( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // Latitude is treated clockwise as rotation about the X-axis. We flip the latitude value so that a positive
      // rotation produces a clockwise rotation (when facing the axis).
      latitude = latitude.multiply( -1.0 );

      double cosLat = latitude.cos();
      double sinLat = latitude.sin();
      double cosLon = longitude.cos();
      double sinLon = longitude.sin();

      // The north-pointing tangent is derived by rotating the vector (0, 1, 0) about the Y-axis by longitude degrees,
      // then rotating it about the X-axis by -latitude degrees. This can be represented by a combining two rotation
      // matrices Rlat, and Rlon, then transforeachming the vector (0, 1, 0) by the combined transform in 
      //
      // NorthTangent = (Rlon * Rlat) * (0, 1, 0)
      //
      // Since the input vector only has a Y coordinate, this computation can be simplified. The simplified
      // computation is shown here as NorthTangent = (x, y, z).
      //
      double x = sinLat * sinLon;
      //noinspection UnnecessaryLocalVariable
      double y = cosLat;
      double z = sinLat * cosLon;

      return new Vec4( x, y, z ).normalize3();
    }

    public Matrix computeModelCoordinateOriginTransform( Angle latitude, Angle longitude, double metersElevation )
    {
      return computeSurfaceOrientationAtPosition( latitude, longitude, metersElevation );
    }

    public Matrix computeModelCoordinateOriginTransform( Position position )
    {
      return computeSurfaceOrientationAtPosition( position );
    }

    public Matrix computeSurfaceOrientationAtPosition( Angle latitude, Angle longitude, double metersElevation )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return computeEllipsoidalOrientationAtPosition( latitude, longitude, metersElevation );
    }

    public Matrix computeSurfaceOrientationAtPosition( Position position )
    {
      if ( position == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return this.computeSurfaceOrientationAtPosition( position.getLatitude(), position.getLongitude(),
        position.getElevation() );
    }

    public Vec4 computeEllipsoidalPointFromPosition( Angle latitude, Angle longitude, double metersElevation )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return this.geodeticToEllipsoidal( latitude, longitude, metersElevation );
    }

    public Vec4 computeEllipsoidalPointFromPosition( Position position )
    {
      if ( position == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return this.computeEllipsoidalPointFromPosition( position.getLatitude(), position.getLongitude(),
        position.getAltitude() );
    }

    public Vec4 computeEllipsoidalPointFromLocation( LatLon location )
    {
      if ( location == null )
      {
        string message = Logging.getMessage( "nullValue.LocationIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return this.geodeticToEllipsoidal( location.getLatitude(), location.getLongitude(), 0 );
    }

    public Position computePositionFromEllipsoidalPoint( Vec4 ellipsoidalPoint )
    {
      if ( ellipsoidalPoint == null )
      {
        string message = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return this.ellipsoidalToGeodetic( ellipsoidalPoint );
    }

    public Vec4 computeEllipsoidalNormalAtLocation( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double cosLat = latitude.cos();
      double cosLon = longitude.cos();
      double sinLat = latitude.sin();
      double sinLon = longitude.sin();

      double eq2 = this.equatorialRadius * this.equatorialRadius;
      double pol2 = this.polarRadius * this.polarRadius;

      double x = cosLat * sinLon / eq2;
      double y = ( 1.0 - this.es ) * sinLat / pol2;
      double z = cosLat * cosLon / eq2;

      return new Vec4( x, y, z ).normalize3();
    }

    public Matrix computeEllipsoidalOrientationAtPosition( Angle latitude, Angle longitude,
      double metersElevation )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      Vec4 point = this.computeEllipsoidalPointFromPosition( latitude, longitude, metersElevation );
      // Transform to the cartesian coordinates of (latitude, longitude, metersElevation).
      Matrix transform = Matrix.fromTranslation( point );
      // Rotate the coordinate system to match the longitude.
      // Longitude is treated as counter-clockwise rotation about the Y-axis.
      transform = transform.multiply( Matrix.fromRotationY( longitude ) );
      // Rotate the coordinate system to match the latitude.
      // Latitude is treated clockwise as rotation about the X-axis. We flip the latitude value so that a positive
      // rotation produces a clockwise rotation (when facing the axis).
      transform = transform.multiply( Matrix.fromRotationX( latitude.multiply( -1.0 ) ) );
      return transform;
    }

    public Position getIntersectionPosition( Line line )
    {
      if ( line == null )
      {
        string message = Logging.getMessage( "nullValue.LineIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      Intersection[] intersections = intersect( line );
      if ( intersections == null )
        return null;

      return computePositionFromPoint( intersections[0].getIntersectionPoint() );
    }

    /**
     * Maps a position to world Cartesian coordinates. The Y axis points to the north pole. The Z axis points to the
     * intersection of the prime meridian and the equator, in the equatorial plane. The X axis completes a right-handed
     * coordinate system, and is 90 degrees east of the Z axis and also in the equatorial plane. Sea level is at z =
     * zero.
     *
     * @param latitude        the latitude of the position.
     * @param longitude       the longitude of the position.
     * @param metersElevation the number of meters above or below mean sea level.
     *
     * @return The Cartesian point corresponding to the input position.
     */

    protected Vec4 geodeticToCartesian( Angle latitude, Angle longitude, double metersElevation )
    {
      return geodeticToEllipsoidal( latitude, longitude, metersElevation );
    }

    /**
     * Maps a position to ellipsoidal coordinates. The Y axis points to the north pole. The Z axis points to the
     * intersection of the prime meridian and the equator, in the equatorial plane. The X axis completes a right-handed
     * coordinate system, and is 90 degrees east of the Z axis and also in the equatorial plane. Sea level is at z =
     * zero.
     *
     * @param latitude        the latitude of the position.
     * @param longitude       the longitude of the position.
     * @param metersElevation the number of meters above or below mean sea level.
     *
     * @return The ellipsoidal point corresponding to the input position.
     *
     * @see #ellipsoidalToGeodetic(gov.nasa.worldwind.geom.Vec4)
     */

    protected Vec4 geodeticToEllipsoidal( Angle latitude, Angle longitude, double metersElevation )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double cosLat = Math.Cos( latitude.radians );
      double sinLat = Math.Sin( latitude.radians );
      double cosLon = Math.Cos( longitude.radians );
      double sinLon = Math.Sin( longitude.radians );

      // getRadius (in meters) of vertical in prime meridian
      double rpm =  equatorialRadius / Math.Sqrt( 1.0 - this.es * sinLat * sinLat );

      double x = ( rpm + metersElevation ) * cosLat * sinLon;
      double y = ( rpm * ( 1.0 - this.es ) + metersElevation ) * sinLat;
      double z = ( rpm + metersElevation ) * cosLat * cosLon;

      return new Vec4( x, y, z );
    }

    /**
     * Maps a grid of geographic positions to Cartesian coordinates. The Y axis points to the north pole. The Z axis
     * points to the intersection of the prime meridian and the equator, in the equatorial plane. The X axis completes a
     * right-handed coordinate system, and is 90 degrees east of the Z axis and also in the equatorial plane. Sea level
     * is at z = zero.
     * <p/>
     * This method provides an interface for efficient generation of a grid of cartesian points within a sector. The
     * grid is constructed by dividing the sector into <code>numLon x numLat</code> evenly separated points in
     * geographic coordinates. The first and last points in latitude and longitude are placed at the sector's minimum
     * and maximum boundary, and the remaining points are spaced evenly between those boundary points.
     * <p/>
     * For each grid point within the sector, an elevation value is specified via an array of elevations. The
     * calculation at each position incorporates the associated elevation.
     *
     * @param sector          The sector over which to generate the points.
     * @param numLat          The number of points to generate latitudinally.
     * @param numLon          The number of points to generate longitudinally.
     * @param metersElevation An array of elevations to incorporate in the point calculations. There must be one
     *                        elevation value in the array for each generated point, so the array must have a length of
     *                        at least <code>numLon x numLat</code>. Elevations are read from this array in row major
     *                        order, beginning with the row of minimum latitude.
     * @param out             An array to hold the computed cartesian points. It must have a length of at least
     *                        <code>numLon x numLat</code>. Points are written to this array in row major order,
     *                        beginning with the row of minimum latitude.
     *
     * @throws ArgumentException If any argument is null, or if numLat or numLon are less than or equal to zero.
     */

    protected void geodeticToCartesian( Sector sector, int numLat, int numLon, double[] metersElevation, Vec4[] outVector )
    {
      double minLat = sector.getMinLatitude().radians;
      double maxLat = sector.getMaxLatitude().radians;
      double minLon = sector.getMinLongitude().radians;
      double maxLon = sector.getMaxLongitude().radians;
      double deltaLat = ( maxLat - minLat ) / ( numLat > 1 ? numLat - 1 : 1 );
      double deltaLon = ( maxLon - minLon ) / ( numLon > 1 ? numLon - 1 : 1 );
      int pos = 0;

      // Compute the cosine and sine of each longitude value. This eliminates the need to re-compute the same values
      // for each row of constant latitude (and varying longitude).
      double[] cosLon = new double[numLon];
      double[] sinLon = new double[numLon];
      double lon = minLon;
      for ( int i = 0; i < numLon; i++, lon += deltaLon )
      {
        if ( i == numLon - 1 ) // explicitly set the last lon to the max longitude to ensure alignment
          lon = maxLon;

        cosLon[i] = Math.Cos( lon );
        sinLon[i] = Math.Sin( lon );
      }

      // Iterate over the latitude and longitude coordinates in the specified sector, computing the Cartesian point
      // corresponding to each latitude and longitude.
      double lat = minLat;
      for ( int j = 0; j < numLat; j++, lat += deltaLat )
      {
        if ( j == numLat - 1 ) // explicitly set the last lat to the max latitude to ensure alignment
          lat = maxLat;

        // Latitude is constant for each row. Values that are a function of latitude can be computed once per row.
        double cosLat = Math.Cos( lat );
        double sinLat = Math.Sin( lat );
        double rpm = this.equatorialRadius / Math.Sqrt( 1.0 - this.es * sinLat * sinLat );

        for ( int i = 0; i < numLon; i++ )
        {
          double elev = metersElevation[pos];
          double x = ( rpm + elev ) * cosLat * sinLon[i];
          double y = ( rpm * ( 1.0 - this.es ) + elev ) * sinLat;
          double z = ( rpm + elev ) * cosLat * cosLon[i];
          outVector[pos++] = new Vec4( x, y, z );
        }
      }
    }    

    /**
     * Compute the geographic position to corresponds to a Cartesian point.
     *
     * @param cart Cartesian point to convert to geographic.
     *
     * @return The geographic position of {@code cart}.
     *
     * @see #geodeticToCartesian(gov.nasa.worldwind.geom.Angle, SharpEarth.geom.Angle, double)
     */
    protected Position cartesianToGeodetic( Vec4 cart )
    {
      return ellipsoidalToGeodetic( cart );
    }

    /**
     * Compute the geographic position to corresponds to an ellipsoidal point.
     *
     * @param cart Ellipsoidal point to convert to geographic.
     *
     * @return The geographic position of {@code cart}.
     *
     * @see #geodeticToEllipsoidal(gov.nasa.worldwind.geom.Angle, SharpEarth.geom.Angle, double)
     */
    protected Position ellipsoidalToGeodetic( Vec4 cart )
    {
      // Contributed by Nathan Kronenfeld. Integrated 1/24/2011. Brings this calculation in line with Vermeille's
      // most recent update.
      if ( null == cart )
      {
        string message = Logging.getMessage( "nullValue.PointIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // According to
      // H. Vermeille,
      // "An analytical method to transform geocentric into geodetic coordinates"
      // http://www.springerlink.com/content/3t6837t27t351227/fulltext.pdf
      // Journal of Geodesy, accepted 10/2010, not yet published
      double X = cart.z();
      double Y = cart.x();
      double Z = cart.y();
      double XXpYY = X * X + Y * Y;
      double sqrtXXpYY = Math.Sqrt( XXpYY );

      double a = this.equatorialRadius;
      double ra2 = 1 / ( a * a );
      double e2 = this.es;
      double e4 = e2 * e2;

      // Step 1
      double p = XXpYY * ra2;
      double q = Z * Z * ( 1 - e2 ) * ra2;
      double r = ( p + q - e4 ) / 6;

      double h;
      double phi;

      double evoluteBorderTest = 8 * r * r * r + e4 * p * q;
      if ( evoluteBorderTest > 0 || q != 0 )
      {
        double u;

        if ( evoluteBorderTest > 0 )
        {
          // Step 2: general case
          double rad1 = Math.Sqrt( evoluteBorderTest );
          double rad2 = Math.Sqrt( e4 * p * q );

          // 10*e2 is my arbitrary decision of what Vermeille means by "near... the cusps of the evolute".
          if ( evoluteBorderTest > 10 * e2 )
          {
            double rad3 = Math.Pow( ( rad1 + rad2 ) * ( rad1 + rad2 ), (1.0 / 3.0) );
            u = r + 0.5 * rad3 + 2 * r * r / rad3;
          }
          else
          {
            u = r + 0.5 * Math.Pow( ( rad1 + rad2 ) * ( rad1 + rad2 ), (1.0 / 3.0) ) + 0.5 * Math.Pow(
              ( rad1 - rad2 ) * ( rad1 - rad2 ), (1.0 / 3.0) );
          }
        }
        else
        {
          // Step 3: near evolute
          double rad1 = Math.Sqrt( -evoluteBorderTest );
          double rad2 = Math.Sqrt( -8 * r * r * r );
          double rad3 = Math.Sqrt( e4 * p * q );
          double atan = 2 * Math.Atan2( rad3, rad1 + rad2 ) / 3;

          u = -4 * r * Math.Sin( atan ) * Math.Cos( Math.PI / 6 + atan );
        }

        double v = Math.Sqrt( u * u + e4 * q );
        double w = e2 * ( u + v - q ) / ( 2 * v );
        double k = ( u + v ) / ( Math.Sqrt( w * w + u + v ) + w );
        double D = k * sqrtXXpYY / ( k + e2 );
        double sqrtDDpZZ = Math.Sqrt( D * D + Z * Z );

        h = ( k + e2 - 1 ) * sqrtDDpZZ / k;
        phi = 2 * Math.Atan2( Z, sqrtDDpZZ + D );
      }
      else
      {
        // Step 4: singular disk
        double rad1 = Math.Sqrt( 1 - e2 );
        double rad2 = Math.Sqrt( e2 - p );
        double e = Math.Sqrt( e2 );

        h = -a * rad1 * rad2 / e;
        phi = rad2 / ( e * rad2 + rad1 * Math.Sqrt( p ) );
      }

      // Compute lambda
      double lambda;
      double s2 = Math.Sqrt( 2 );
      if ( ( s2 - 1 ) * Y < sqrtXXpYY + X )
      {
        // case 1 - -135deg < lambda < 135deg
        lambda = 2 * Math.Atan2( Y, sqrtXXpYY + X );
      }
      else if ( sqrtXXpYY + Y < ( s2 + 1 ) * X )
      {
        // case 2 - -225deg < lambda < 45deg
        lambda = -Math.PI * 0.5 + 2 * Math.Atan2( X, sqrtXXpYY - Y );
      }
      else
      {
        // if (sqrtXXpYY-Y<(s2=1)*X) {  // is the test, if needed, but it's not
        // case 3: - -45deg < lambda < 225deg
        lambda = Math.PI * 0.5 - 2 * Math.Atan2( X, sqrtXXpYY + Y );
      }

      return Position.fromRadians( phi, lambda, h );
    }

    public SectorGeometryList tessellate( DrawContext dc )
    {
      if ( _tessellator == null )
      {
        _tessellator = (Tessellator)WorldWind.createConfigurationComponent( AVKey.TESSELLATOR_CLASS_NAME );

        if ( _tessellator == null )
        {
          string message = Logging.getMessage( "Tessellator.TessellatorUnavailable" );
          Logging.logger().severe( message );
          throw new IllegalStateException( message );
        }
      }

      return _tessellator.tessellate( dc );
    }

    /**
     * Determines whether a point is above a given elevation
     *
     * @param point     the <code>Vec4</code> point to test.
     * @param elevation the elevation to test for.
     *
     * @return true if the given point is above the given elevation.
     */
    public bool isPointAboveElevation( Vec4 point, double elevation )
    {
      //noinspection SimplifiableIfStatement
      if ( point == null )
        return false;

      return ( point.x() * point.x() ) / ( ( this.equatorialRadius + elevation ) * ( this.equatorialRadius + elevation ) )
             + ( point.y() * point.y() ) / ( ( this.polarRadius + elevation ) * ( this.polarRadius + elevation ) )
             + ( point.z() * point.z() ) / ( ( this.equatorialRadius + elevation ) * ( this.equatorialRadius + elevation ) )
             - 1 > 0;
    }

    /**
     * Construct an elevation model given a key for a configuration source and the source's default value.
     *
     * @param key          the key identifying the configuration property in {@link Configuration}.
     * @param defaultValue the default value of the property to use if it's not found in {@link Configuration}.
     *
     * @return a new elevation model configured according to the configuration source.
     */
    public static ElevationModel makeElevationModel( string key, string defaultValue )
    {
      if ( key == null )
      {
        String msg = Logging.getMessage( "nullValue.KeyIsNull" );
        throw new ArgumentException( msg );
      }

      object configSource = Configuration.getStringValue( key, defaultValue );
      return (ElevationModel)BasicFactory.create( AVKey.ELEVATION_MODEL_FACTORY, configSource );
    }
  }
}
