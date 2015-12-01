/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.EventListener;
namespace SharpEarth.events{


/**
 * @author tag
 * @version $Id: PositionListener.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface PositionListener extends EventListener
{
    public void moved(PositionEvent event);
}
}
