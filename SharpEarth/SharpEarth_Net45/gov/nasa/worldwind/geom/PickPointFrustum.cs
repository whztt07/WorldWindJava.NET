/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.awt;
using SharpEarth.util;
namespace SharpEarth.geom{



/**
 * A viewport aligned {@link SharpEarth.geom.Frustum} that also stores the 2D screen rectangle that the {@link
 * SharpEarth.geom.Frustum} contains.
 *
 * @author Jeff Addison
 * @version $Id: PickPointFrustum.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class PickPointFrustum extends Frustum
{
    private final Rectangle screenRect;

    /**
     * Constructs a new PickPointFrustum from another Frustum and screen rectangle
     *
     * @param frustum frustum to create the PickPointFrustum from
     * @param rect    screen rectangle to store with this frustum
     */
    public PickPointFrustum(Frustum frustum, Rectangle rect)
    {
        super(frustum.getLeft(), frustum.getRight(), frustum.getBottom(), frustum.getTop(), frustum.getNear(),
            frustum.getFar());

        if (rect == null)
        {
            String message = Logging.getMessage("nullValue.RectangleIsNull");
            Logging.logger().fine(message);
            throw new ArgumentException(message);
        }

        this.screenRect = rect;
    }

    // ============== Intersection Functions ======================= //

    /**
     * Returns true if the specified 2D screen {@link java.awt.Rectangle} intersects the space enclosed by this view
     * aligned frustums screen rectangle.
     *
     * @param rect the rectangle to test
     *
     * @return true if the specified Rectangle intersects the space enclosed by this Frustum, and false otherwise.
     *
     * @throws ArgumentException if the extent is null.
     */
    public final bool intersects(Rectangle rect)
    {
        if (rect == null)
        {
            String message = Logging.getMessage("nullValue.RectangleIsNull");
            Logging.logger().fine(message);
            throw new ArgumentException(message);
        }

        return this.screenRect.intersects(rect);
    }

    /**
     * Returns true if the specified point is inside the 2D screen rectangle enclosed by this frustum
     *
     * @param x the x coordinate to test.
     * @param y the y coordinate to test.
     *
     * @return true if the specified point is inside the space enclosed by this Frustum, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool contains(double x, double y)
    {
        return this.screenRect.contains(x, y);
    }

    /**
     * Returns true if the specified point is inside the 2D screen rectangle enclosed by this frustum
     *
     * @param point the point to test.
     *
     * @return true if the specified point is inside the space enclosed by this Frustum, and false otherwise.
     *
     * @throws ArgumentException if the point is null.
     */
    public final bool contains(Point point)
    {
        if (point == null)
        {
            String msg = Logging.getMessage("nullValue.PointIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        return this.screenRect.contains(point);
    }

    /**
     * Returns a copy of this PickPointFrustum which is transformed by the specified Matrix.
     *
     * @param matrix the Matrix to transform this Frustum by.
     *
     * @return a copy of this Frustum, transformed by the specified Matrix.
     *
     * @throws ArgumentException if the matrix is null
     */
    public PickPointFrustum transformBy(Matrix matrix)
    {
        if (matrix == null)
        {
            String msg = Logging.getMessage("nullValue.MatrixIsNull");
            Logging.logger().fine(msg);
            throw new ArgumentException(msg);
        }

        return new PickPointFrustum(super.transformBy(matrix), this.screenRect);
    }

    /**
     * Returns the screenRect associated with this frustum
     *
     * @return screenRect associated with this frustum
     */
    public Rectangle getScreenRect()
    {
        return screenRect;
    }
}
}
