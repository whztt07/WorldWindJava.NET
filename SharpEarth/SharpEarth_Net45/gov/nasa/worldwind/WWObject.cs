/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.events;
using SharpEarth.avlist;
using java.beans;

namespace SharpEarth{


/**
 * An interface provided by the major World Wind components to provide attribute-value list management and
 * property change management. Classifies implementers as property-change listeners, allowing them to receive
 * property-change events.
 *
 * @author Tom Gaskins
 * @version $Id: WWObject.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface WWObject : AVList, PropertyChangeListener, MessageListener
{
}
}
