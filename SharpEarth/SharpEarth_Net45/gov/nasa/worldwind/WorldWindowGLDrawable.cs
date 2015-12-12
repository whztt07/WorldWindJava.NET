/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using javax.media.opengl.GLAutoDrawable;
using SharpEarth.cache;
namespace SharpEarth{



/**
 * @author tag
 * @version $Id: WorldWindowGLDrawable.java 1855 2014-02-28 23:01:02Z tgaskins $
 */
public interface WorldWindowGLDrawable : WorldWindow
{
    void initDrawable(GLAutoDrawable glAutoDrawable);

    void initGpuResourceCache(GpuResourceCache cache);

    void endInitialization();
}
}
