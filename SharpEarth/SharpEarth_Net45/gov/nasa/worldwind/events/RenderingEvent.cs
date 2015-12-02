/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;

namespace SharpEarth.events{


/**
 * @author tag
 * @version $Id: RenderingEvent.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class RenderingEvent : WWEvent
{
    public static readonly string BEFORE_RENDERING = "gov.nasa.worldwind.RenderingEvent.BeforeRendering";
    public static readonly string BEFORE_BUFFER_SWAP = "gov.nasa.worldwind.RenderingEvent.BeforeBufferSwap";
    public static readonly string AFTER_BUFFER_SWAP = "gov.nasa.worldwind.RenderingEvent.AfterBufferSwap";

    private string stage;

    public RenderingEvent(object source, string stage) : base(source)
    {
        this.stage = stage;
    }

    public string getStage()
    {
        return this.stage ?? "gov.nasa.worldwind.RenderingEvent.UnknownStage";
    }

    public override string ToString()
    {
        return this.GetType().Name + " "
            + (this.stage ?? Logging.getMessage("generic.Unknown"));
    }
}
}
