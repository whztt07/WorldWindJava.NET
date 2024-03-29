/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util;
using SharpEarth.util;
using SharpEarth.globes;
using System.Collections.Generic;
using System;

namespace SharpEarth.geom{



/**
 * @author tag
 * @version $Id: Position.java 2291 2014-08-30 21:38:47Z tgaskins $
 */
public class Position : LatLon
{
    public static readonly Position ZERO = new Position(Angle.ZERO, Angle.ZERO, 0d);

    public readonly double elevation;

    public static Position fromRadians(double latitude, double longitude, double elevation)
    {
        return new Position(Angle.fromRadians(latitude), Angle.fromRadians(longitude), elevation);
    }

    public static Position fromDegrees(double latitude, double longitude, double elevation)
    {
        return new Position(Angle.fromDegrees(latitude), Angle.fromDegrees(longitude), elevation);
    }

    public static Position fromDegrees(double latitude, double longitude)
    {
        return new Position(Angle.fromDegrees(latitude), Angle.fromDegrees(longitude), 0);
    }

    public Position(Angle latitude, Angle longitude, double elevation)
        : base( latitude, longitude)
    {
      this.elevation = elevation;
    }

    public Position(LatLon latLon, double elevation)
        :base(latLon)
    {
      this.elevation = elevation;
    }

    // A class that makes it easier to pass around position lists.
    public class PositionList
    {
        public List<Position> list;

        public PositionList(List<Position> list)
        {
            this.list = list;
        }
    }

    /**
     * Obtains the elevation of this position
     *
     * @return this position's elevation
     */
    public double getElevation()
    {
        return this.elevation;
    }

    /**
     * Obtains the elevation of this position
     *
     * @return this position's elevation
     */
    public double getAltitude()
    {
        return this.elevation;
    }

    public Position add(Position that)
    {
        Angle lat = Angle.normalizedLatitude(this.latitude.add(that.latitude));
        Angle lon = Angle.normalizedLongitude(this.longitude.add(that.longitude));

        return new Position(lat, lon, this.elevation + that.elevation);
    }

    public Position subtract(Position that)
    {
        Angle lat = Angle.normalizedLatitude(this.latitude.subtract(that.latitude));
        Angle lon = Angle.normalizedLongitude(this.longitude.subtract(that.longitude));

        return new Position(lat, lon, this.elevation - that.elevation);
    }

