/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SharpEarth.geom;
using SharpEarth.util;

namespace SharpEarth.view.orbit
{
/**
 * @author dcollins
 * @version $Id: OrbitViewPropertyAccessor.java 1171 2013-02-11 21:45:02Z dcollins $
 */
  public class OrbitViewPropertyAccessor : ViewPropertyAccessor
  {
    private OrbitViewPropertyAccessor()
    {
    }

    public static PropertyAccessor.PositionAccessor createCenterPositionAccessor( OrbitView view )
    {
      return new CenterPositionAccessor( view );
    }

    public static PropertyAccessor.DoubleAccessor createZoomAccessor( OrbitView view )
    {
      return new ZoomAccessor( view );
    }

    private class CenterPositionAccessor : PropertyAccessor.PositionAccessor
    {
      private readonly OrbitView orbitView;

      public CenterPositionAccessor( OrbitView view )
      {
        orbitView = view;
      }

      public Position getPosition()
      {
        if ( orbitView == null )
          return null;

        return orbitView.getCenterPosition();
      }

      public bool setPosition( Position value )
      {
        //noinspection SimplifiableIfStatement
        if ( orbitView == null || value == null )
          return false;

        try
        {
          orbitView.setCenterPosition( value );
          return true;
        }
        catch ( Exception e )
        {
          return false;
        }
      }
    }

    private class ZoomAccessor : PropertyAccessor.DoubleAccessor
    {
      private readonly OrbitView orbitView;

      public ZoomAccessor( OrbitView orbitView )
      {
        this.orbitView = orbitView;
      }

      public double? getDouble()
      {
        if ( orbitView == null )
          return null;

        return orbitView.getZoom();
      }

      public bool setDouble( double? value )
      {
        //noinspection SimplifiableIfStatement
        if ( orbitView == null || value == null )
          return false;

        try
        {
          orbitView.setZoom( value.Value );
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