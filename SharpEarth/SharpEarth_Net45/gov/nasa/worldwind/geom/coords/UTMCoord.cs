/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.globes;
using SharpEarth.geom;
using SharpEarth.avlist;
using System;
using System.Text;

namespace SharpEarth.geom.coords{


/**
 * This immutable class holds a set of UTM coordinates along with it's corresponding latitude and longitude.
 *
 * @author Patrick Murris
 * @version $Id: UTMCoord.java 1171 2013-02-11 21:45:02Z dcollins $
 */

public class UTMCoord
{
    private Angle latitude;
    private Angle longitude;
    private string hemisphere;
    private int zone;
    private double easting;
    private double northing;
    private Angle centralMeridian;

    /**
     * Create a set of UTM coordinates from a pair of latitude and longitude for a WGS84 globe.
     *
     * @param latitude  the latitude <code>Angle</code>.
     * @param longitude the longitude <code>Angle</code>.
     *
     * @return the corresponding <code>UTMCoord</code>.
     *
     * @throws ArgumentException if <code>latitude</code> or <code>longitude</code> is null, or the conversion to
     *                                  UTM coordinates fails.
     */
    public static UTMCoord fromLatLon(Angle latitude, Angle longitude)
    {
        return fromLatLon(latitude, longitude, (Globe) null);
    }

