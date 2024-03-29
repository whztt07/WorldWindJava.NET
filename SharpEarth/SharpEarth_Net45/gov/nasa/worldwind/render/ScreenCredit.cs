/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.awt;
namespace SharpEarth.render{


/**
 * @author tag
 * @version $Id: ScreenCredit.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface ScreenCredit : Renderable
{
    void setViewport(Rectangle viewport);

    Rectangle getViewport();

    void setOpacity(double opacity);

    double getOpacity();

    void setLink(String link);

    String getLink();

    public void pick(DrawContext dc, java.awt.Point pickPoint);
}
}
