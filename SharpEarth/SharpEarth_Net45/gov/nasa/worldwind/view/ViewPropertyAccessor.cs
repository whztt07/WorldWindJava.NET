/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.geom;
using SharpEarth.util;

namespace SharpEarth.view
{
/**
 * @author jym
 * @version $Id: ViewPropertyAccessor.java 1171 2013-02-11 21:45:02Z dcollins $
 */

  public class ViewPropertyAccessor
  {
    public static PropertyAccessor.DoubleAccessor createElevationAccessor( View view )
    {
      return new ElevationAccessor( view );
    }

    public static PropertyAccessor.AngleAccessor createHeadingAccessor( View view )
    {
      return new HeadingAccessor( view );
    }

    public static PropertyAccessor.AngleAccessor createPitchAccessor( View view )
    {
      return new PitchAccessor( view );
    }

    public static PropertyAccessor.AngleAccessor createRollAccessor( View view )
    {
      return new RollAccessor( view );
    }

    public static PropertyAccessor.PositionAccessor createEyePositionAccessor( View view )
    {
      return new EyePositionAccessor( view );
    }

    public class HeadingAccessor : PropertyAccessor.AngleAccessor
    {
      protected View view;

      public HeadingAccessor( View view )
      {
        this.view = view;
      }

      public Angle getAngle()
      {
        return view != null ? view.getHeading() : null;
      }

      public bool setAngle( Angle value )
      {
        if ( view == null || value == null )
          return false;
        try
        {
          view.setHeading( value );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }

    public class PitchAccessor : PropertyAccessor.AngleAccessor
    {
      protected View view;

      public PitchAccessor( View view )
      {
        this.view = view;
      }

      public Angle getAngle()
      {
        if ( view == null )
          return null;

        return view.getPitch();
      }

      public bool setAngle( Angle value )
      {
        //noinspection SimplifiableIfStatement
        if ( view == null || value == null )
          return false;

        try
        {
          view.setPitch( value );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }

    public class RollAccessor : PropertyAccessor.AngleAccessor
    {
      protected View view;

      public RollAccessor( View view )
      {
        this.view = view;
      }

      public Angle getAngle()
      {
        return view != null ? view.getRoll() : null;
      }

      public bool setAngle( Angle value )
      {
        if ( view == null || value == null )
          return false;
        try
        {
          view.setRoll( value );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }

    public class EyePositionAccessor : PropertyAccessor.PositionAccessor
    {
      protected View view;

      public EyePositionAccessor( View view )
      {
        this.view = view;
      }

      public Position getPosition()
      {
        return view != null ? view.getEyePosition() : null;
      }

      public bool setPosition( Position value )
      {
        if ( view == null || value == null )
          return false;
        try
        {
          view.setEyePosition( value );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }

    public class ElevationAccessor : PropertyAccessor.DoubleAccessor
    {
      protected View view;

      public ElevationAccessor( View view )
      {
        this.view = view;
      }

      public double? getDouble()
      {
        if ( view == null )
          return null;
        return view.getEyePosition().getElevation();
      }

      public bool setDouble( double? value )
      {
        if ( view == null || value == null )
          return false;
        try
        {
          view.setEyePosition( new Position( view.getCurrentEyePosition(), value.Value ) );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }
  }
}