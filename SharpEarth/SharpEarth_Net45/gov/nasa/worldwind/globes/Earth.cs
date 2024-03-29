/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.avlist;

namespace SharpEarth.globes
{
  /**
   * Defines a model of the Earth, using the <a href="http://en.wikipedia.org/wiki/World_Geodetic_System"
   * target="_blank">World Geodetic System</a> (WGS84).
   *
   * @author Tom Gaskins
   * @version $Id: Earth.java 1958 2014-04-24 19:25:37Z tgaskins $
   */

  public class Earth : EllipsoidalGlobe
  {
    public static readonly double WGS84_EQUATORIAL_RADIUS = 6378137.0; // ellipsoid equatorial getRadius, in meters
    public static readonly double WGS84_POLAR_RADIUS = 6356752.3; // ellipsoid polar getRadius, in meters
    public static readonly double WGS84_ES = 0.00669437999013; // eccentricity squared, semi-major axis

    public static readonly double ELEVATION_MIN = -11000d; // Depth of Marianas trench
    public static readonly double ELEVATION_MAX = 8500d; // Height of Mt. Everest.

    public Earth() : base( WGS84_EQUATORIAL_RADIUS, WGS84_POLAR_RADIUS, WGS84_ES,
            EllipsoidalGlobe.makeElevationModel( AVKey.EARTH_ELEVATION_MODEL_CONFIG_FILE,
                "config/Earth/EarthElevations2.xml" ) )
    {
    }

    public override string ToString()
    {
      return "Earth";
    }
  }
}
