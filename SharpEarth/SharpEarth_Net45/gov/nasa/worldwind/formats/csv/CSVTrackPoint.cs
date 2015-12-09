/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.tracks;
using SharpEarth.geom;
using SharpEarth.util;
namespace SharpEarth.formats.csv{


/**
 * @author tag
 * @version $Id: CSVTrackPoint.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class CSVTrackPoint : TrackPoint
{
    string time = "";
    private double latitude;
    private double longitude;
    private double altitude;

    /**
     * @param words
     * @throws ArgumentException if <code>words</code> is null or has length less than 1
     */
    public CSVTrackPoint(string[] words)
    {
        if (words == null)
        {
            string msg = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if (words.Length < 2)
        {
            string msg = Logging.getMessage("generic.ArrayInvalidLength", words.Length );
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.doValues(words);
    }

    private void doValues(string[] words)
    {
        this.latitude = this.parseLatitude(words[1]);
        this.longitude = this.parseLongitude(words[2]);
        if (words.Length > 3)
            this.altitude = this.parseElevation(words[3], "M");
    }

    private double parseLatitude(string angle)
    {
      double value;
      double.TryParse( angle, out value );
      return value;
    }

    private double parseLongitude(string angle)
    {
      double value;
      double.TryParse( angle, out value );
      return value;
    }

    private double parseElevation(string alt, string units)
    {
      double value;
      return double.TryParse( alt, out value ) ? unitsToMeters( units ) : 0;
    }

    private double unitsToMeters(string units)
    {
        double f;

        if (units.Equals("M")) // meters
            f = 1d;
        else if (units.Equals("f")) // feet
            f = 3.2808399;
        else if (units.Equals("F")) // fathoms
            f = 0.5468066528;
        else
            f = 1d;

        return f;
    }

    public double getLatitude()
    {
        return latitude;
    }

    /**
     * @param latitude
     * @throws ArgumentException if <code>latitude</code> is less than -90 or greater than 90
     */
    public void setLatitude(double latitude)
    {
        if (latitude > 90 || latitude < -90)
        {
            string msg = Logging.getMessage("generic.AngleOutOfRange", latitude);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.latitude = latitude;
    }

    public double getLongitude()
    {
        return longitude;
    }

    /**
     * @param longitude
     * @throws ArgumentException if <code>longitude</code> is less than -180 or greater than 180
     */
    public void setLongitude(double longitude)
    {
        if (longitude > 180 || longitude < -180)
        {
            string msg = Logging.getMessage("generic.AngleOutOfRange", longitude);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.longitude = longitude;
    }

    public Position getPosition()
    {
        return Position.fromDegrees(this.latitude, this.longitude, this.altitude);
    }

    public void setPosition(Position position)
    {
        if (position == null)
        {
            string msg = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.latitude = position.getLatitude().getDegrees();
        this.longitude = position.getLongitude().getDegrees();
        this.altitude = position.getElevation();
    }

    public double getElevation()
    {
        return this.altitude;
    }

    public void setElevation(double elevation)
    {
        this.altitude = elevation;
    }

    public string getTime()
    {
        return null;
    }

    public void setTime(string time)
    {
        this.time = time;
    }

    public override string ToString()
    {
        return string.Format("(%10.8f\u00B0, %11.8f\u00B0, %10.4g m, %s)", this.latitude, this.longitude, this.altitude, this.time);
    }
}
}
