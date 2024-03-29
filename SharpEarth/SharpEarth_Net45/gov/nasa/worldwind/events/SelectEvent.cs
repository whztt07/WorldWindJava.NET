/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using java.awt.events;
using SharpEarth.util;
using SharpEarth.pick;
namespace SharpEarth.events{



/**
 * This class signals that an object or terrain is under the cursor and identifies that object and the operation that
 * caused the signal. See the <em>Field Summary</em> for a description of the possible operations. When a SelectEvent
 * occurs, all select event listeners registered with the associated {@link SharpEarth.WorldWindow} are called.
 * Select event listeners are registered by calling {@link SharpEarth.WorldWindow#addSelectListener(SelectListener)}.
 * <p/>
 * A <code>ROLLOVER</code> SelectEvent is generated every frame when the cursor is over a visible object either because
 * the user moved it there or because the World Window was repainted and a visible object was found to be under the
 * cursor. A <code>ROLLOVER</code> SelectEvent is also generated when there are no longer any objects under the cursor.
 * Select events generated for objects under the cursor have a non-null pickPoint, and contain the top-most visible
 * object of all objects at the cursor position.
 * <p/>
 * A <code>BOX_ROLLOVER</code> SelectEvent is generated every frame when the selection box intersects a visible object
 * either because the user moved or expanded it or because the World Window was repainted and a visible object was found
 * to intersect the box. A <code>BOX_ROLLOVER</code> SelectEvent is also generated when there are no longer any objects
 * intersecting the selection box. Select events generated for objects intersecting the selection box have a non-null
 * pickRectangle, and contain all top-most visible objects of all objects intersecting the selection box.
 * <p/>
 * If a select listener performs some action in response to a select event, it should call the event's {@link
 * #consume()} method in order to indicate to subsequently called listeners that the event has been responded to and no
 * further action should be taken. Left press select events should not be consumed unless it is necessary to do so.
 * Consuming left press events prevents the WorldWindow from gaining focus, thereby preventing it from receiving key
 * events.
 * <p/>
 * If no object is under the cursor but the cursor is over terrain, the select event will identify the terrain as the
 * picked object and will include the corresponding geographic position. See {@link
 * SharpEarth.pick.PickedObject#isTerrain()}.
 *
 * @author tag
 * @version $Id: SelectEvent.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class SelectEvent : WWEvent
{
    /** The user clicked the left mouse button while the cursor was over picked object. */
    public static readonly string LEFT_CLICK = "gov.nasa.worldwind.SelectEvent.LeftClick";
    /** The user double-clicked the left mouse button while the cursor was over picked object. */
    public static readonly string LEFT_DOUBLE_CLICK = "gov.nasa.worldwind.SelectEvent.LeftDoubleClick";
    /** The user clicked the right mouse button while the cursor was over picked object. */
    public static readonly string RIGHT_CLICK = "gov.nasa.worldwind.SelectEvent.RightClick";
    /** The user pressed the left mouse button while the cursor was over picked object. */
    public static readonly string LEFT_PRESS = "gov.nasa.worldwind.SelectEvent.LeftPress";
    /** The user pressed the right mouse button while the cursor was over picked object. */
    public static readonly string RIGHT_PRESS = "gov.nasa.worldwind.SelectEvent.RightPress";
    /**
     * The cursor has moved over the picked object and become stationary, or has moved off the object of the most recent
     * <code>HOVER</code> event. In the latter case, the picked object will be null.
     */
    public static readonly string HOVER = "gov.nasa.worldwind.SelectEvent.Hover";
    /**
     * The cursor has moved over the object or has moved off the object most recently rolled over. In the latter case
     * the picked object will be null.
     */
    public static readonly string ROLLOVER = "gov.nasa.worldwind.SelectEvent.Rollover";
    /** The user is attempting to drag the picked object. */
    public static readonly string DRAG = "gov.nasa.worldwind.SelectEvent.Drag";
    /** The user has stopped dragging the picked object. */
    public static readonly string DRAG_END = "gov.nasa.worldwind.SelectEvent.DragEnd";
    /**
     * The user has selected one or more of objects using a selection box. A box rollover event is generated every frame
     * if one or more objects intersect the box, in which case the event's pickedObjects list contain the selected
     * objects. A box rollover event is generated once when the selection becomes empty, in which case the event's
     * pickedObjects is <code>null</code>. In either case, the event's pickRect contains the selection box bounds in AWT
     * screen coordinates.
     */
    public static readonly string BOX_ROLLOVER = "gov.nasa.worldwind.SelectEvent.BoxRollover";