    /**
     * Create a set of UTM coordinates from a pair of latitude and longitude for the given <code>Globe</code>.
     *
     * @param latitude  the latitude <code>Angle</code>.
     * @param longitude the longitude <code>Angle</code>.
     * @param globe     the <code>Globe</code> - can be null (will use WGS84).
     *
     * @return the corresponding <code>UTMCoord</code>.
     *
     * @throws ArgumentException if <code>latitude</code> or <code>longitude</code> is null, or the conversion to
     *                                  UTM coordinates fails.
     */
    public static UTMCoord fromLatLon(Angle latitude, Angle longitude, Globe globe)
    {
        if (latitude == null || longitude == null)
        {
            string message = Logging.getMessage("nullValue.LatitudeOrLongitudeIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        UTMCoordConverter converter = new UTMCoordConverter(globe);
        long err = converter.convertGeodeticToUTM(latitude.radians, longitude.radians);

        if (err != UTMCoordConverter.UTM_NO_ERROR)
        {
            string message = Logging.getMessage("Coord.UTMConversionError");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return new UTMCoord(latitude, longitude, converter.getZone(), converter.getHemisphere(),
            converter.getEasting(), converter.getNorthing(), Angle.fromRadians(converter.getCentralMeridian()));
    }

    public static UTMCoord fromLatLon(Angle latitude, Angle longitude, string datum)
    {
        if (latitude == null || longitude == null)
        {
            string message = Logging.getMessage("nullValue.LatitudeOrLongitudeIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        UTMCoordConverter converter;
        if (!WWUtil.isEmpty(datum) && datum.Equals("NAD27"))
        {
            converter = new UTMCoordConverter(UTMCoordConverter.CLARKE_A, UTMCoordConverter.CLARKE_F);
            LatLon llNAD27 = UTMCoordConverter.convertWGS84ToNAD27(latitude, longitude);
            latitude = llNAD27.getLatitude();
            longitude = llNAD27.getLongitude();
        }
        else
        {
            converter = new UTMCoordConverter(UTMCoordConverter.WGS84_A, UTMCoordConverter.WGS84_F);
        }

        long err = converter.convertGeodeticToUTM(latitude.radians, longitude.radians);

        if (err != UTMCoordConverter.UTM_NO_ERROR)
        {
            string message = Logging.getMessage("Coord.UTMConversionError");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return new UTMCoord(latitude, longitude, converter.getZone(), converter.getHemisphere(),
            converter.getEasting(), converter.getNorthing(), Angle.fromRadians(converter.getCentralMeridian()));
    }

    /**
     * Create a set of UTM coordinates for a WGS84 globe.
     *
     * @param zone       the UTM zone - 1 to 60.
     * @param hemisphere the hemisphere, either {@link SharpEarth.avlist.AVKey#NORTH} or {@link
     *                   SharpEarth.avlist.AVKey#SOUTH}.
     * @param easting    the easting distance in meters
     * @param northing   the northing distance in meters.
     *
     * @return the corresponding <code>UTMCoord</code>.
     *
     * @throws ArgumentException if the conversion to UTM coordinates fails.
     */
    public static UTMCoord fromUTM(int zone, string hemisphere, double easting, double northing)
    {
        return fromUTM(zone, hemisphere, easting, northing, null);
    }

    /**
     * Create a set of UTM coordinates for the given <code>Globe</code>.
     *
     * @param zone       the UTM zone - 1 to 60.
     * @param hemisphere the hemisphere, either {@link SharpEarth.avlist.AVKey#NORTH} or {@link
     *                   SharpEarth.avlist.AVKey#SOUTH}.
     * @param easting    the easting distance in meters
     * @param northing   the northing distance in meters.
     * @param globe      the <code>Globe</code> - can be null (will use WGS84).
     *
     * @return the corresponding <code>UTMCoord</code>.
     *
     * @throws ArgumentException if the conversion to UTM coordinates fails.
     */
    public static UTMCoord fromUTM(int zone, string hemisphere, double easting, double northing, Globe globe)
    {
        UTMCoordConverter converter = new UTMCoordConverter(globe);
        long err = converter.convertUTMToGeodetic(zone, hemisphere, easting, northing);

        if (err != UTMCoordConverter.UTM_NO_ERROR)
        {
            string message = Logging.getMessage("Coord.UTMConversionError");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return new UTMCoord(Angle.fromRadians(converter.getLatitude()),
            Angle.fromRadians(converter.getLongitude()),
            zone, hemisphere, easting, northing, Angle.fromRadians(converter.getCentralMeridian()));
    }

    /**
     * Convenience method for converting a UTM coordinate to a geographic location.
     *
     * @param zone       the UTM zone: 1 to 60.
     * @param hemisphere the hemisphere, either {@link SharpEarth.avlist.AVKey#NORTH} or {@link
     *                   SharpEarth.avlist.AVKey#SOUTH}.
     * @param easting    the easting distance in meters
     * @param northing   the northing distance in meters.
     * @param globe      the <code>Globe</code>. Can be null (will use WGS84).
     *
     * @return the geographic location corresponding to the specified UTM coordinate.
     */
    public static LatLon locationFromUTMCoord(int zone, string hemisphere, double easting, double northing, Globe globe)
    {
        UTMCoord coord = UTMCoord.fromUTM(zone, hemisphere, easting, northing, globe);
        return new LatLon(coord.getLatitude(), coord.getLongitude());
    }

    /**
     * Create an arbitrary set of UTM coordinates with the given values.
     *
     * @param latitude   the latitude <code>Angle</code>.
     * @param longitude  the longitude <code>Angle</code>.
     * @param zone       the UTM zone - 1 to 60.
     * @param hemisphere the hemisphere, either {@link SharpEarth.avlist.AVKey#NORTH} or {@link
     *                   SharpEarth.avlist.AVKey#SOUTH}.
     * @param easting    the easting distance in meters
     * @param northing   the northing distance in meters.
     *
     * @throws ArgumentException if <code>latitude</code> or <code>longitude</code> is null.
     */
    public UTMCoord(Angle latitude, Angle longitude, int zone, string hemisphere, double easting, double northing)
       :this(latitude, longitude, zone, hemisphere, easting, northing, Angle.fromDegreesLongitude(0.0))
    {
    }

    /**
     * Create an arbitrary set of UTM coordinates with the given values.
     *
     * @param latitude        the latitude <code>Angle</code>.
     * @param longitude       the longitude <code>Angle</code>.
     * @param zone            the UTM zone - 1 to 60.
     * @param hemisphere      the hemisphere, either {@link SharpEarth.avlist.AVKey#NORTH} or {@link
     *                        SharpEarth.avlist.AVKey#SOUTH}.
     * @param easting         the easting distance in meters
     * @param northing        the northing distance in meters.
     * @param centralMeridian the cntral meridian <code>Angle</code>.
     *
     * @throws ArgumentException if <code>latitude</code> or <code>longitude</code> is null.
     */
    public UTMCoord(Angle latitude, Angle longitude, int zone, string hemisphere, double easting, double northing,
        Angle centralMeridian)
    {
        if (latitude == null || longitude == null)
        {
            string message = Logging.getMessage("nullValue.LatitudeOrLongitudeIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.latitude = latitude;
        this.longitude = longitude;
        this.hemisphere = hemisphere;
        this.zone = zone;
        this.easting = easting;
        this.northing = northing;
        this.centralMeridian = centralMeridian;
    }

    public Angle getCentralMeridian()
    {
        return this.centralMeridian;
    }

    public Angle getLatitude()
    {
        return this.latitude;
    }

    public Angle getLongitude()
    {
        return this.longitude;
    }

    public int getZone()
    {
        return this.zone;
    }

    public string getHemisphere()
    {
        return this.hemisphere;
    }

    public double getEasting()
    {
        return this.easting;
    }

    public double getNorthing()
    {
        return this.northing;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(zone);
        sb.Append(" ").Append(AVKey.NORTH.Equals(hemisphere) ? "N" : "S");
        sb.Append(" ").Append(easting).Append("E");
        sb.Append(" ").Append(northing).Append("N");
        return sb.ToString();
    }
}
}
