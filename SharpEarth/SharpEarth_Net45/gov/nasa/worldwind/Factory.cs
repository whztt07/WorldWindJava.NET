/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using SharpEarth.avlist;

namespace SharpEarth{


/**
 * General factory interface.
 *
 * @author tag
 * @version $Id: Factory.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface Factory
{
    /**
     * Creates an object from a general configuration source.
     *
     * @param configSource the configuration source.
     * @param parameters       properties to apply during object creation.
     *
     * @return the new object.
     *
     * @throws ArgumentException if the configuration source is null or an empty string.
     * @throws SharpEarth.exception.WWUnrecognizedException
     *                                  if the type of source or some object-specific value is unrecognized.
     * @throws SharpEarth.exception.WWRuntimeException
     *                                  if object creation fails. The exception indicating the source of the failure is
     *                                  included as the {@link Exception#initCause(Throwable)}.
     */
    object createFromConfigSource(object configSource, AVList parameters);
}
}
