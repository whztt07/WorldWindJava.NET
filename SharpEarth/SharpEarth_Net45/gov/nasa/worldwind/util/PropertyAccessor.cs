/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.geom;
namespace SharpEarth.util{


/**
 * @author jym
 * @version $Id: PropertyAccessor.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PropertyAccessor
{
    public interface AngleAccessor
    {
        Angle getAngle();

        bool setAngle(Angle value);
    }

    public interface DoubleAccessor
    {
        double getDouble();

        bool setDouble( double value );
    }

    public interface PositionAccessor
    {
        Position getPosition();

        bool setPosition(Position value);
    }
}
}
