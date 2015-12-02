/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;

namespace SharpEarth.exception{

/**
 * @author Tom Gaskins
 * @version $Id: WWRuntimeException.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class WWRuntimeException : Exception
{
    public WWRuntimeException()
    {
    }

    public WWRuntimeException(string s) : base(s)
    {
    }

    public WWRuntimeException(string s, Exception innerException) : base(s, innerException)
    {
    }

    public WWRuntimeException(Exception innerException ) : base("", innerException )
    {
    }
}
}
