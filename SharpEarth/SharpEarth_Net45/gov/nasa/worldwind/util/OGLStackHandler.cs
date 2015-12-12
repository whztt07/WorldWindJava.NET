/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util;
using SharpEarth.java.util;

namespace SharpEarth.util{


/**
 * @author tag
 * @version $Id: OGLStackHandler.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class OGLStackHandler
{
    private bool attribsPushed;
    private bool clientAttribsPushed;
    private bool modelviewPushed;
    private bool projectionPushed;
    private bool texturePushed;

    public void clear()
    {
        this.attribsPushed = false;
        this.clientAttribsPushed = false;
        this.modelviewPushed = false;
        this.projectionPushed = false;
        this.texturePushed = false;
    }

    public bool isActive()
    {
        return this.attribsPushed || this.clientAttribsPushed || this.modelviewPushed || this.projectionPushed
            || this.texturePushed;
    }

    public void pushAttrib(GL2 gl, uint mask)
    {
        gl.PushAttrib(mask);
        this.attribsPushed = true;
    }

    public void pushClientAttrib(GL2 gl, uint mask)
    {
        gl.PushClientAttrib(mask);
        this.clientAttribsPushed = true;
    }

    public void pushModelview(GL2 gl)
    {
      gl.MatrixMode(GL2.GL_MODELVIEW);
      gl.PushMatrix();
      this.modelviewPushed = true;
    }

    public void pushProjection(GL2 gl)
    {
        gl.MatrixMode(GL2.GL_PROJECTION);
        gl.PushMatrix();
        this.projectionPushed = true;
    }

    public void pushTexture(GL2 gl)
    {
        gl.MatrixMode(GL2.GL_TEXTURE);
        gl.PushMatrix();
        this.texturePushed = true;
    }

    public void pop(GL2 gl)
    {
        if (this.attribsPushed)
        {
          gl.PopAttrib();
          this.attribsPushed = false;
        }

        if (this.clientAttribsPushed)
        {
            gl.PopClientAttrib();
            this.clientAttribsPushed = false;
        }

        if (this.modelviewPushed)
        {
            gl.MatrixMode(GL2.GL_MODELVIEW);
            gl.PopMatrix();
            this.modelviewPushed = false;
        }

        if (this.projectionPushed)
        {
            gl.MatrixMode(GL2.GL_PROJECTION);
            gl.PopAttrib();
            this.projectionPushed = false;
        }

        if (this.texturePushed)
        {
            gl.MatrixMode(GL2.GL_TEXTURE);
            gl.PopMatrix();
            this.texturePushed = false;
        }
    }

    public void pushModelviewIdentity(GL2 gl)
    {
        gl.MatrixMode(GL2.GL_MODELVIEW);
        this.modelviewPushed = true;
        gl.PushMatrix();
        gl.LoadIdentity();
    }

    public void pushProjectionIdentity(GL2 gl)
    {
        gl.MatrixMode(GL2.GL_PROJECTION);
        this.projectionPushed = true;
        gl.PushMatrix();
        gl.LoadIdentity();
    }

    public void pushTextureIdentity(GL2 gl)
    {
        gl.MatrixMode(GL2.GL_TEXTURE);
        this.texturePushed = true;
        gl.PushMatrix();
        gl.LoadIdentity();
    }
}
}
