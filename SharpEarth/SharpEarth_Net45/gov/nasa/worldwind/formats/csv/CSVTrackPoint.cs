/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.tracks;
using SharpEarth.geom.Position;
using SharpEarth.util;
namespace SharpEarth.formats.csv{


/**
 * @author tag
 * @version $Id: CSVTrackPoint.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class CSVTrackPoint implements TrackPoint
{
    String time = "";
    private double latitude;
    private double longitude;
    private double altitude;

    /**
     * @param words
     * @throws ArgumentException if <code>words</code> is null or has length less than 1
     */
    public CSVTrackPoint(String[] words)
    {
        if (words == null)
        {
            String msg = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if (words.length < 2)
        {
            String msg = Logging.getMessage("generic.ArrayInvalidLength", words.length);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        this.doValues(words);
    }

    private void doValues(String[] words)
    {
        this.latitude = this.parseLatitude(words[1]);
        this.longitude = this.parseLongitude(words[2]);
        if (words.length > 3)
            this.altitude = this.parseElevation(words[3], "M");
    }

    private double parseLatitude(String angle)
    {
        return angle.length() == 0 ? 0 : Double.parseDouble(angle);
    }

    private double parseLongitude(String angle)
    {
        return angle.length() == 0 ? 0 : Double.parseDouble(angle);
    }

    private double parseElevation(String alt, String units)
    {
        return alt.length() == 0 ? 0 : Double.parseDouble(alt) * unitsToMeters(units);
    }

    private double unitsToMeters(String units)
    {
        double f;

        if (units.equals("M")) // meters
            f = 1d;
        else if (units.equals("f")) // feet
            f = 3.2808399;
        else if (units.equals("F")) // fathoms
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
            String msg = Logging.getMessage("generic.AngleOutOfRange", latitude);
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
            String msg = Logging.getMessage("generic.AngleOutOfRange", longitude);
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
            String msg = Logging.getMessage("nullValue.PositionIsNull");
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

    public String getTime()
    {
        return null;
    }

    public void setTime(String time)
    {
        this.time = time;
    }

    @Override
    public override string ToString()
    {
        return String.Format("(%10.8f\u00B0, %11.8f\u00B0, %10.4g m, %s)", this.latitude, this.longitude,
            this.altitude, this.time);
    }
}
}
