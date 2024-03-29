/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.avlist;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpEarth.geom
{



  /**
   * Represents a point on the two-dimensional surface of a globe. Latitude is the degrees North and ranges between [-90,
   * 90], while longitude refers to degrees East, and ranges between (-180, 180].
   * <p/>
   * Instances of <code>LatLon</code> are immutable.
   *
   * @author Tom Gaskins
   * @version $Id: LatLon.java 1171 2013-02-11 21:45:02Z dcollins $
   */
  public class LatLon
  {
    public readonly static LatLon ZERO = new LatLon( Angle.ZERO, Angle.ZERO );
    public readonly static double RADIANS_TO_DEGREES = 180.0 / Math.PI;
    public readonly static double DEGREES_TO_RADIANS = Math.PI / 180.0;
    public readonly Angle latitude;
    public readonly Angle longitude;

    /**
     * Factor method for obtaining a new <code>LatLon</code> from two angles expressed in radians.
     *
     * @param latitude  in radians
     * @param longitude in radians
     *
     * @return a new <code>LatLon</code> from the given angles, which are expressed as radians
     */
    public static LatLon fromRadians( double latitude, double longitude )
    {
      return new LatLon( latitude * RADIANS_TO_DEGREES, longitude * RADIANS_TO_DEGREES );
    }

    /**
     * Factory method for obtaining a new <code>LatLon</code> from two angles expressed in degrees.
     *
     * @param latitude  in degrees
     * @param longitude in degrees
     *
     * @return a new <code>LatLon</code> from the given angles, which are expressed as degrees
     */
    public static LatLon fromDegrees( double latitude, double longitude )
    {
      return new LatLon( latitude, longitude );
    }

    private LatLon( double latitude, double longitude )
    {
      this.latitude = Angle.fromDegrees( latitude );
      this.longitude = Angle.fromDegrees( longitude );
    }

    /**
     * Constructs a new  <code>LatLon</code> from two angles. Neither angle may be null.
     *
     * @param latitude  latitude
     * @param longitude longitude
     *
     * @throws ArgumentException if <code>latitude</code> or <code>longitude</code> is null
     */
    public LatLon( Angle latitude, Angle longitude )
    {
      if ( latitude == null || longitude == null )
      {
        string message = Logging.getMessage( "nullValue.LatitudeOrLongitudeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      this.latitude = latitude;
      this.longitude = longitude;
    }

    public LatLon( LatLon latLon )
    {
      if ( latLon == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      this.latitude = latLon.latitude;
      this.longitude = latLon.longitude;
    }

    /**
     * Obtains the latitude of this <code>LatLon</code>.
     *
     * @return this <code>LatLon</code>'s latitude
     */
    public Angle getLatitude()
    {
      return this.latitude;
    }

    /**
     * Obtains the longitude of this <code>LatLon</code>.
     *
     * @return this <code>LatLon</code>'s longitude
     */
    public Angle getLongitude()
    {
      return this.longitude;
    }

    /**
     * Returns an array of this object's latitude and longitude in degrees.
     *
     * @return the array of latitude and longitude, arranged in that order.
     */
    public double[] asDegreesArray()
    {
      return new double[] { this.getLatitude().degrees, this.getLongitude().degrees };
    }

    /**
     * Returns an array of this object's latitude and longitude in radians.
     *
     * @return the array of latitude and longitude, arranged in that order.
     */
    public double[] asRadiansArray()
    {
      return new double[] { this.getLatitude().radians, this.getLongitude().radians };
    }

    /**
     * Returns an interpolated location between <code>value1</code> and <code>value2</code>, according to the specified
     * path type. If the path type is {@link AVKey#GREAT_CIRCLE} this returns an interpolated value on the great arc
     * that spans the two locations (see {@link #interpolateGreatCircle(double, LatLon, LatLon)}). If the path type is
     * {@link AVKey#RHUMB_LINE} or {@link AVKey#LOXODROME} this returns an interpolated value on the rhumb line that
     * spans the two locations (see {@link #interpolateRhumb(double, LatLon, LatLon)}. Otherwise, this returns the
     * linear interpolation of the two locations (see {@link #interpolate(double, LatLon, LatLon)}.
     *
     * @param pathType the path type used to interpolate between geographic locations.
     * @param amount   the interpolation factor
     * @param value1   the first location.
     * @param value2   the second location.
     *
     * @return an interpolated location between <code>value1</code> and <code>value2</code>, according to the specified
     *         path type.
     *
     * @throws ArgumentException if the path type or either location is null.
     */
    public static LatLon interpolate( string pathType, double amount, LatLon value1, LatLon value2 )
    {
      if ( pathType == null )
      {
        string message = Logging.getMessage( "nullValue.PathTypeIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( value1 == null || value2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( pathType.Equals( AVKey.GREAT_CIRCLE ) )
      {
        return interpolateGreatCircle( amount, value1, value2 );
      }
      else if ( pathType.Equals( AVKey.RHUMB_LINE ) || pathType.Equals( AVKey.LOXODROME ) )
      {
        return interpolateRhumb( amount, value1, value2 );
      }
      else // Default to linear interpolation.
      {
        return interpolate( amount, value1, value2 );
      }
    }

    /**
     * Returns the linear interpolation of <code>value1</code> and <code>value2</code>, treating the geographic
     * locations as simple 2D coordinate pairs.
     *
     * @param amount the interpolation factor
     * @param value1 the first location.
     * @param value2 the second location.
     *
     * @return the linear interpolation of <code>value1</code> and <code>value2</code>.
     *
     * @throws ArgumentException if either location is null.
     */
    public static LatLon interpolate( double amount, LatLon value1, LatLon value2 )
    {
      if ( value1 == null || value2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( LatLon.Equals( value1, value2 ) )
        return value1;

      Line line;
      try
      {
        line = Line.fromSegment(
            new Vec4( value1.getLongitude().radians, value1.getLatitude().radians, 0 ),
            new Vec4( value2.getLongitude().radians, value2.getLatitude().radians, 0 ) );
      }
      catch ( ArgumentException e )
      {
        // Locations became coincident after calculations.
        return value1;
      }

      Vec4 p = line.getPointAt( amount );

      return LatLon.fromRadians( p.x(), p.y() );
    }

    /**
     * Returns the an interpolated location along the great-arc between <code>value1</code> and <code>value2</code>. The
     * interpolation factor <code>amount</code> defines the weight given to each value, and is clamped to the range [0,
     * 1]. If <code>a</code> is 0 or less, this returns <code>value1</code>. If <code>amount</code> is 1 or more, this
     * returns <code>value2</code>. Otherwise, this returns the location on the great-arc between <code>value1</code>
     * and <code>value2</code> corresponding to the specified interpolation factor.
     *
     * @param amount the interpolation factor
     * @param value1 the first location.
     * @param value2 the second location.
     *
     * @return an interpolated location along the great-arc between <code>value1</code> and <code>value2</code>.
     *
     * @throws ArgumentException if either location is null.
     */
    public static LatLon interpolateGreatCircle( double amount, LatLon value1, LatLon value2 )
    {
      if ( value1 == null || value2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( LatLon.Equals( value1, value2 ) )
        return value1;

      double t = WWMath.clamp( amount, 0d, 1d );
      Angle azimuth = LatLon.greatCircleAzimuth( value1, value2 );
      Angle distance = LatLon.greatCircleDistance( value1, value2 );
      Angle pathLength = Angle.fromDegrees( t * distance.degrees );

      return LatLon.greatCircleEndPosition( value1, azimuth, pathLength );
    }

    /**
     * Returns the an interpolated location along the rhumb line between <code>value1</code> and <code>value2</code>.
     * The interpolation factor <code>amount</code> defines the weight given to each value, and is clamped to the range
     * [0, 1]. If <code>a</code> is 0 or less, this returns <code>value1</code>. If <code>amount</code> is 1 or more,
     * this returns <code>value2</code>. Otherwise, this returns the location on the rhumb line between
     * <code>value1</code> and <code>value2</code> corresponding to the specified interpolation factor.
     *
     * @param amount the interpolation factor
     * @param value1 the first location.
     * @param value2 the second location.
     *
     * @return an interpolated location along the rhumb line between <code>value1</code> and <code>value2</code>
     *
     * @throws ArgumentException if either location is null.
     */
    public static LatLon interpolateRhumb( double amount, LatLon value1, LatLon value2 )
    {
      if ( value1 == null || value2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( LatLon.Equals( value1, value2 ) )
        return value1;

      double t = WWMath.clamp( amount, 0d, 1d );
      Angle azimuth = LatLon.rhumbAzimuth( value1, value2 );
      Angle distance = LatLon.rhumbDistance( value1, value2 );
      Angle pathLength = Angle.fromDegrees( t * distance.degrees );

      return LatLon.rhumbEndPosition( value1, azimuth, pathLength );
    }

    /**
     * Computes the great circle angular distance between two locations. The return value gives the distance as the
     * angle between the two positions on the pi radius circle. In radians, this angle is also the arc length of the
     * segment between the two positions on that circle. To compute a distance in meters from this value, multiply it by
     * the radius of the globe.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return the angular distance between the two locations. In radians, this value is the arc length on the radius pi
     *         circle.
     */
    public static Angle greatCircleDistance( LatLon p1, LatLon p2 )
    {
      if ( (p1 == null) || (p2 == null) )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      // "Haversine formula," taken from http://en.wikipedia.org/wiki/Great-circle_distance#Formul.C3.A6
      double a = Math.Sin( (lat2 - lat1) / 2.0 );
      double b = Math.Sin( (lon2 - lon1) / 2.0 );
      double c = a * a + +Math.Cos( lat1 ) * Math.Cos( lat2 ) * b * b;
      double distanceRadians = 2.0 * Math.Asin( Math.Sqrt( c ) );

      return Double.IsNaN( distanceRadians ) ? Angle.ZERO : Angle.fromRadians( distanceRadians );
    }

    /**
     * Computes the azimuth angle (clockwise from North) that points from the first location to the second location.
     * This angle can be used as the starting azimuth for a great circle arc that begins at the first location, and
     * passes through the second location.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return Angle that points from the first location to the second location.
     */
    public static Angle greatCircleAzimuth( LatLon p1, LatLon p2 )
    {
      if ( (p1 == null) || (p2 == null) )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      if ( lon1 == lon2 )
        return lat1 > lat2 ? Angle.POS180 : Angle.ZERO;

      // Taken from "Map Projections - A Working Manual", page 30, equation 5-4b.
      // The atan2() function is used in place of the traditional atan(y/x) to simplify the case when x==0.
      double y = Math.Cos( lat2 ) * Math.Sin( lon2 - lon1 );
      double x = Math.Cos( lat1 ) * Math.Sin( lat2 ) - Math.Sin( lat1 ) * Math.Cos( lat2 ) * Math.Cos( lon2 - lon1 );
      double azimuthRadians = Math.Atan2( y, x );

      return Double.IsNaN( azimuthRadians ) ? Angle.ZERO : Angle.fromRadians( azimuthRadians );
    }

    /**
     * Computes the location on a great circle arc with the given starting location, azimuth, and arc distance.
     *
     * @param p                  LatLon of the starting location
     * @param greatCircleAzimuth great circle azimuth angle (clockwise from North)
     * @param pathLength         arc distance to travel
     *
     * @return LatLon location on the great circle arc.
     */
    public static LatLon greatCircleEndPosition( LatLon p, Angle greatCircleAzimuth, Angle pathLength )
    {
      if ( p == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( greatCircleAzimuth == null || pathLength == null )
      {
        string message = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat = p.getLatitude().radians;
      double lon = p.getLongitude().radians;
      double azimuth = greatCircleAzimuth.radians;
      double distance = pathLength.radians;

      if ( distance == 0 )
        return p;

      // Taken from "Map Projections - A Working Manual", page 31, equation 5-5 and 5-6.
      double endLatRadians = Math.Asin( Math.Sin( lat ) * Math.Cos( distance )
          + Math.Cos( lat ) * Math.Sin( distance ) * Math.Cos( azimuth ) );
      double endLonRadians = lon + Math.Atan2(
          Math.Sin( distance ) * Math.Sin( azimuth ),
          Math.Cos( lat ) * Math.Cos( distance ) - Math.Sin( lat ) * Math.Sin( distance ) * Math.Cos( azimuth ) );

      if ( Double.IsNaN( endLatRadians ) || Double.IsNaN( endLonRadians ) )
        return p;

      return new LatLon(
          Angle.fromRadians( endLatRadians ).normalizedLatitude(),
          Angle.fromRadians( endLonRadians ).normalizedLongitude() );
    }

    /**
     * Computes the location on a great circle arc with the given starting location, azimuth, and arc distance.
     *
     * @param p                         LatLon of the starting location
     * @param greatCircleAzimuthRadians great circle azimuth angle (clockwise from North), in radians
     * @param pathLengthRadians         arc distance to travel, in radians
     *
     * @return LatLon location on the great circle arc.
     */
    public static LatLon greatCircleEndPosition( LatLon p, double greatCircleAzimuthRadians, double pathLengthRadians )
    {
      if ( p == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return greatCircleEndPosition( p,
          Angle.fromRadians( greatCircleAzimuthRadians ), Angle.fromRadians( pathLengthRadians ) );
    }

    /**
     * Returns two locations with the most extreme latitudes on the great circle with the given starting location and
     * azimuth.
     *
     * @param location location on the great circle.
     * @param azimuth  great circle azimuth angle (clockwise from North).
     *
     * @return two locations where the great circle has its extreme latitudes.
     *
     * @throws ArgumentException if either <code>location</code> or <code>azimuth</code> are null.
     */
    public static LatLon[] greatCircleExtremeLocations( LatLon location, Angle azimuth )
    {
      if ( location == null )
      {
        string message = Logging.getMessage( "nullValue.LocationIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( azimuth == null )
      {
        string message = Logging.getMessage( "nullValue.AzimuthIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat0 = location.getLatitude().radians;
      double az = azimuth.radians;

      // Derived by solving the function for longitude on a great circle against the desired longitude. We start with
      // the equation in "Map Projections - A Working Manual", page 31, equation 5-5:
      //
      // lat = asin( sin(lat0) * cos(c) + cos(lat0) * sin(c) * cos(Az) )
      //
      // Where (lat0, lon) are the starting coordinates, c is the angular distance along the great circle from the
      // starting coordinate, and Az is the azimuth. All values are in radians.
      //
      // Solving for angular distance gives distance to the equator:
      //
      // tan(c) = -tan(lat0) / cos(Az)
      //
      // The great circle is by definition centered about the Globe's origin. Therefore intersections with the
      // equator will be antipodal (exactly 180 degrees opposite each other), as will be the extreme latitudes.
      // By observing the symmetry of a great circle, it is also apparent that the extreme latitudes will be 90
      // degrees from either intersection with the equator.
      //
      // d1 = c + 90
      // d2 = c - 90

      double tanDistance = -Math.Tan( lat0 ) / Math.Cos( az );
      double distance = Math.Atan( tanDistance );

      Angle extremeDistance1 = Angle.fromRadians( distance + (Math.PI / 2.0) );
      Angle extremeDistance2 = Angle.fromRadians( distance - (Math.PI / 2.0) );

      return new LatLon[]
          {
                greatCircleEndPosition(location, azimuth, extremeDistance1),
                greatCircleEndPosition(location, azimuth, extremeDistance2)
          };
    }

    /**
     * Returns two locations with the most extreme latitudes on the great circle arc defined by, and limited to, the two
     * locations.
     *
     * @param begin beginning location on the great circle arc.
     * @param end   ending location on the great circle arc.
     *
     * @return two locations with the most extreme latitudes on the great circle arc.
     *
     * @throws ArgumentException if either <code>begin</code> or <code>end</code> are null.
     */
    public static LatLon[] greatCircleArcExtremeLocations( LatLon begin, LatLon end )
    {
      if ( begin == null )
      {
        string message = Logging.getMessage( "nullValue.BeginIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( end == null )
      {
        string message = Logging.getMessage( "nullValue.EndIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      LatLon minLatLocation = null;
      LatLon maxLatLocation = null;
      double minLat = Angle.POS90.degrees;
      double maxLat = Angle.NEG90.degrees;

      // Compute the min and max latitude and associated locations from the arc endpoints.
      foreach ( LatLon ll in new[] { begin, end } )
      {
        if ( minLat >= ll.getLatitude().degrees )
        {
          minLat = ll.getLatitude().degrees;
          minLatLocation = ll;
        }
        if ( maxLat <= ll.getLatitude().degrees )
        {
          maxLat = ll.getLatitude().degrees;
          maxLatLocation = ll;
        }
      }

      // Compute parameters for the great circle arc defined by begin and end. Then compute the locations of extreme
      // latitude on entire the great circle which that arc is part of.
      Angle greatArcAzimuth = greatCircleAzimuth( begin, end );
      Angle greatArcDistance = greatCircleDistance( begin, end );
      LatLon[] greatCircleExtremes = greatCircleExtremeLocations( begin, greatArcAzimuth );

      // Determine whether either of the extreme locations are inside the arc defined by begin and end. If so,
      // adjust the min and max latitude accordingly.
      foreach ( LatLon ll in greatCircleExtremes )
      {
        Angle az = LatLon.greatCircleAzimuth( begin, ll );
        Angle d = LatLon.greatCircleDistance( begin, ll );

        // The extreme location must be between the begin and end locations. Therefore its azimuth relative to
        // the begin location should have the same signum, and its distance relative to the begin location should
        // be between 0 and greatArcDistance, inclusive.
        if ( Math.Sign( az.degrees ) == Math.Sign( greatArcAzimuth.degrees ) )
        {
          if ( d.degrees >= 0 && d.degrees <= greatArcDistance.degrees )
          {
            if ( minLat >= ll.getLatitude().degrees )
            {
              minLat = ll.getLatitude().degrees;
              minLatLocation = ll;
            }
            if ( maxLat <= ll.getLatitude().degrees )
            {
              maxLat = ll.getLatitude().degrees;
              maxLatLocation = ll;
            }
          }
        }
      }

      return new LatLon[] { minLatLocation, maxLatLocation };
    }

    /**
     * Returns two locations with the most extreme latitudes on the sequence of great circle arcs defined by each pair
     * of locations in the specified iterable.
     *
     * @param locations the pairs of locations defining a sequence of great circle arcs.
     *
     * @return two locations with the most extreme latitudes on the great circle arcs.
     *
     * @throws ArgumentException if <code>locations</code> is null.
     */
    public static LatLon[] greatCircleArcExtremeLocations( IEnumerable<LatLon> locations )
    {
      if ( locations == null )
      {
        string message = Logging.getMessage( "nullValue.LocationsListIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      LatLon minLatLocation = null;
      LatLon maxLatLocation = null;

      LatLon lastLocation = null;

      foreach ( LatLon ll in locations )
      {
        if ( lastLocation != null )
        {
          LatLon[] extremes = LatLon.greatCircleArcExtremeLocations( lastLocation, ll );
          if ( extremes == null )
            continue;

          if ( minLatLocation == null || minLatLocation.getLatitude().degrees > extremes[0].getLatitude().degrees )
            minLatLocation = extremes[0];
          if ( maxLatLocation == null || maxLatLocation.getLatitude().degrees < extremes[1].getLatitude().degrees )
            maxLatLocation = extremes[1];
        }

        lastLocation = ll;
      }

      return new LatLon[] { minLatLocation, maxLatLocation };
    }

    /**
     * Computes the length of the rhumb line between two locations. The return value gives the distance as the angular
     * distance between the two positions on the pi radius circle. In radians, this angle is also the arc length of the
     * segment between the two positions on that circle. To compute a distance in meters from this value, multiply it by
     * the radius of the globe.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return the arc length of the rhumb line between the two locations. In radians, this value is the arc length on
     *         the radius pi circle.
     */
    public static Angle rhumbDistance( LatLon p1, LatLon p2 )
    {
      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      // Taken from http://www.movable-type.co.uk/scripts/latlong.html
      double dLat = lat2 - lat1;
      double dLon = lon2 - lon1;
      double dPhi = Math.Log( Math.Tan( lat2 / 2.0 + Math.PI / 4.0 ) / Math.Tan( lat1 / 2.0 + Math.PI / 4.0 ) );
      double q = dLat / dPhi;
      if ( Double.IsNaN( dPhi ) || Double.IsNaN( q ) )
      {
        q = Math.Cos( lat1 );
      }
      // If lonChange over 180 take shorter rhumb across 180 meridian.
      if ( Math.Abs( dLon ) > Math.PI )
      {
        dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
      }

      double distanceRadians = Math.Sqrt( dLat * dLat + q * q * dLon * dLon );

      return Double.IsNaN( distanceRadians ) ? Angle.ZERO : Angle.fromRadians( distanceRadians );
    }

    /**
     * Computes the azimuth angle (clockwise from North) of a rhumb line (a line of constant heading) between two
     * locations.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return azimuth Angle of a rhumb line between the two locations.
     */
    public static Angle rhumbAzimuth( LatLon p1, LatLon p2 )
    {
      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      // Taken from http://www.movable-type.co.uk/scripts/latlong.html
      double dLon = lon2 - lon1;
      double dPhi = Math.Log( Math.Tan( lat2 / 2.0 + Math.PI / 4.0 ) / Math.Tan( lat1 / 2.0 + Math.PI / 4.0 ) );
      // If lonChange over 180 take shorter rhumb across 180 meridian.
      if ( Math.Abs( dLon ) > Math.PI )
      {
        dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
      }
      double azimuthRadians = Math.Atan2( dLon, dPhi );

      return Double.IsNaN( azimuthRadians ) ? Angle.ZERO : Angle.fromRadians( azimuthRadians );
    }

    /**
     * Computes the location on a rhumb line with the given starting location, rhumb azimuth, and arc distance along the
     * line.
     *
     * @param p            LatLon of the starting location
     * @param rhumbAzimuth rhumb azimuth angle (clockwise from North)
     * @param pathLength   arc distance to travel
     *
     * @return LatLon location on the rhumb line.
     */
    public static LatLon rhumbEndPosition( LatLon p, Angle rhumbAzimuth, Angle pathLength )
    {
      if ( p == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( rhumbAzimuth == null || pathLength == null )
      {
        string message = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p.getLatitude().radians;
      double lon1 = p.getLongitude().radians;
      double azimuth = rhumbAzimuth.radians;
      double distance = pathLength.radians;

      if ( distance == 0 )
        return p;

      // Taken from http://www.movable-type.co.uk/scripts/latlong.html
      double lat2 = lat1 + distance * Math.Cos( azimuth );
      double dPhi = Math.Log( Math.Tan( lat2 / 2.0 + Math.PI / 4.0 ) / Math.Tan( lat1 / 2.0 + Math.PI / 4.0 ) );
      double q = (lat2 - lat1) / dPhi;
      if ( Double.IsNaN( dPhi ) || Double.IsNaN( q ) || Double.IsInfinity( q ) )
      {
        q = Math.Cos( lat1 );
      }
      double dLon = distance * Math.Sin( azimuth ) / q;
      // Handle latitude passing over either pole.
      if ( Math.Abs( lat2 ) > Math.PI / 2.0 )
      {
        lat2 = lat2 > 0 ? Math.PI - lat2 : -Math.PI - lat2;
      }
      double lon2 = (lon1 + dLon + Math.PI) % (2 * Math.PI) - Math.PI;

      if ( Double.IsNaN( lat2 ) || Double.IsNaN( lon2 ) )
        return p;

      return new LatLon(
          Angle.fromRadians( lat2 ).normalizedLatitude(),
          Angle.fromRadians( lon2 ).normalizedLongitude() );
    }

    /**
     * Computes the location on a rhumb line with the given starting location, rhumb azimuth, and arc distance along the
     * line.
     *
     * @param p                   LatLon of the starting location
     * @param rhumbAzimuthRadians rhumb azimuth angle (clockwise from North), in radians
     * @param pathLengthRadians   arc distance to travel, in radians
     *
     * @return LatLon location on the rhumb line.
     */
    public static LatLon rhumbEndPosition( LatLon p, double rhumbAzimuthRadians, double pathLengthRadians )
    {
      if ( p == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      return rhumbEndPosition( p, Angle.fromRadians( rhumbAzimuthRadians ), Angle.fromRadians( pathLengthRadians ) );
    }

    /**
     * Computes the length of the linear path between two locations. The return value gives the distance as the angular
     * distance between the two positions on the pi radius circle. In radians, this angle is also the arc length of the
     * segment between the two positions on that circle. To compute a distance in meters from this value, multiply it by
     * the radius of the globe.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return the arc length of the line between the two locations. In radians, this value is the arc length on the
     *         radius pi circle.
     */
    public static Angle linearDistance( LatLon p1, LatLon p2 )
    {
      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      double dLat = lat2 - lat1;
      double dLon = lon2 - lon1;

      // If lonChange over 180 take shorter path across 180 meridian.
      if ( Math.Abs( dLon ) > Math.PI )
      {
        dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
      }

      double distanceRadians = Math.Sqrt( dLat * dLat + dLon * dLon );

      return Double.IsNaN( distanceRadians ) ? Angle.ZERO : Angle.fromRadians( distanceRadians );
    }

    /**
     * Computes the azimuth angle (clockwise from North) of a linear path two locations.
     *
     * @param p1 LatLon of the first location
     * @param p2 LatLon of the second location
     *
     * @return azimuth Angle of a linear path between the two locations.
     */
    public static Angle linearAzimuth( LatLon p1, LatLon p2 )
    {
      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p1.getLatitude().radians;
      double lon1 = p1.getLongitude().radians;
      double lat2 = p2.getLatitude().radians;
      double lon2 = p2.getLongitude().radians;

      if ( lat1 == lat2 && lon1 == lon2 )
        return Angle.ZERO;

      double dLon = lon2 - lon1;
      double dLat = lat2 - lat1;

      // If lonChange over 180 take shorter rhumb across 180 meridian.
      if ( Math.Abs( dLon ) > Math.PI )
      {
        dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
      }
      double azimuthRadians = Math.Atan2( dLon, dLat );

      return Double.IsNaN( azimuthRadians ) ? Angle.ZERO : Angle.fromRadians( azimuthRadians );
    }

    /**
     * Computes the location on a linear path given a starting location, azimuth, and arc distance along the line. A
     * linear path is determined by treating latitude and longitude as a rectangular grid. This type of path is a
     * straight line in the equidistant cylindrical map projection (also called equirectangular).
     *
     * @param p             LatLon of the starting location
     * @param linearAzimuth azimuth angle (clockwise from North)
     * @param pathLength    arc distance to travel
     *
     * @return LatLon location on the line.
     */
    public static LatLon linearEndPosition( LatLon p, Angle linearAzimuth, Angle pathLength )
    {
      if ( p == null )
      {
        string message = Logging.getMessage( "nullValue.LatLonIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      if ( linearAzimuth == null || pathLength == null )
      {
        string message = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      double lat1 = p.getLatitude().radians;
      double lon1 = p.getLongitude().radians;
      double azimuth = linearAzimuth.radians;
      double distance = pathLength.radians;

      if ( distance == 0 )
        return p;

      double lat2 = lat1 + distance * Math.Cos( azimuth );

      // Handle latitude passing over either pole.
      if ( Math.Abs( lat2 ) > Math.PI / 2.0 )
      {
        lat2 = lat2 > 0 ? Math.PI - lat2 : -Math.PI - lat2;
      }
      double lon2 = (lon1 + distance * Math.Sin( azimuth ) + Math.PI) % (2 * Math.PI) - Math.PI;

      if ( Double.IsNaN( lat2 ) || Double.IsNaN( lon2 ) )
        return p;

      return new LatLon(
          Angle.fromRadians( lat2 ).normalizedLatitude(),
          Angle.fromRadians( lon2 ).normalizedLongitude() );
    }

    /**
     * Compute the average rhumb distance between locations.
     *
     * @param locations Locations of which to compute average.
     *
     * @return Average rhumb line distance between locations, as an angular distance.
     */
    public static Angle getAverageDistance( IEnumerable<LatLon> locations )
    {
      if ( (locations == null) )
      {
        string msg = Logging.getMessage( "nullValue.LocationsListIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      double totalDistance = 0.0;
      int count = 0;

      foreach ( LatLon p1 in locations )
      {
        foreach ( LatLon p2 in locations )
        {
          if ( p1 != p2 )
          {
            double d = rhumbDistance( p1, p2 ).radians;
            totalDistance += d;
            count++;
          }
        }
      }

      return (count == 0) ? Angle.ZERO : Angle.fromRadians( totalDistance / (double)count );
    }

    public LatLon add( LatLon that )
    {
      if ( that == null )
      {
        string msg = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      Angle lat = Angle.normalizedLatitude( this.latitude.add( that.latitude ) );
      Angle lon = Angle.normalizedLongitude( this.longitude.add( that.longitude ) );

      return new LatLon( lat, lon );
    }

    public LatLon subtract( LatLon that )
    {
      if ( that == null )
      {
        string msg = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      Angle lat = Angle.normalizedLatitude( this.latitude.subtract( that.latitude ) );
      Angle lon = Angle.normalizedLongitude( this.longitude.subtract( that.longitude ) );

      return new LatLon( lat, lon );
    }

    public LatLon add( Position that )
    {
      if ( that == null )
      {
        string msg = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      Angle lat = Angle.normalizedLatitude( this.latitude.add( that.getLatitude() ) );
      Angle lon = Angle.normalizedLongitude( this.longitude.add( that.getLongitude() ) );

      return new LatLon( lat, lon );
    }

    public LatLon subtract( Position that )
    {
      if ( that == null )
      {
        string msg = Logging.getMessage( "nullValue.AngleIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      Angle lat = Angle.normalizedLatitude( this.latitude.subtract( that.getLatitude() ) );
      Angle lon = Angle.normalizedLongitude( this.longitude.subtract( that.getLongitude() ) );

      return new LatLon( lat, lon );
    }

    public static bool locationsCrossDateLine( IEnumerable<LatLon> locations )
    {
      if ( locations == null )
      {
        string msg = Logging.getMessage( "nullValue.LocationsListIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      LatLon pos = null;
      foreach ( LatLon posNext in locations )
      {
        if ( pos != null )
        {
          // A segment cross the line if end pos have different longitude signs
          // and are more than 180 degrees longitude apart
          if ( Math.Sign( pos.getLongitude().degrees ) != Math.Sign( posNext.getLongitude().degrees ) )
          {
            double delta = Math.Abs( pos.getLongitude().degrees - posNext.getLongitude().degrees );
            if ( delta > 180 && delta < 360 )
              return true;
          }
        }
        pos = posNext;
      }

      return false;
    }

    public static bool locationsCrossDateline( LatLon p1, LatLon p2 )
    {
      if ( p1 == null || p2 == null )
      {
        string msg = Logging.getMessage( "nullValue.LocationIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      // A segment cross the line if end pos have different longitude signs
      // and are more than 180 degrees longitude apart
      if ( Math.Sign( p1.getLongitude().degrees ) != Math.Sign( p2.getLongitude().degrees ) )
      {
        double delta = Math.Abs( p1.getLongitude().degrees - p2.getLongitude().degrees );
        if ( delta > 180 && delta < 360 )
          return true;
      }

      return false;
    }

    /**
     * Transform the negative longitudes of a dateline-spanning location list to positive values that maintain the
     * relationship with the other locations in the list. Negative longitudes are transformed to values greater than 180
     * degrees, as though longitude spanned [0, 360] rather than [-180, 180]. This enables arithmetic operations to be
     * performed on the locations without having to take into account the longitude jump at the dateline.
     *
     * @param locations the locations to transform. This list is not modified.
     *
     * @return a new list of locations transformed as described above.
     *
     * @throws ArgumentException if the location list is null.
     */
    public static List<LatLon> makeDatelineCrossingLocationsPositive( IEnumerable<LatLon> locations )
    {
      if ( locations == null )
      {
        string msg = Logging.getMessage( "nullValue.LocationsListIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      List<LatLon> newLocations = new List<LatLon>();
      if ( !locations.Any() )
        return newLocations;


      foreach ( LatLon location in locations )
      {
        if ( location == null )
          continue;

        if ( location.getLongitude().degrees < 0 )
        {
          newLocations.Add(
              LatLon.fromDegrees( location.getLatitude().degrees, location.getLongitude().degrees + 360 ) );
        }
        else
        {
          newLocations.Add( location );
        }
      }

      return newLocations;
    }

    /**
     * Parses a string containing latitude and longitude coordinates in either Degrees-minutes-seconds or decimal
     * degrees. The latitude must precede the longitude and the angles must be separated by a comma.
     *
     * @param latLonstring a string containing the comma separated latitude and longitude in either DMS or decimal
     *                     degrees.
     *
     * @return a <code>LatLon</code> instance with the parsed angles.
     *
     * @throws ArgumentException if <code>latLonString</code> is null.
     * @throws NumberFormatException    if the string does not form a latitude, longitude pair.
     */
    public LatLon parseLatLon( string latLonString ) // TODO
    {
      if ( latLonString == null )
      {
        string msg = Logging.getMessage( "nullValue.StringIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      throw new NotSupportedException(); // TODO: remove when implemented
    }

    public override string ToString()
    {
      string las = String.Format( "Lat %7.4f\u00B0", this.getLatitude().getDegrees() );
      string los = String.Format( "Lon %7.4f\u00B0", this.getLongitude().getDegrees() );
      return "(" + las + ", " + los + ")";
    }

    public override bool Equals( object o )
    {
      if ( this == o )
        return true;
      if ( o == null || GetType() != o.GetType() )
        return false;

      var latLon = (LatLon)o;

      if ( !latitude.Equals( latLon.latitude ) )
        return false;
      //noinspection RedundantIfStatement
      if ( !longitude.Equals( latLon.longitude ) )
        return false;

      return true;
    }

    public static bool equals( LatLon a, LatLon b )
    {
      return a.getLatitude().Equals( b.getLatitude() ) && a.getLongitude().Equals( b.getLongitude() );
    }

    public override int GetHashCode()
    {
      int result;
      result = latitude.GetHashCode();
      result = 29 * result + longitude.GetHashCode();
      return result;
    }

    /**
     * Compute the forward azimuth between two positions
     *
     * @param p1               first position
     * @param p2               second position
     * @param equatorialRadius the equatorial radius of the globe in meters
     * @param polarRadius      the polar radius of the globe in meters
     *
     * @return the azimuth
     */
    public static Angle ellipsoidalForwardAzimuth( LatLon p1, LatLon p2, double equatorialRadius, double polarRadius )
    {
      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // TODO: What if polar radius is larger than equatorial radius?
      // Calculate flattening
      double f = (equatorialRadius - polarRadius) / equatorialRadius; // flattening

      // Calculate reduced latitudes and related sines/cosines
      double U1 = Math.Atan( (1.0 - f) * Math.Tan( p1.latitude.radians ) );
      double cU1 = Math.Cos( U1 );
      double sU1 = Math.Sin( U1 );

      double U2 = Math.Atan( (1.0 - f) * Math.Tan( p2.latitude.radians ) );
      double cU2 = Math.Cos( U2 );
      double sU2 = Math.Sin( U2 );

      // Calculate difference in longitude
      double L = p2.longitude.subtract( p1.longitude ).radians;

      // Vincenty's Formula for Forward Azimuth
      // iterate until change in lambda is negligible (e.g. 1e-12 ~= 0.06mm)
      // first approximation
      double lambda = L;
      double sLambda = Math.Sin( lambda );
      double cLambda = Math.Cos( lambda );

      // dummy value to ensure
      double lambda_prev = Double.MaxValue;
      int count = 0;
      while ( Math.Abs( lambda - lambda_prev ) > 1e-12 && count++ < 100 )
      {
        // Store old lambda
        lambda_prev = lambda;
        // Calculate new lambda
        double sSigma = Math.Sqrt( Math.Pow( cU2 * sLambda, 2 )
            + Math.Pow( cU1 * sU2 - sU1 * cU2 * cLambda, 2 ) );
        double cSigma = sU1 * sU2 + cU1 * cU2 * cLambda;
        double sigma = Math.Atan2( sSigma, cSigma );
        double sAlpha = cU1 * cU2 * sLambda / sSigma;
        double cAlpha2 = 1 - sAlpha * sAlpha; // trig identity
                                              // As cAlpha2 approaches zeros, set cSigmam2 to zero to converge on a solution
        double cSigmam2;
        if ( Math.Abs( cAlpha2 ) < 1e-6 )
        {
          cSigmam2 = 0;
        }
        else
        {
          cSigmam2 = cSigma - 2 * sU1 * sU2 / cAlpha2;
        }
        double c = f / 16 * cAlpha2 * (4 + f * (4 - 3 * cAlpha2));

        lambda = L + (1 - c) * f * sAlpha * (sigma + c * sSigma * (cSigmam2 + c * cSigma * (-1 + 2 * cSigmam2)));
        sLambda = Math.Sin( lambda );
        cLambda = Math.Cos( lambda );
      }

      return Angle.fromRadians( Math.Atan2( cU2 * sLambda, cU1 * sU2 - sU1 * cU2 * cLambda ) );
    }

    // TODO: Need method to compute end position from initial position, azimuth and distance. The companion to the
    // spherical version, endPosition(), above.

    /**
     * Computes the distance between two points on an ellipsoid iteratively.
     * <p/>
     * NOTE: This method was copied from the UniData NetCDF Java library. http://www.unidata.ucar.edu/software/netcdf-java/
     * <p/>
     * Algorithm from U.S. National Geodetic Survey, FORTRAN program "inverse," subroutine "INVER1," by L. PFEIFER and
     * JOHN G. GERGEN. See http://www.ngs.noaa.gov/TOOLS/Inv_Fwd/Inv_Fwd.html
     * <p/>
     * Original documentation: SOLUTION OF THE GEODETIC INVERSE PROBLEM AFTER T.VINCENTY MODIFIED RAINSFORD'S METHOD
     * WITH HELMERT'S ELLIPTICAL TERMS EFFECTIVE IN ANY AZIMUTH AND AT ANY DISTANCE SHORT OF ANTIPODAL
     * STANDPOINT/FOREPOINT MUST NOT BE THE GEOGRAPHIC POLE
     * <p/>
     * Requires close to 1.4 E-5 seconds wall clock time per call on a 550 MHz Pentium with Linux 7.2.
     *
     * @param p1               first position
     * @param p2               second position
     * @param equatorialRadius the equatorial radius of the globe in meters
     * @param polarRadius      the polar radius of the globe in meters
     *
     * @return distance in meters between the two points
     */
    public static double ellipsoidalDistance( LatLon p1, LatLon p2, double equatorialRadius, double polarRadius )
    {
      // TODO: I think there is a non-iterative way to calculate the distance. Find it and compare with this one.
      // TODO: What if polar radius is larger than equatorial radius?
      double F = (equatorialRadius - polarRadius) / equatorialRadius; // flattening = 1.0 / 298.257223563;
      double R = 1.0 - F;
      double EPS = 0.5E-13;

      if ( p1 == null || p2 == null )
      {
        string message = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      // Algorithm from National Geodetic Survey, FORTRAN program "inverse,"
      // subroutine "INVER1," by L. PFEIFER and JOHN G. GERGEN.
      // http://www.ngs.noaa.gov/TOOLS/Inv_Fwd/Inv_Fwd.html
      // Conversion to JAVA from FORTRAN was made with as few changes as possible
      // to avoid errors made while recasting form, and to facilitate any future
      // comparisons between the original code and the altered version in Java.
      // Original documentation:
      // SOLUTION OF THE GEODETIC INVERSE PROBLEM AFTER T.VINCENTY
      // MODIFIED RAINSFORD'S METHOD WITH HELMERT'S ELLIPTICAL TERMS
      // EFFECTIVE IN ANY AZIMUTH AND AT ANY DISTANCE SHORT OF ANTIPODAL
      // STANDPOINT/FOREPOINT MUST NOT BE THE GEOGRAPHIC POLE
      // A IS THE SEMI-MAJOR AXIS OF THE REFERENCE ELLIPSOID
      // F IS THE FLATTENING (NOT RECIPROCAL) OF THE REFERNECE ELLIPSOID
      // LATITUDES GLAT1 AND GLAT2
      // AND LONGITUDES GLON1 AND GLON2 ARE IN RADIANS POSITIVE NORTH AND EAST
      // FORWARD AZIMUTHS AT BOTH POINTS RETURNED IN RADIANS FROM NORTH
      //
      // Reference ellipsoid is the WGS-84 ellipsoid.
      // See http://www.colorado.edu/geography/gcraft/notes/datum/elist.html
      // FAZ is forward azimuth in radians from pt1 to pt2;
      // BAZ is backward azimuth from point 2 to 1;
      // S is distance in meters.
      //
      // Conversion to JAVA from FORTRAN was made with as few changes as possible
      // to avoid errors made while recasting form, and to facilitate any future
      // comparisons between the original code and the altered version in Java.
      //
      //IMPLICIT REAL*8 (A-H,O-Z)
      //  COMMON/CONST/PI,RAD

      double GLAT1 = p1.getLatitude().radians;
      double GLAT2 = p2.getLatitude().radians;
      double TU1 = R * Math.Sin( GLAT1 ) / Math.Cos( GLAT1 );
      double TU2 = R * Math.Sin( GLAT2 ) / Math.Cos( GLAT2 );
      double CU1 = 1.0 / Math.Sqrt( TU1 * TU1 + 1.0 );
      double SU1 = CU1 * TU1;
      double CU2 = 1.0 / Math.Sqrt( TU2 * TU2 + 1.0 );
      double S = CU1 * CU2;
      double BAZ = S * TU2;
      double FAZ = BAZ * TU1;
      double GLON1 = p1.getLongitude().radians;
      double GLON2 = p2.getLongitude().radians;
      double X = GLON2 - GLON1;
      double D, SX, CX, SY, CY, Y, SA, C2A, CZ, E, C;
      do
      {
        SX = Math.Sin( X );
        CX = Math.Cos( X );
        TU1 = CU2 * SX;
        TU2 = BAZ - SU1 * CU2 * CX;
        SY = Math.Sqrt( TU1 * TU1 + TU2 * TU2 );
        CY = S * CX + FAZ;
        Y = Math.Atan2( SY, CY );
        SA = S * SX / SY;
        C2A = -SA * SA + 1.0;
        CZ = FAZ + FAZ;
        if ( C2A > 0.0 )
        {
          CZ = -CZ / C2A + CY;
        }
        E = CZ * CZ * 2.0 - 1.0;
        C = ((-3.0 * C2A + 4.0) * F + 4.0) * C2A * F / 16.0;
        D = X;
        X = ((E * CY * C + CZ) * SY * C + Y) * SA;
        X = (1.0 - C) * X * F + GLON2 - GLON1;
        //IF(DABS(D-X).GT.EPS) GO TO 100
      }
      while ( Math.Abs( D - X ) > EPS );

      //FAZ = Math.Atan2(TU1, TU2);
      //BAZ = Math.Atan2(CU1 * SX, BAZ * CX - SU1 * CU2) + Math.PI;
      X = Math.Sqrt( (1.0 / R / R - 1.0) * C2A + 1.0 ) + 1.0;
      X = (X - 2.0) / X;
      C = 1.0 - X;
      C = (X * X / 4.0 + 1.0) / C;
      D = (0.375 * X * X - 1.0) * X;
      X = E * CY;
      S = 1.0 - E - E;
      S = ((((SY * SY * 4.0 - 3.0) * S * CZ * D / 6.0 - X) * D / 4.0 + CZ) * SY
          * D + Y) * C * equatorialRadius * R;

      return S;
    }

    /**
     * Computes a new set of locations translated from a specified location to a new location.
     *
     * @param oldLocation the original reference location.
     * @param newLocation the new reference location.
     * @param locations   the locations to translate.
     *
     * @return the translated locations, or null if the locations could not be translated.
     *
     * @throws ArgumentException if any argument is null.
     */
    public static List<LatLon> computeShiftedLocations( Position oldLocation, Position newLocation,
        IEnumerable<LatLon> locations )
    {
      // TODO: Account for dateline spanning
      if ( oldLocation == null || newLocation == null )
      {
        string msg = Logging.getMessage( "nullValue.PositionIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      if ( locations == null )
      {
        string msg = Logging.getMessage( "nullValue.PositionsListIsNull" );
        Logging.logger().severe( msg );
        throw new ArgumentException( msg );
      }

      List<LatLon> newPositions = new List<LatLon>();

      foreach ( LatLon location in locations )
      {
        Angle distance = LatLon.greatCircleDistance( oldLocation, location );
        Angle azimuth = LatLon.greatCircleAzimuth( oldLocation, location );
        newPositions.Add( Position.greatCircleEndPosition( newLocation, azimuth, distance ) );
      }

      return newPositions;
    }
  }
}
