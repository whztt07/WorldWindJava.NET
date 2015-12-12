/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth
{
  public interface Disposable
  {
    /// <summary>
    /// Disposes of any internal resources allocated by the object.
    /// </summary>
    void dispose();
  }
}
