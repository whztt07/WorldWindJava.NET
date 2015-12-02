/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util.Logging;
using SharpEarth.globes.Globe;
using SharpEarth.geom;
namespace SharpEarth.formats.vpf{


/**
 * @author dcollins
 * @version $Id: VPFTile.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFTile implements ExtentHolder
{
    private int id;
    private String name;
    private VPFBoundingBox bounds;

    public VPFTile(int id, String name, VPFBoundingBox bounds)
    {
        if (name == null)
        {
            String message = Logging.getMessage("nullValue.NameIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (bounds == null)
        {
            String message = Logging.getMessage("nullValue.BoundingBoxIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.id = id;
        this.name = name;
        this.bounds = bounds;
    }

    public int getId()
    {
        return this.id;
    }

    public String getName()
    {
        return this.name;
    }

    public VPFBoundingBox getBounds()
    {
        return this.bounds;
    }

    public Extent getExtent(Globe globe, double verticalExaggeration)
    {
        if (globe == null)
        {
            String message = Logging.getMessage("nullValue.GlobeIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return Sector.computeBoundingCylinder(globe, verticalExaggeration, this.bounds.toSector());
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;

        VPFTile vpfTile = (VPFTile) o;

        if (id != vpfTile.id)
            return false;
        if (bounds != null ? !bounds.equals(vpfTile.bounds) : vpfTile.bounds != null)
            return false;
        //noinspection RedundantIfStatement
        if (name != null ? !name.equals(vpfTile.name) : vpfTile.name != null)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        int result = id;
        result = 31 * result + (name != null ? name.hashCode() : 0);
        result = 31 * result + (bounds != null ? bounds.hashCode() : 0);
        return result;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.append(this.id);
        sb.append(": ");
        sb.append(this.name);
        return sb.ToString();
    }
}
}
