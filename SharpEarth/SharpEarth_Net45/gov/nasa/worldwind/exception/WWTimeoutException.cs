/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.exception{

/**
 * Thrown when a World Wind operation times out.
 *
 * @author tag
 * @version $Id: WWTimeoutException.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class WWTimeoutException : WWRuntimeException
{
    public WWTimeoutException(String message)
    {
        base(message);
    }
}
}
