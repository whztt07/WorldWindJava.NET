/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.util;
using SharpEarth.geom;

namespace SharpEarth.view.orbit{


/**
 * BasicOrbitViewLimits provides an implementation of OrbitViewLimits.
 *
 * @author dcollins
 * @version $Id: BasicOrbitViewLimits.java 2253 2014-08-22 16:33:46Z dcollins $
 */
public class BasicOrbitViewLimits : BasicViewPropertyLimits , OrbitViewLimits
{
    protected Sector centerLocationLimits;
    protected double minCenterElevation;
    protected double maxCenterElevation;
    protected double minZoom;
    protected double maxZoom;

    /** Creates a new BasicOrbitViewLimits with default limits. */
    public BasicOrbitViewLimits()
    {
        this.reset();
    }

    /** {@inheritDoc} */
    public Sector getCenterLocationLimits()
    {
        return this.centerLocationLimits;
    }

    /** {@inheritDoc} */
    public void setCenterLocationLimits(Sector sector)
    {
        if (sector == null)
        {
            String message = Logging.getMessage("nullValue.SectorIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.centerLocationLimits = sector;
    }

    /** {@inheritDoc} */
    public double[] getCenterElevationLimits()
    {
        return new double[] {this.minCenterElevation, this.maxCenterElevation};
    }

    /** {@inheritDoc} */
    public void setCenterElevationLimits(double minValue, double maxValue)
    {
        this.minCenterElevation = minValue;
        this.maxCenterElevation = maxValue;
    }

    /** {@inheritDoc} */
    public double[] getZoomLimits()
    {
        return new double[] {this.minZoom, this.maxZoom};
    }

    /** {@inheritDoc} */
    public void setZoomLimits(double minValue, double maxValue)
    {
        this.minZoom = minValue;
        this.maxZoom = maxValue;
    }

    /** {@inheritDoc} */
    public void reset()
    {
        base.reset();

        this.centerLocationLimits = Sector.FULL_SPHERE;
        this.minCenterElevation = -Double.MaxValue;
        this.maxCenterElevation = Double.MaxValue;
        this.minZoom = 0;
        this.maxZoom = Double.MaxValue;
    }

    /** {@inheritDoc} */
    public Position limitCenterPosition(View view, Position position)
    {
        if (view == null)
        {
            String message = Logging.getMessage("nullValue.ViewIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (position == null)
        {
            String message = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Sector sector = this.centerLocationLimits;
        Angle lat = Angle.clamp(position.latitude, sector.getMinLatitude(), sector.getMaxLatitude());
        Angle lon = Angle.clamp(position.longitude, sector.getMinLongitude(), sector.getMaxLongitude());
        double alt = WWMath.clamp(position.elevation, this.minCenterElevation, this.maxCenterElevation);

        return new Position(lat, lon, alt);
    }

    /** {@inheritDoc} */
    public double limitZoom(View view, double value)
    {
        if (view == null)
        {
            String message = Logging.getMessage("nullValue.ViewIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        double minZoom = this.minZoom;
        double maxZoom = this.maxZoom;

        if (this.is2DGlobe(view.getGlobe())) // limit zoom to ~360 degrees of visible longitude on 2D globes
        {
            double max2DZoom = Math.PI * view.getGlobe().getEquatorialRadius() / view.getFieldOfView().tanHalfAngle();
            if (minZoom > max2DZoom)
                minZoom = max2DZoom;
            if (maxZoom > max2DZoom)
                maxZoom = max2DZoom;
        }

        return WWMath.clamp(value, minZoom, maxZoom);
    }

    /**
     * Applies the orbit view property limits to the specified view.
     *
     * @param view       the view that receives the property limits.
     * @param viewLimits defines the view property limits.
     *
     * @throws ArgumentException if any argument is null.
     * @deprecated Use methods that limit individual view properties directly: {@link #limitCenterPosition(gov.nasa.worldwind.View,
     *             SharpEarth.geom.Position)}, {@link #limitHeading(gov.nasa.worldwind.View,
     *             SharpEarth.geom.Angle)}, etc.
     */
    public static void applyLimits(OrbitView view, OrbitViewLimits viewLimits)
    {
        if (view == null)
        {
            String message = Logging.getMessage("nullValue.ViewIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (viewLimits == null)
        {
            String message = Logging.getMessage("nullValue.ViewLimitsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        view.setCenterPosition(limitCenterPosition(view.getCenterPosition(), viewLimits));
        view.setHeading(limitHeading(view.getHeading(), viewLimits));
        view.setPitch(limitPitch(view.getPitch(), viewLimits));
        view.setZoom(limitZoom(view.getZoom(), viewLimits));
    }

    /**
     * Clamp center location angles and elevation to the range specified in a limit object.
     *
     * @param position   position to clamp to the allowed range.
     * @param viewLimits defines the center location and elevation limits.
     *
     * @throws ArgumentException if any argument is null.
     * @deprecated Use {@link #limitCenterPosition(gov.nasa.worldwind.View, SharpEarth.geom.Position)} instead.
     */
    public static Position limitCenterPosition(Position position, OrbitViewLimits viewLimits)
    {
        if (position == null)
        {
            String message = Logging.getMessage("nullValue.PositionIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (viewLimits == null)
        {
            String message = Logging.getMessage("nullValue.ViewLimitsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return new Position(
            limitCenterLocation(position.getLatitude(), position.getLongitude(), viewLimits),
            limitCenterElevation(position.getElevation(), viewLimits));
    }

    /**
     * Clamp center location angles to the range specified in a limit object.
     *
     * @param latitude   latitude angle to clamp to the allowed range.
     * @param longitude  longitude angle to clamp to the allowed range.
     * @param viewLimits defines the center location limits.
     *
     * @throws ArgumentException if any argument is null.
     * @deprecated Use {@link #limitCenterPosition(gov.nasa.worldwind.View, SharpEarth.geom.Position)} instead.
     */
    public static LatLon limitCenterLocation(Angle latitude, Angle longitude, OrbitViewLimits viewLimits)
    {
        if (latitude == null || longitude == null)
        {
            String message = Logging.getMessage("nullValue.LatitudeOrLongitudeIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (viewLimits == null)
        {
            String message = Logging.getMessage("nullValue.ViewLimitsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        Sector limits = viewLimits.getCenterLocationLimits();
        Angle newLatitude = latitude;
        Angle newLongitude = longitude;

        if (latitude.compareTo(limits.getMinLatitude()) < 0)
        {
            newLatitude = limits.getMinLatitude();
        }
        else if (latitude.compareTo(limits.getMaxLatitude()) > 0)
        {
            newLatitude = limits.getMaxLatitude();
        }

        if (longitude.compareTo(limits.getMinLongitude()) < 0)
        {
            newLongitude = limits.getMinLongitude();
        }
        else if (longitude.compareTo(limits.getMaxLongitude()) > 0)
        {
            newLongitude = limits.getMaxLongitude();
        }

        return new LatLon(newLatitude, newLongitude);
    }

    /**
     * Clamp an center elevation to the range specified in a limit object.
     *
     * @param value      elevation to clamp to the allowed range.
     * @param viewLimits defines the center elevation limits.
     *
     * @throws ArgumentException if any argument is null.
     * @deprecated Use {@link #limitCenterPosition(gov.nasa.worldwind.View, SharpEarth.geom.Position)} instead.
     */
    public static double limitCenterElevation(double value, OrbitViewLimits viewLimits)
    {
        if (viewLimits == null)
        {
            String message = Logging.getMessage("nullValue.ViewLimitsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        double[] limits = viewLimits.getCenterElevationLimits();
        double newValue = value;

        if (value < limits[0])
        {
            newValue = limits[0];
        }
        else if (value > limits[1])
        {
            newValue = limits[1];
        }

        return newValue;
    }

    /**
     * Clamp an zoom distance to the range specified in a limit object.
     *
     * @param value      distance to clamp to the allowed range.
     * @param viewLimits defines the zoom distance limits.
     *
     * @throws ArgumentException if any argument is null.
     * @deprecated Use {@link #limitZoom(gov.nasa.worldwind.View, double)} instead.
     */
    public static double limitZoom(double value, OrbitViewLimits viewLimits)
    {
        if (viewLimits == null)
        {
            String message = Logging.getMessage("nullValue.ViewLimitsIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        double[] limits = viewLimits.getZoomLimits();
        double newValue = value;

        if (value < limits[0])
        {
            newValue = limits[0];
        }
        else if (value > limits[1])
        {
            newValue = limits[1];
        }

        return newValue;
    }

    //**************************************************************//
    //******************** Restorable State  ***********************//
    //**************************************************************//

    public void getRestorableState(RestorableSupport rs, RestorableSupport.StateObject context)
    {
        base.getRestorableState(rs, context);

        rs.addStateValueAsSector(context, "centerLocationLimits", this.centerLocationLimits);
        rs.addStateValueAsDouble(context, "minCenterElevation", this.minCenterElevation);
        rs.addStateValueAsDouble(context, "maxCenterElevation", this.maxCenterElevation);
        rs.addStateValueAsDouble(context, "minZoom", this.minZoom);
        rs.addStateValueAsDouble(context, "maxZoom", this.maxZoom);
    }

    public void restoreState(RestorableSupport rs, RestorableSupport.StateObject context)
    {
      base.restoreState(rs, context);

        Sector sector = rs.getStateValueAsSector(context, "centerLocationLimits");
        if (sector != null)
            this.setCenterLocationLimits(sector);

        // Min and max center elevation.
        double[] minAndMaxValue = this.getCenterElevationLimits();
        Double min = rs.getStateValueAsDouble(context, "minCenterElevation");
        if (min != null)
            minAndMaxValue[0] = min;

        Double max = rs.getStateValueAsDouble(context, "maxCenterElevation");
        if (max != null)
            minAndMaxValue[1] = max;

        if (min != null || max != null)
            this.setCenterElevationLimits(minAndMaxValue[0], minAndMaxValue[1]);

        // Min and max zoom value.        
        minAndMaxValue = this.getZoomLimits();
        min = rs.getStateValueAsDouble(context, "minZoom");
        if (min != null)
            minAndMaxValue[0] = min;

        max = rs.getStateValueAsDouble(context, "maxZoom");
        if (max != null)
            minAndMaxValue[1] = max;

        if (min != null || max != null)
            this.setZoomLimits(minAndMaxValue[0], minAndMaxValue[1]);
    }
}
}
