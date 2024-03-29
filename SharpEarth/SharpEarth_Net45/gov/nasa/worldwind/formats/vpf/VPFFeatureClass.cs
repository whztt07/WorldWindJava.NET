/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util;
using SharpEarth.avlist.AVListImpl;
namespace SharpEarth.formats.vpf{



/**
 * @author dcollins
 * @version $Id: VPFFeatureClass.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFFeatureClass : AVListImpl
{
    protected VPFCoverage coverage;
    protected VPFFeatureClassSchema schema;
    protected VPFRelation[] relations;
    protected String joinTableName;
    protected String primitiveTableName;

    public VPFFeatureClass(VPFCoverage coverage, VPFFeatureClassSchema schema, String joinTableName,
        String primitiveTableName)
    {
        this.coverage = coverage;
        this.schema = schema;
        this.joinTableName = joinTableName;
        this.primitiveTableName = primitiveTableName;
    }

    public VPFCoverage getCoverage()
    {
        return this.coverage;
    }

    public VPFFeatureClassSchema getSchema()
    {
        return this.schema;
    }

    public String getClassName()
    {
        return this.schema.getClassName();
    }

    public VPFFeatureType getType()
    {
        return this.schema.getType();
    }

    public String getFeatureTableName()
    {
        return this.schema.getFeatureTableName();
    }

    public String getJoinTableName()
    {
        return this.joinTableName;
    }

    public String getPrimitiveTableName()
    {
        return this.primitiveTableName;
    }

    public VPFRelation[] getRelations()
    {
        return this.relations;
    }

    public void setRelations(VPFRelation[] relations)
    {
        this.relations = relations;
    }

    public Collection<? extends VPFFeature> createFeatures(VPFFeatureFactory factory)
    {
        return null;
    }

    public Collection<? extends VPFSymbol> createFeatureSymbols(VPFSymbolFactory factory)
    {
        return null;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || this.GetType() != o.GetType())
            return false;

        VPFFeatureClass that = (VPFFeatureClass) o;

        if (this.coverage != null ? !this.coverage.getFilePath().Equals(that.coverage.getFilePath())
            : that.coverage != null)
            return false;
        if (this.schema != null ? !this.schema.Equals(that.schema) : that.schema != null)
            return false;
        if (!Arrays.Equals(this.relations, that.relations))
            return false;
        if (this.joinTableName != null ? !this.joinTableName.Equals(that.joinTableName) : that.joinTableName != null)
            return false;
        //noinspection RedundantIfStatement
        if (this.primitiveTableName != null ? !this.primitiveTableName.Equals(that.primitiveTableName)
            : that.primitiveTableName != null)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        int result = this.coverage != null ? this.coverage.GetHashCode() : 0;
        result = 31 * result + (this.schema != null ? this.schema.GetHashCode() : 0);
        result = 31 * result + (this.relations != null ? Arrays.hashCode(this.relations) : 0);
        result = 31 * result + (this.joinTableName != null ? this.joinTableName.GetHashCode() : 0);
        result = 31 * result + (this.primitiveTableName != null ? this.primitiveTableName.GetHashCode() : 0);
        return result;
    }
}
}