    /**
     * Returns the linear interpolation of <code>value1</code> and <code>value2</code>, treating the geographic
     * locations as simple 2D coordinate pairs, and treating the elevation values as 1D scalars.
     *
     * @param amount the interpolation factor
     * @param value1 the first position.
     * @param value2 the second position.
     *
     * @return the linear interpolation of <code>value1</code> and <code>value2</code>.
     *
     * @throws ArgumentException if either position is null.
     */
    public static Position interpolate(double amount, Position value1, Position value2)
    {
        if (value1 == null || value2 == null)
        {
            String message = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (amount < 0)
            return value1;
        else if (amount > 1)
            return value2;

        LatLon latLon = LatLon.interpolate(amount, value1, value2);
        // Elevation is independent of geographic interpolation method (i.e. rhumb, great-circle, linear), so we
        // interpolate elevation linearly.
        double elevation = WWMath.mix(amount, value1.getElevation(), value2.getElevation());

        return new Position(latLon, elevation);
    }

    /**
     * Returns the an interpolated location along the great-arc between <code>value1</code> and <code>value2</code>. The
     * position's elevation components are linearly interpolated as a simple 1D scalar value. The interpolation factor
     * <code>amount</code> defines the weight given to each value, and is clamped to the range [0, 1]. If <code>a</code>
     * is 0 or less, this returns <code>value1</code>. If <code>amount</code> is 1 or more, this returns
     * <code>value2</code>. Otherwise, this returns the position on the great-arc between <code>value1</code> and
     * <code>value2</code> with a linearly interpolated elevation component, and corresponding to the specified
     * interpolation factor.
     *
     * @param amount the interpolation factor
     * @param value1 the first position.
     * @param value2 the second position.
     *
     * @return an interpolated position along the great-arc between <code>value1</code> and <code>value2</code>, with a
     *         linearly interpolated elevation component.
     *
     * @throws ArgumentException if either location is null.
     */
    public static Position interpolateGreatCircle(double amount, Position value1, Position value2)
    {
        if (value1 == null || value2 == null)
        {
            String message = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        LatLon latLon = LatLon.interpolateGreatCircle(amount, value1, value2);
        // Elevation is independent of geographic interpolation method (i.e. rhumb, great-circle, linear), so we
        // interpolate elevation linearly.
        double elevation = WWMath.mix(amount, value1.getElevation(), value2.getElevation());

        return new Position(latLon, elevation);
    }

    /**
     * Returns the an interpolated location along the rhumb line between <code>value1</code> and <code>value2</code>.
     * The position's elevation components are linearly interpolated as a simple 1D scalar value. The interpolation
     * factor <code>amount</code> defines the weight given to each value, and is clamped to the range [0, 1]. If
     * <code>a</code> is 0 or less, this returns <code>value1</code>. If <code>amount</code> is 1 or more, this returns
     * <code>value2</code>. Otherwise, this returns the position on the rhumb line between <code>value1</code> and
     * <code>value2</code> with a linearly interpolated elevation component, and corresponding to the specified
     * interpolation factor.
     *
     * @param amount the interpolation factor
     * @param value1 the first position.
     * @param value2 the second position.
     *
     * @return an interpolated position along the great-arc between <code>value1</code> and <code>value2</code>, with a
     *         linearly interpolated elevation component.
     *
     * @throws ArgumentException if either location is null.
     */
    public static Position interpolateRhumb(double amount, Position value1, Position value2)
    {
        if (value1 == null || value2 == null)
        {
            String message = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        LatLon latLon = LatLon.interpolateRhumb(amount, value1, value2);
        // Elevation is independent of geographic interpolation method (i.e. rhumb, great-circle, linear), so we
        // interpolate elevation linearly.
        double elevation = WWMath.mix(amount, value1.getElevation(), value2.getElevation());

        return new Position(latLon, elevation);
    }

    public static bool positionsCrossDateLine(IEnumerable<Position> positions)
    {
        if (positions == null)
        {
            String msg = Logging.getMessage("nullValue.PositionsListIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        Position pos = null;
        foreach (Position posNext in positions)
        {
            if (pos != null)
            {
                // A segment cross the line if end pos have different longitude signs
                // and are more than 180 degress longitude apart
                if (Math.Sign(pos.getLongitude().degrees) != Math.Sign(posNext.getLongitude().degrees))
                {
                    double delta = Math.Abs(pos.getLongitude().degrees - posNext.getLongitude().degrees);
                    if (delta > 180 && delta < 360)
                        return true;
                }
            }
            pos = posNext;
        }

        return false;
    }

    /**
     * Computes a new set of positions translated from a specified reference position to a new reference position.
     *
     * @param oldPosition the original reference position.
     * @param newPosition the new reference position.
     * @param positions   the positions to translate.
     *
     * @return the translated positions, or null if the positions could not be translated.
     *
     * @throws ArgumentException if any argument is null.
     */
    public static List<Position> computeShiftedPositions(Position oldPosition, Position newPosition,
        IEnumerable<Position> positions)
    {
        // TODO: Account for dateline spanning
        if (oldPosition == null || newPosition == null)
        {
            String msg = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (positions == null)
        {
            String msg = Logging.getMessage("nullValue.PositionsListIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        List<Position> newPositions = new List<Position>();

        double elevDelta = newPosition.getElevation() - oldPosition.getElevation();

        foreach (Position pos in positions)
        {
            Angle distance = LatLon.greatCircleDistance(oldPosition, pos);
            Angle azimuth = LatLon.greatCircleAzimuth(oldPosition, pos);
            LatLon newLocation = LatLon.greatCircleEndPosition(newPosition, azimuth, distance);
            double newElev = pos.getElevation() + elevDelta;

            newPositions.Add(new Position(newLocation, newElev));
        }

        return newPositions;
    }

    public static List<Position> computeShiftedPositions(Globe globe, Position oldPosition, Position newPosition,
        IEnumerable<Position> positions)
    {
        if (globe == null)
        {
            String msg = Logging.getMessage("nullValue.GlobeIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (oldPosition == null || newPosition == null)
        {
            String msg = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (positions == null)
        {
            String msg = Logging.getMessage("nullValue.PositionsListIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        List<Position> newPositions = new List<Position>();

        double elevDelta = newPosition.getElevation() - oldPosition.getElevation();
        Vec4 oldPoint = globe.computePointFromPosition(oldPosition);
        Vec4 newPoint = globe.computePointFromPosition(newPosition);
        Vec4 delta = newPoint.subtract3(oldPoint);

        foreach (Position pos in positions)
        {
            Vec4 point = globe.computePointFromPosition(pos);
            point = point.add3(delta);
            Position newPos = globe.computePositionFromPoint(point);
            double newElev = pos.getElevation() + elevDelta;

            newPositions.Add(new Position(newPos, newElev));
        }

        return newPositions;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;
        if (!base.Equals(o))
            return false;

        Position position = (Position) o;

        //noinspection RedundantIfStatement
        if (position.elevation.CompareTo(elevation) != 0)
            return false;

        return true;
    }
    
    public override int GetHashCode()
    {
        int result = base.GetHashCode();
        ulong temp;
        temp = (ulong) (elevation != +0.0d ? BitConverter.DoubleToInt64Bits(elevation) : 0L);
        result = 31 * result + (int) (temp ^ (temp >> 32));
        return result;
    }

  public override string ToString()
    {
        return "(" + this.latitude.ToString() + ", " + this.longitude.ToString() + ", " + this.elevation + ")";
    }
}
}
