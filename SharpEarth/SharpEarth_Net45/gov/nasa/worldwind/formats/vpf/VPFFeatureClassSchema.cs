/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.formats.vpf{

/**
 * @author dcollins
 * @version $Id: VPFFeatureClassSchema.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFFeatureClassSchema
{
    protected String className;
    protected VPFFeatureType type;
    protected String featureTableName;

    public VPFFeatureClassSchema(String className, VPFFeatureType type, String featureTableName)
    {
        this.className = className;
        this.type = type;
        this.featureTableName = featureTableName;
    }

    public String getClassName()
    {
        return this.className;
    }

    public VPFFeatureType getType()
    {
        return this.type;
    }

    public String getFeatureTableName()
    {
        return this.featureTableName;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || this.GetType() != o.GetType())
            return false;

        VPFFeatureClassSchema that = (VPFFeatureClassSchema) o;

        if (this.className != null ? !this.className.Equals(that.className) : that.className != null)
            return false;
        if (this.featureTableName != null ? !this.featureTableName.Equals(that.featureTableName)
            : that.featureTableName != null)
            return false;
        //noinspection RedundantIfStatement
        if (this.type != null ? !this.type.Equals(that.type) : that.type != null)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        int result = this.className != null ? this.className.GetHashCode() : 0;
        result = 31 * result + (this.type != null ? this.type.GetHashCode() : 0);
        result = 31 * result + (this.featureTableName != null ? this.featureTableName.GetHashCode() : 0);
        return result;
    }
}
}