    private readonly string eventAction;
    private readonly Point pickPoint;
    private readonly Rectangle pickRect;
    private readonly MouseEvent mouseEvent;
    private readonly PickedObjectList pickedObjects;

    public SelectEvent(object source, string eventAction, MouseEvent mouseEvent, PickedObjectList pickedObjects) : base(source)
    {
        this.eventAction = eventAction;
        this.pickPoint = mouseEvent != null ? mouseEvent.getPoint() : null;
        this.pickRect = default(Rectangle);
        this.mouseEvent = mouseEvent;
        this.pickedObjects = pickedObjects;
    }

    public SelectEvent(Object source, String eventAction, Point pickPoint, PickedObjectList pickedObjects) : base(source)
    {
        this.eventAction = eventAction;
        this.pickPoint = pickPoint;
        this.pickRect = default( Rectangle );
        this.mouseEvent = null;
        this.pickedObjects = pickedObjects;
    }

    public SelectEvent(Object source, String eventAction, Rectangle pickRectangle, PickedObjectList pickedObjects) : base(source)
    {
        this.eventAction = eventAction;
        this.pickPoint = default( Point);
        this.pickRect = pickRectangle;
        this.mouseEvent = null;
        this.pickedObjects = pickedObjects;
    }

    public new void consume()
    {
        base.consume();
        if (this.getMouseEvent() != null)
            this.getMouseEvent().consume();
    }

    public String getEventAction()
    {
        return this.eventAction ?? "gov.nasa.worldwind.SelectEvent.UnknownEventAction";
    }

    public Point getPickPoint()
    {
        return this.pickPoint;
    }

    public Rectangle getPickRectangle()
    {
        return this.pickRect;
    }

    public MouseEvent getMouseEvent()
    {
        return this.mouseEvent;
    }

    public bool hasObjects()
    {
        return this.pickedObjects != null && this.pickedObjects.Count > 0;
    }

    public PickedObjectList getObjects()
    {
        return this.pickedObjects;
    }

    public PickedObject getTopPickedObject()
    {
        return this.hasObjects() ? this.pickedObjects.getTopPickedObject() : null;
    }

    public Object getTopObject()
    {
        PickedObject tpo = this.getTopPickedObject();
        return tpo != null ? tpo.getObject() : null;
    }

    /**
     * Returns a list of all picked objects in this event's picked object list who's onTop flag is set to true. This
     * returns <code>null</code> if this event's picked object list is empty, or does not contain any picked objects
     * marked as on top.
     *
     * @return a new list of the picked objects marked as on top, or <code>null</code> if nothing is marked as on top.
     */
    public List<PickedObject> getAllTopPickedObjects()
    {
        return this.hasObjects() ? this.pickedObjects.getAllTopPickedObjects() : null;
    }

    /**
     * Returns a list of all objects associated with a picked object in this event's picked object list who's onTop flag
     * is set to true. This returns <code>null</code> if this event's picked object list is empty, or does not contain
     * any picked objects marked as on top.
     *
     * @return a new list of the objects associated with a picked object marked as on top, or <code>null</code> if
     *         nothing is marked as on top.
     */
    public List<object> getAllTopObjects()
    {
        return this.hasObjects() ? this.pickedObjects.getAllTopObjects() : null;
    }

    public bool isRollover()
    {
        return this.getEventAction() == ROLLOVER;
    }

    public bool isHover()
    {
        return this.getEventAction() == HOVER;
    }

    public bool isDragEnd()
    {
        return this.getEventAction() == DRAG_END;
    }

    public bool isDrag()
    {
        return this.getEventAction() == DRAG;
    }

    public bool isRightPress()
    {
        return this.getEventAction() == RIGHT_PRESS;
    }

    public bool isRightClick()
    {
        return this.getEventAction() == RIGHT_CLICK;
    }

    public bool isLeftDoubleClick()
    {
        return this.getEventAction() == LEFT_DOUBLE_CLICK;
    }

    public bool isLeftClick()
    {
        return this.getEventAction() == LEFT_CLICK;
    }

    public bool isLeftPress()
    {
        return this.getEventAction() == LEFT_PRESS;
    }

    public bool isBoxSelect()
    {
        return this.getEventAction() == BOX_ROLLOVER;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(this.GetType().Name + " "
            + (this.eventAction != null ? this.eventAction : Logging.getMessage("generic.Unknown")));
        if (this.pickedObjects != null && this.pickedObjects.getTopObject() != null)
            sb.Append(", ").Append( this.pickedObjects.getTopObject().GetType().Name);

        return sb.ToString();
    }
}
}
