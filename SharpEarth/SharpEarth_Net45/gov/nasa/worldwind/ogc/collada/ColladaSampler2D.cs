/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.ogc.collada{

/**
 * Represents the COLLADA <i>sampler2D</i> element and provides access to its contents.
 *
 * @author pabercrombie
 * @version $Id: ColladaSampler2D.java 654 2012-06-25 04:15:52Z pabercrombie $
 */
public class ColladaSampler2D : ColladaAbstractObject
{
    /**
     * Construct an instance.
     *
     * @param ns the qualifying namespace URI. May be null to indicate no namespace qualification.
     */
    public ColladaSampler2D(String ns)
    {
        super(ns);
    }

    /**
     * Indicates the value of the <i>source</i> field.
     *
     * @return The value of the <i>source</i> field, or null if the field is not set.
     */
    public ColladaSource getSource()
    {
        return (ColladaSource) this.getField("source");
    }
}
}
