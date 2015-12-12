/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.collada{

/**
 * Represents the COLLADA <i>image</i> element and provides access to its contents.
 *
 * @author pabercrombie
 * @version $Id: ColladaImage.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class ColladaImage : ColladaAbstractObject
{
    public ColladaImage(String ns)
    {
        super(ns);
    }

    public String getInitFrom()
    {
        return (String) this.getField("init_from");
    }
}
}
