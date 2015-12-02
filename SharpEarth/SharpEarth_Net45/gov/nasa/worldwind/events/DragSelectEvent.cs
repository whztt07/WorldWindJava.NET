/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System.Drawing;
using java.awt.events;
using SharpEarth.pick;
using SharpEarth.pick;
namespace SharpEarth.events{



/**
 * This class is a specialization of {@link SelectEvent} and includes the pick point screen position of the most recent
 * drag event prior to the current one.
 *
 * @author tag
 * @version $Id: DragSelectEvent.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class DragSelectEvent : SelectEvent
{
    private readonly Point previousPickPoint;

    public DragSelectEvent(object source, string eventAction, MouseEvent mouseEvent, PickedObjectList pickedObjects,
        Point previousPickPoint) : base( source, eventAction, mouseEvent, pickedObjects );
    {
        this.previousPickPoint = previousPickPoint;
    }

    /**
     * Indicates the most screen position of the drag event immediately prior to this one.
     *
     * @return the screen position of the event just prior to this one.
     */
    public Point getPreviousPickPoint()
    {
        return this.previousPickPoint;
    }
}
}
