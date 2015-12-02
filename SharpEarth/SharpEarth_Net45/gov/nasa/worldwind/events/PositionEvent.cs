/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Drawing;
using SharpEarth.geom;

namespace SharpEarth.events{


/**
 * @author tag
 * @version $Id: PositionEvent.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PositionEvent : WWEvent
{
    private readonly Point screenPoint;
    private readonly Position position;
    private readonly Position previousPosition;

    public PositionEvent(object source, Point screenPoint, Position previousPosition, Position position) : base(source)
    {
        this.screenPoint = screenPoint;
        this.position = position;
        this.previousPosition = previousPosition;
    }

    public Point getScreenPoint()
    {
        return screenPoint;
    }

    public Position getPosition()
    {
        return position;
    }

    public Position getPreviousPosition()
    {
        return previousPosition;
    }

  public override string ToString()
  {
        return this.GetType().Name + " "
            + (this.previousPosition != null ? this.previousPosition.ToString() : "null")
            + " --> "
            + (this.position != null ? this.position.ToString() : "null");
    }
}
}
