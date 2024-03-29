/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using SharpEarth.avlist.AVListImpl;
namespace SharpEarth.formats.vpf{


/**
 * @author dcollins
 * @version $Id: VPFFeature.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFFeature : AVListImpl
{
    protected VPFFeatureClass featureClass;
    protected int id;
    protected VPFBoundingBox bounds;
    private int[] primitiveIds;

    public VPFFeature(VPFFeatureClass featureClass, int id, VPFBoundingBox bounds, int[] primitiveIds)
    {
        if (featureClass == null)
        {
            String message = Logging.getMessage("nullValue.FeatureClassIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (bounds == null)
        {
            String message = Logging.getMessage("nullValue.BoundingBoxIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.featureClass = featureClass;
        this.id = id;
        this.bounds = bounds;
        this.primitiveIds = primitiveIds;
    }

    public VPFFeatureClass getFeatureClass()
    {
        return this.featureClass;
    }

    public VPFFeatureType getType()
    {
        return this.featureClass.getType();
    }

    public int getId()
    {
        return this.id;
    }

    public VPFBoundingBox getBounds()
    {
        return this.bounds;
    }

    public int[] getPrimitiveIds()
    {
        return this.primitiveIds;
    }
}
}
