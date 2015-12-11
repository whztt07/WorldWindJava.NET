/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using System;

namespace SharpEarth.exception{

  /**
   * Indicates that a value, request or other item or action is not recognized.
   *
   * @author tag
   * @version $Id: WWUnrecognizedException.java 1171 2013-02-11 21:45:02Z dcollins $
   */
  public class WWUnrecognizedException : WWRuntimeException
  {
    private string msg;
    /**
     * Construct an exception with a message string.
     *
     * @param msg the message.
     */
    public WWUnrecognizedException(string msg)
      : base(msg)
    {
        
    }

    /**
     * Construct an exception with a message string and a intial-cause exception.
     *
     * @param msg the message.
     * @param t   the exception causing this exception.
     */
    public WWUnrecognizedException(string msg, Exception t)
      : base(msg, t)
    {
        
    }
  }
}
