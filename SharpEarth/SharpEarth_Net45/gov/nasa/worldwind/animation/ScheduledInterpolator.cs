/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using SharpEarth.util;
using java;

namespace SharpEarth.animation{



/**
 * @author jym
 * @version $Id: ScheduledInterpolator.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class ScheduledInterpolator : Interpolator
{
    private long startTime = -1;
    private readonly long length;

    public ScheduledInterpolator(long lengthMillis) : this(null, lengthMillis)
    {
    }

    public ScheduledInterpolator(DateTime? startTime, long lengthMillis)
    {
        if (lengthMillis < 0)
        {
            string message = Logging.getMessage("generic.ArgumentOutOfRange", lengthMillis);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (startTime != null)
            this.startTime = startTime.Value.getTime();
        this.length = lengthMillis;
    }

    public ScheduledInterpolator( DateTime startTime, DateTime stopTime )
    {
        if (startTime.after(stopTime))
        {
        string message = Logging.getMessage("generic.ArgumentOutOfRange", startTime);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.startTime = startTime.getTime();
        this.length = stopTime.getTime() - startTime.getTime();
    }

    public double nextInterpolant()
    {
      long currentTime = java.System.currentTimeMillis();
        // When no start time is specified, begin counting time on the first run.
        if (this.startTime < 0)
            this.startTime = currentTime;
        // Exit when current time is before starting time.
        if (currentTime < this.startTime)
            return 0;

        long elapsedTime = currentTime - this.startTime;
        double unclampedInterpolant = ((double) elapsedTime) / ((double) this.length);
        return AnimationSupport.clampDouble(unclampedInterpolant, 0, 1);
    }

}
}
