/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using System;
using System.Text;

namespace SharpEarth.geom{


/**
 * @author Chris Maxwell
 * @version $Id: Quaternion.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class Quaternion
{
    // Multiplicative identity quaternion.
    public static readonly Quaternion IDENTITY = new Quaternion(0, 0, 0, 1);

    public readonly double _x;
    public readonly double _y;
    public readonly double _z;
    public readonly double _w;

    // 4 values in a quaternion.
    private static readonly int NUM_ELEMENTS = 4;
    // Cached computations.
    private int hashCode;

    public Quaternion(double x, double y, double z, double w)
    {
        this._x = x;
        this._y = y;
        this._z = z;
        this._w = w;
    }

    public override bool Equals(Object obj)
    {
        if (this == obj)
            return true;
        if (obj == null || obj.GetType() != this.GetType())
            return false;

        Quaternion that = (Quaternion) obj;
        return (this.x() == that.x())
            && (this.y() == that.y())
            && (this.z() == that.z())
            && (this.w() == that.w());
    }

    public override int GetHashCode()
    {
        if (this.hashCode == 0)
        {
            int result;
            ulong tmp;
            tmp = (ulong)BitConverter.DoubleToInt64Bits(this.x());
            result = (int) (tmp ^ (tmp >> 32));
            tmp = (ulong)BitConverter.DoubleToInt64Bits(this.y());
            result = 31 * result + (int) (tmp ^ (tmp >> 32));
            tmp = (ulong)BitConverter.DoubleToInt64Bits(this.z() );
            result = 31 * result + (int) (tmp ^ (tmp >> 32));
            tmp = (ulong)BitConverter.DoubleToInt64Bits(this.w() );
            result = 31 * result + (int) (tmp ^ (tmp >> 32));
            this.hashCode = result;
        }
        return this.hashCode;
    }

    public static Quaternion fromArray(double[] compArray, int offset)
    {
        if (compArray == null)
        {
            String msg = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if ((compArray.Length - offset) < NUM_ELEMENTS)
        {
            String msg = Logging.getMessage("generic.ArrayInvalidLength", compArray.Length);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        //noinspection PointlessArithmeticExpression                
        return new Quaternion(
            compArray[0 + offset],
            compArray[1 + offset],
            compArray[2 + offset],
            compArray[3 + offset]);
    }

    public double[] toArray(double[] compArray, int offset)
    {
        if (compArray == null)
        {
            String msg = Logging.getMessage("nullValue.ArrayIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if ((compArray.Length - offset) < NUM_ELEMENTS)
        {
            String msg = Logging.getMessage("generic.ArrayInvalidLength", compArray.Length);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        
        //noinspection PointlessArithmeticExpression
        compArray[0 + offset] = this.x();
        compArray[1 + offset] = this.y();
        compArray[2 + offset] = this.z();
        compArray[3 + offset] = this.w();
        return compArray;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(");
        sb.Append(this.x()).Append(", ");
        sb.Append(this.y()).Append(", ");
        sb.Append(this.z()).Append(", ");
        sb.Append(this.w() );
        sb.Append(")");
        return sb.ToString();
    }

    public double getX()
    {
        return this._x;
    }

    public double getY()
    {
        return this._y;
    }

    public double getZ()
    {
        return this._z;
    }

    public double getW()
    {
        return this._w;
    }

    public double x()
    {
        return this._x;
    }

    public double y()
    {
        return this._y;
    }

    public double z()
    {
        return this._z;
    }

    public double w()
    {
        return this._w;
    }

    // ============== Factory Functions ======================= //
    // ============== Factory Functions ======================= //
    // ============== Factory Functions ======================= //

    public static Quaternion fromAxisAngle(Angle angle, Vec4 axis)
    {
        if (angle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if (axis == null)
        {
            String msg = Logging.getMessage("nullValue.Vec4IsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return fromAxisAngle(angle, axis.x(), axis.y(), axis.z(), true);
    }

    public static Quaternion fromAxisAngle(Angle angle, double axisX, double axisY, double axisZ)
    {
        if (angle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        return fromAxisAngle(angle, axisX, axisY, axisZ, true);
    }

    private static Quaternion fromAxisAngle(Angle angle, double axisX, double axisY, double axisZ, bool normalize)
    {
        if (angle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (normalize)
        {
            double length = Math.Sqrt((axisX * axisX) + (axisY * axisY) + (axisZ * axisZ));
            if (!isZero(length) && (length != 1.0))
            {
                axisX /= length;
                axisY /= length;
                axisZ /= length;
            }
        }

        double s = angle.sinHalfAngle();
        double c = angle.cosHalfAngle();
        return new Quaternion(axisX * s, axisY * s, axisZ * s, c);
    }

    public static Quaternion fromMatrix(Matrix matrix)
    {
        if (matrix == null)
        {
            String msg = Logging.getMessage("nullValue.MatrixIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        
        double t = 1.0 + matrix.m11() + matrix.m22() + matrix.m33();
        double x, y, z, w;
        double s;
        const double EPSILON = 0.00000001;
        if (t > EPSILON)
        {
			s = 2.0 * Math.Sqrt(t);
			x = (matrix.m32() - matrix.m23()) / s;
            y = (matrix.m13() - matrix.m31()) / s;
            z = (matrix.m21() - matrix.m12()) / s;
            w = s / 4.0;
        }
        else if ((matrix.m11() > matrix.m22()) && (matrix.m11() > matrix.m33()) )
        {
			s = 2.0 * Math.Sqrt(1.0 + matrix.m11() - matrix.m22() - matrix.m33() );
			x = s / 4.0;
			y = (matrix.m21() + matrix.m12()) / s;
			z = (matrix.m13() + matrix.m31()) / s;
			w = (matrix.m32() - matrix.m23()) / s;
		}
        else if (matrix.m22() > matrix.m33() )
        {
			s = 2.0 * Math.Sqrt(1.0 + matrix.m22() - matrix.m11() - matrix.m33() );
			x = (matrix.m21() + matrix.m12()) / s;
			y = s / 4.0;
      z = (matrix.m32() + matrix.m23()) / s;
			w = (matrix.m13() - matrix.m31()) / s;
		}
        else
        {
			s = 2.0 * Math.Sqrt(1.0 + matrix.m33() - matrix.m11() - matrix.m22() );
			x = (matrix.m13() + matrix.m31()) / s;
			y = (matrix.m32() + matrix.m23()) / s;
			z = s / 4.0;
			w = (matrix.m21() - matrix.m12()) / s;
		}
        return new Quaternion(x, y, z, w);
    }

    /**
     * Returns a Quaternion created from three Euler angle rotations. The angles represent rotation about their
     * respective unit-axes. The angles are applied in the order X, Y, Z.
     * Angles can be extracted by calling {@link #getRotationX}, {@link #getRotationY}, {@link #getRotationZ}.
     *
     * @param x Angle rotation about unit-X axis.
     * @param y Angle rotation about unit-Y axis.
     * @param z Angle rotation about unit-Z axis.
     * @return Quaternion representation of the combined X-Y-Z rotation.
     */
    public static Quaternion fromRotationXYZ(Angle x, Angle y, Angle z)
    {
        if (x == null || y == null || z == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        double cx = x.cosHalfAngle();
        double cy = y.cosHalfAngle();
        double cz = z.cosHalfAngle();
        double sx = x.sinHalfAngle();
        double sy = y.sinHalfAngle();
        double sz = z.sinHalfAngle();

        // The order in which the three Euler angles are applied is critical. This can be thought of as multiplying
        // three quaternions together, one for each Euler angle (and corresponding unit axis). Like matrices,
        // quaternions affect vectors in reverse order. For example, suppose we construct a quaternion
        //     Q = (QX * QX) * QZ
        // then transform some vector V by Q. This can be thought of as first transforming V by QZ, then QY, and
        // finally by QX. This means that the order of quaternion multiplication is the reverse of the order in which
        // the Euler angles are applied.
        //
        // The ordering below refers to the order in which angles are applied.
        //
        // QX = (sx, 0,  0,  cx)
        // QY = (0,  sy, 0,  cy)
        // QZ = (0,  0,  sz, cz)
        //
        // 1. XYZ Ordering
        // (QZ * QY * QX)
        // qw = (cx * cy * cz) + (sx * sy * sz);
        // qx = (sx * cy * cz) - (cx * sy * sz);
        // qy = (cx * sy * cz) + (sx * cy * sz);
        // qz = (cx * cy * sz) - (sx * sy * cz);
        //
        // 2. ZYX Ordering
        // (QX * QY * QZ)
        // qw = (cx * cy * cz) - (sx * sy * sz);
        // qx = (sx * cy * cz) + (cx * sy * sz);
        // qy = (cx * sy * cz) - (sx * cy * sz);
        // qz = (cx * cy * sz) + (sx * sy * cz);
        //

        double qw = (cx * cy * cz) + (sx * sy * sz);
        double qx = (sx * cy * cz) - (cx * sy * sz);
        double qy = (cx * sy * cz) + (sx * cy * sz);
        double qz = (cx * cy * sz) - (sx * sy * cz);

        return new Quaternion(qx, qy, qz, qw);
    }

    /**
     * Returns a Quaternion created from latitude and longitude rotations.
     * Latitude and longitude can be extracted from a Quaternion by calling
     * {@link #getLatLon}.
     *
     * @param latitude Angle rotation of latitude.
     * @param longitude Angle rotation of longitude.
     * @return Quaternion representing combined latitude and longitude rotation.
     */
    public static Quaternion fromLatLon(Angle latitude, Angle longitude)
    {
        if (latitude == null || longitude == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        double clat = latitude.cosHalfAngle();
        double clon = longitude.cosHalfAngle();
        double slat = latitude.sinHalfAngle();
        double slon = longitude.sinHalfAngle();
        
        // The order in which the lat/lon angles are applied is critical. This can be thought of as multiplying two
        // quaternions together, one for each lat/lon angle. Like matrices, quaternions affect vectors in reverse
        // order. For example, suppose we construct a quaternion
        //     Q = QLat * QLon
        // then transform some vector V by Q. This can be thought of as first transforming V by QLat, then QLon. This
        // means that the order of quaternion multiplication is the reverse of the order in which the lat/lon angles
        // are applied.
        //
        // The ordering below refers to order in which angles are applied.
        //
        // QLat = (0,    slat, 0, clat)
        // QLon = (slon, 0,    0, clon)
        //
        // 1. LatLon Ordering
        // (QLon * QLat)
        // qw = clat * clon;
        // qx = clat * slon;
        // qy = slat * clon;
        // qz = slat * slon;
        //
        // 2. LonLat Ordering
        // (QLat * QLon)
        // qw = clat * clon;
        // qx = clat * slon;
        // qy = slat * clon;
        // qz = - slat * slon;
        //

        double qw = clat * clon;
        double qx = clat * slon;
        double qy = slat * clon;
        double qz = 0.0 - slat * slon;

        return new Quaternion(qx, qy, qz, qw);
    }

    // ============== Arithmetic Functions ======================= //
    // ============== Arithmetic Functions ======================= //
    // ============== Arithmetic Functions ======================= //

    public  Quaternion add(Quaternion quaternion)
    {
        if (quaternion == null)
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return new Quaternion(
            this.x()+ quaternion.x(),
            this.y()+ quaternion.y(),
            this.z()+ quaternion.z(),
            this.w() + quaternion.w() );
    }

    public  Quaternion subtract(Quaternion quaternion)
    {
        if (quaternion == null)
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return new Quaternion(
            this.x() - quaternion.x(),
            this.y() - quaternion.y(),
            this.z() - quaternion.z(),
            this.w() - quaternion.w() );
    }

    public  Quaternion multiplyComponents(double value)
    {
        return new Quaternion(
            this.x() * value,
            this.y() * value,
            this.z() * value,
            this.w() * value);
    }

    public  Quaternion multiply(Quaternion quaternion)
    {
        if (quaternion == null)
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return new Quaternion(
            (this.w() * quaternion.x()) + (this.x() * quaternion.w()) + (this.y() * quaternion.z()) - (this.z() * quaternion.y()),
            (this.w() * quaternion.y()) + (this.y() * quaternion.w()) + (this.z() * quaternion.x()) - (this.x() * quaternion.z()),
            (this.w() * quaternion.z()) + (this.z() * quaternion.w()) + (this.x() * quaternion.y()) - (this.y() * quaternion.x()),
            (this.w() * quaternion.w()) - (this.x() * quaternion.x()) - (this.y() * quaternion.y()) - (this.z() * quaternion.z()) );
    }

    public  Quaternion divideComponents(double value)
    {
        if (isZero(value))
        {
            String msg = Logging.getMessage("generic.ArgumentOutOfRange", value);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return new Quaternion(
            this.x() / value,
            this.y() / value,
            this.z() / value,
            this.w() / value);
    }

    public  Quaternion divideComponents(Quaternion quaternion)
    {
        if (quaternion == null)
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return new Quaternion(
            this.x() / quaternion.x(),
            this.y() / quaternion.y(),
            this.z() / quaternion.z(),
            this.w() / quaternion.w() );
    }

    public  Quaternion getConjugate()
    {
        return new Quaternion(
            0.0 - this.x(),
            0.0 - this.y(),
            0.0 - this.z(),
            this.w() );
    }

    public  Quaternion getNegative()
    {
        return new Quaternion(
            0.0 - this.x(),
            0.0 - this.y(),
            0.0 - this.z(),
            0.0 - this.w() );   
    }

    // ============== Geometric Functions ======================= //
    // ============== Geometric Functions ======================= //
    // ============== Geometric Functions ======================= //

    public  double getLength()
    {
        return Math.Sqrt(this.getLengthSquared());
    }

    public  double getLengthSquared()
    {
        return (this.x() * this.x())
             + (this.y() * this.y())
             + (this.z() * this.z())
             + (this.w() * this.w());
    }

    public  Quaternion normalize()
    {
        double length = this.getLength();
        // Vector has zero length.
        if (isZero(length))
        {
            return this;
        }
        else
        {
            return new Quaternion(
                this.x() / length,
                this.y() / length,
                this.z() / length,
                this.w() / length);
        }
    }

    public  double dot(Quaternion quaternion)
    {
        if (quaternion == null)
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return (this.x() * quaternion.x()) + (this.y() * quaternion.y()) + (this.z() * quaternion.z()) + (this.w() * quaternion.w());
    }

    public  Quaternion getInverse()
    {
        double length = this.getLength();
        // Vector has zero length.
        if (isZero(length))
        {
            return this;
        }
        else
        {
            return new Quaternion(
                (0.0 - this.x()) / length,
                (0.0 - this.y()) / length,
                (0.0 - this.z()) / length,
                this.w() / length);
        }
    }

    // ============== Mixing Functions ======================= //
    // ============== Mixing Functions ======================= //
    // ============== Mixing Functions ======================= //

    public static Quaternion mix(double amount, Quaternion value1, Quaternion value2)
    {
        if ((value1 == null) || (value2 == null))
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (amount < 0.0)
            return value1;
        else if (amount > 1.0)
            return value2;

        double t1 = 1.0 - amount;
        return new Quaternion(
            (value1.x() * t1) + (value2.x() * amount),
            (value1.y() * t1) + (value2.y() * amount),
            (value1.z() * t1) + (value2.z() * amount),
            (value1.w() * t1) + (value2.w() * amount));
    }

    public static Quaternion slerp(double amount, Quaternion value1, Quaternion value2)
    {
        if ((value1 == null) || (value2 == null))
        {
            String msg = Logging.getMessage("nullValue.QuaternionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (amount < 0.0)
            return value1;
        else if (amount > 1.0)
            return value2;

        double dot = value1.dot(value2);
        double x2, y2, z2, w2;
        if (dot < 0.0)
        {
            dot = 0.0 - dot;
            x2 = 0.0 - value2.x();
            y2 = 0.0 - value2.y();
            z2 = 0.0 - value2.z();
            w2 = 0.0 - value2.w();
        }
        else
        {
            x2 = value2.x();
            y2 = value2.y();
            z2 = value2.z();
            w2 = value2.w();
        }

        double t1, t2;

         double EPSILON = 0.0001;
        if ((1.0 - dot) > EPSILON) // standard case (slerp)
        {
            double angle = Math.Acos(dot);
            double sinAngle = Math.Sin(angle);
            t1 = Math.Sin((1.0 - amount) * angle) / sinAngle;
            t2 = Math.Sin(amount * angle) / sinAngle;
        }
        else // just lerp
        {
            t1 = 1.0 - amount;
            t2 = amount;
        }

        return new Quaternion(
            (value1.x() * t1) + (x2 * t2),
            (value1.y() * t1) + (y2 * t2),
            (value1.z() * t1) + (z2 * t2),
            (value1.w() * t1) + (w2 * t2));
    }

    // ============== Accessor Functions ======================= //
    // ============== Accessor Functions ======================= //
    // ============== Accessor Functions ======================= //

    public  Angle getAngle()
    {
        double w = this.w();

        double length = this.getLength();
        if (!isZero(length) && (length != 1.0))
            w /= length;

        double radians = 2.0 * Math.Acos(w);
        if (Double.IsNaN(radians))
            return null;

        return Angle.fromRadians(radians);
    }

    public  Vec4 getAxis()
    {
        double x = this.x();
        double y = this.y();
        double z = this.z();

        double length = this.getLength();
        if (!isZero(length) && (length != 1.0))
        {
            x /= length;
            y /= length;
            z /= length;
        }

        double vecLength = Math.Sqrt((x * x) + (y * y) + (z * z));
        if (!isZero(vecLength) && (vecLength != 1.0))
        {
            x /= vecLength;
            y /= vecLength;
            z /= vecLength;
        }

        return new Vec4(x, y, z);
    }

    public  Angle getRotationX()
    {
        double radians = Math.Atan2((2.0 * this.x() * this.w()) - (2.0 * this.y() * this.z()),
                                    1.0 - 2.0 * (this.x() * this.x()) - 2.0 * (this.z() * this.z()));
        if (Double.IsNaN(radians))
            return null;

        return Angle.fromRadians(radians);
    }

    public  Angle getRotationY()
    {
        double radians = Math.Atan2((2.0 * this.y() * this.w()) - (2.0 * this.x() * this.z()),
                                    1.0 - (2.0 * this.y() * this.y()) - (2.0 * this.z() * this.z()));
        if (Double.IsNaN(radians))
            return null;

        return Angle.fromRadians(radians);
    }

    public  Angle getRotationZ()
    {
        double radians = Math.Asin((2.0 * this.x() * this.y()) + (2.0 * this.z() * this.w()));
        if (Double.IsNaN(radians))
            return null;

        return Angle.fromRadians(radians);
    }

    public  LatLon getLatLon()
    {
        double latRadians = Math.Asin((2.0 * this.y() * this.w()) - (2.0 * this.x() * this.z()));
        double lonRadians = Math.Atan2((2.0 * this.y() * this.z()) + (2.0 * this.x() * this.w()),
                                       (this.w() * this.w()) - (this.x() * this.x()) - (this.y() * this.y()) + (this.z() * this.z()));
        if (Double.IsNaN(latRadians) || Double.IsNaN(lonRadians))
            return null;

        return LatLon.fromRadians(latRadians, lonRadians);
    }

    // ============== Helper Functions ======================= //
    // ============== Helper Functions ======================= //
    // ============== Helper Functions ======================= //

    private static  Double PositiveZero = +0.0d;

    private static  Double NegativeZero = -0.0d;

    private static bool isZero(double value)
    {
        return (PositiveZero.CompareTo(value) == 0)
            || (NegativeZero.CompareTo(value) == 0);
    }
}
}
