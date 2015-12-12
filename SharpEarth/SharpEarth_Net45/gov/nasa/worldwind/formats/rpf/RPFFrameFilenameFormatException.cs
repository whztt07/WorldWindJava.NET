/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.formats.rpf{

/**
 * @author dcollins
 * @version $Id: RPFFrameFilenameFormatException.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class RPFFrameFilenameFormatException : ArgumentException
{
    public RPFFrameFilenameFormatException()
    {
    }

    public RPFFrameFilenameFormatException(String message)
    {
        base(message);
    }

    public RPFFrameFilenameFormatException(String message, Throwable cause)
    {
        base(message, cause);
    }

    public RPFFrameFilenameFormatException(Throwable cause)
    {
        base(cause);
    }
}
}
