/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.util;
namespace SharpEarth.formats.nitfs{


/**
 * @author Lado Garakanidze
 * @version $Id: NITFSRuntimeException.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public sealed class NITFSRuntimeException : java.lang.RuntimeException
{
    public NITFSRuntimeException()
    {
        base();
    }

    public NITFSRuntimeException(String messageID)
    {
        base(Logging.getMessage(messageID));
        log(this.getMessage());
    }

    public NITFSRuntimeException(String messageID, String parameters)
    {
        base(Logging.getMessage(messageID) + parameters);
        log(this.getMessage());
    }

    public NITFSRuntimeException(Throwable throwable)
    {
        base(throwable);
        log(this.getMessage());
    }

    public NITFSRuntimeException(String messageID, Throwable throwable)
    {
        base(Logging.getMessage(messageID), throwable);
        log(this.getMessage());
    }

    public NITFSRuntimeException(String messageID, String parameters, Throwable throwable)
    {
        base(Logging.getMessage(messageID) + parameters, throwable);
        log(this.getMessage());
    }

    // TODO: Calling the logger from here causes the wrong method to be listed in the log record. Must call the
    // logger from the site with the problem and generating the exception.
    private void log(String s)
    {
        Logging.logger().fine(s);
    }
}
}
