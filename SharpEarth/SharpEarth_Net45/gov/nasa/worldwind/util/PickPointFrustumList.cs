/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.ArrayList;
using java.awt;
using SharpEarth.geom;
namespace SharpEarth.util{



/**
 * @author Jeff Addison
 * @version $Id: PickPointFrustumList.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PickPointFrustumList extends ArrayList<PickPointFrustum>
{
    public PickPointFrustumList()
    {
    }

    public PickPointFrustumList(PickPointFrustumList list)
    {
        super(list);
    }

    /**
     * Returns true if the specified point is inside the space enclosed by ALL of the frustums
     *
     * @param point the point to test.
     *
     * @return true if the specified point is inside the space enclosed by ALL the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool containsInAll(Vec4 point)
    {
        if (point == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (!frustum.contains(point))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * Returns true if the specified point is inside the space enclosed by ANY of the frustums
     *
     * @param point the point to test.
     *
     * @return true if the specified point is inside the space enclosed by ANY the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool containsInAny(Vec4 point)
    {
        if (point == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (frustum.contains(point))
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Returns true if the specified 2D screen point is inside the 2D screen rectangle enclosed by ALL of the frustums
     *
     * @param point the point to test.
     *
     * @return true if the specified point is inside the space enclosed by ALL the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool containsInAll(Point point)
    {
        if (point == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (!frustum.contains(point))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * Returns true if the specified 2D point is inside the 2D screen rectangle enclosed by ANY of the frustums
     *
     * @param x the x coordinate to test.
     * @param y the y coordinate to test.
     *
     * @return true if the specified point is inside the space enclosed by ANY the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool containsInAny(double x, double y)
    {
        for (PickPointFrustum frustum : this)
        {
            if (frustum.contains(x, y))
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Returns true if the specified 2D point is inside the 2D screen rectangle enclosed by ANY of the frustums
     *
     * @param point the point to test.
     *
     * @return true if the specified point is inside the space enclosed by ANY the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool containsInAny(Point point)
    {
        if (point == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (frustum.contains(point))
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Returns true if the specified {@link Extent} intersects the space enclosed by ALL the Frustums. NOTE: Cannot be
     * true if all frustums do not intersect.
     *
     * @param extent the Extent to test.
     *
     * @return true if the specified Extent intersects the space enclosed by ALL Frustums, and false otherwise.
     *
     * @throws ArgumentException if the extent is null.
     */
    public final bool intersectsAll(Extent extent)
    {
        if (extent == null)
        {
            String msg = Logging.getMessage("nullValue.ExtentIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (!frustum.intersects(extent))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * Returns true if the specified {@link Extent} intersects the space enclosed by ANY of the Frustums.
     *
     * @param extent the Extent to test.
     *
     * @return true if the specified Extent intersects the space enclosed by ANY of the Frustums, and false otherwise.
     *
     * @throws ArgumentException if the extent is null.
     */
    public final bool intersectsAny(Extent extent)
    {
        if (extent == null)
        {
            String msg = Logging.getMessage("nullValue.ExtentIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (frustum.intersects(extent))
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Returns true if a specified line segment intersects the space enclosed by ANY of the Frustums.
     *
     * @param pa one end of the segment.
     * @param pb the other end of the segment.
     *
     * @return true if the specified segment intersects the space enclosed by ANY of the Frustums, otherwise false.
     *
     * @throws ArgumentException if either point is null.
     */
    public final bool intersectsAny(Vec4 pa, Vec4 pb)
    {
        for (PickPointFrustum frustum : this)
        {
            if (frustum.intersectsSegment(pa, pb))
                return true;
        }

        return false;
    }

    /**
     * Returns true if the specified {@link java.awt.Rectangle} intersects the 2D screen space enclosed by ALL the
     * Frustums. NOTE: Cannot be true if all frustums do not intersect.
     *
     * @param rect the Rectangle to test.
     *
     * @return true if the specified Rectangle intersects the 2D screen space enclosed by ALL Frustums, and false
     *         otherwise.
     *
     * @throws ArgumentException if the extent is null.
     */
    public final bool intersectsAll(Rectangle rect)
    {
        if (rect == null)
        {
            String msg = Logging.getMessage("nullValue.RectangleIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (!frustum.intersects(rect))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * Returns true if the specified {@link java.awt.Rectangle} intersects the 2D screen space enclosed by ANY of the
     * Frustums.
     *
     * @param rect the Rectangle to test.
     *
     * @return true if the specified Rectangle intersects the 2D screen space enclosed by ANY of the Frustums, and false
     *         otherwise.
     *
     * @throws ArgumentException if the extent is null.
     */
    public final bool intersectsAny(Rectangle rect)
    {
        if (rect == null)
        {
            String msg = Logging.getMessage("nullValue.RectangleIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        for (PickPointFrustum frustum : this)
        {
            if (frustum.intersects(rect))
            {
                return true;
            }
        }

        return false;
    }
}
}
