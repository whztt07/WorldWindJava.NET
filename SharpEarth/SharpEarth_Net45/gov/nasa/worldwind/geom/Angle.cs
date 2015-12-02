/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using SharpEarth.util;
using System;
using System.Text.RegularExpressions;

namespace SharpEarth.geom{



/**
 * Represents a geometric angle. Instances of <code>Angle</code> are immutable. An angle can be obtained through the
 * factory methods {@link #fromDegrees} and {@link #fromRadians}.
 *
 * @author Tom Gaskins
 * @version $Id: Angle.java 2419 2014-11-08 04:44:55Z tgaskins $
 */
public class Angle : IComparable<Angle>
{
    // Angle format
    public readonly static String ANGLE_FORMAT_DD = "gov.nasa.worldwind.Geom.AngleDD";
    public readonly static String ANGLE_FORMAT_DM = "gov.nasa.worldwind.Geom.AngleDM";
    public readonly static String ANGLE_FORMAT_DMS = "gov.nasa.worldwind.Geom.AngleDMS";

    /** Represents an angle of zero degrees */
    public readonly static Angle ZERO = fromDegrees(0);

    /** Represents a right angle of positive 90 degrees */
    public readonly static Angle POS90 = fromDegrees(90);

    /** Represents a right angle of negative 90 degrees */
    public readonly static Angle NEG90 = fromDegrees(-90);

    /** Represents an angle of positive 180 degrees */
    public readonly static Angle POS180 = fromDegrees(180);

    /** Represents an angle of negative 180 degrees */
    public readonly static Angle NEG180 = fromDegrees(-180);

    /** Represents an angle of positive 360 degrees */
    public readonly static Angle POS360 = fromDegrees(360);

    /** Represents an angle of negative 360 degrees */
    public readonly static Angle NEG360 = fromDegrees(-360);

    /** Represents an angle of 1 minute */
    public readonly static Angle MINUTE = fromDegrees(1d / 60d);

    /** Represents an angle of 1 second */
    public readonly static Angle SECOND = fromDegrees(1d / 3600d);

    private readonly static double DEGREES_TO_RADIANS = Math.PI / 180d;
    private readonly static double RADIANS_TO_DEGREES = 180d / Math.PI;

    /**
     * Obtains an angle from a specified number of degrees.
     *
     * @param degrees the size in degrees of the angle to be obtained
     *
     * @return a new angle, whose size in degrees is given by <code>degrees</code>
     */
    public static Angle fromDegrees(double degrees)
    {
        return new Angle(degrees, DEGREES_TO_RADIANS * degrees);
    }

    /**
     * Obtains an angle from a specified number of radians.
     *
     * @param radians the size in radians of the angle to be obtained.
     *
     * @return a new angle, whose size in radians is given by <code>radians</code>.
     */
    public static Angle fromRadians(double radians)
    {
        return new Angle(RADIANS_TO_DEGREES * radians, radians);
    }

    private static readonly double PIOver2 = Math.PI / 2;

    public static Angle fromDegreesLatitude(double degrees)
    {
        degrees = degrees < -90 ? -90 : degrees > 90 ? 90 : degrees;
        double radians = DEGREES_TO_RADIANS * degrees;
        radians = radians < -PIOver2 ? -PIOver2 : radians > PIOver2 ? PIOver2 : radians;

        return new Angle(degrees, radians);
    }

    public static Angle fromRadiansLatitude(double radians)
    {
        radians = radians < -PIOver2 ? -PIOver2 : radians > PIOver2 ? PIOver2 : radians;
        double degrees = RADIANS_TO_DEGREES * radians;
        degrees = degrees < -90 ? -90 : degrees > 90 ? 90 : degrees;

        return new Angle(degrees, radians);
    }

    public static Angle fromDegreesLongitude(double degrees)
    {
        degrees = degrees < -180 ? -180 : degrees > 180 ? 180 : degrees;
        double radians = DEGREES_TO_RADIANS * degrees;
        radians = radians < -Math.PI ? -Math.PI : radians > Math.PI ? Math.PI : radians;

        return new Angle(degrees, radians);
    }

    public static Angle fromRadiansLongitude(double radians)
    {
        radians = radians < -Math.PI ? -Math.PI : radians > Math.PI ? Math.PI : radians;
        double degrees = RADIANS_TO_DEGREES * radians;
        degrees = degrees < -180 ? -180 : degrees > 180 ? 180 : degrees;

        return new Angle(degrees, radians);
    }

    /**
     * Obtains an angle from rectangular coordinates.
     *
     * @param x the abscissa coordinate.
     * @param y the ordinate coordinate.
     *
     * @return a new angle, whose size is determined from <code>x</code> and <code>y</code>.
     */
    public static Angle fromXY(double x, double y)
    {
        double radians = Math.Atan2(y, x);
        return new Angle(RADIANS_TO_DEGREES * radians, radians);
    }

    /**
     * Obtain an angle from a given number of degrees, minutes and seconds.
     *
     * @param degrees integer number of degrees, positive.
     * @param minutes integer number of minutes, positive only between 0 and 60.
     * @param seconds integer number of seconds, positive only between 0 and 60.
     *
     * @return a new angle whose size in degrees is given by <code>degrees</code>, <code>minutes</code> and
     *         <code>seconds</code>.
     *
     * @throws ArgumentException if minutes or seconds are outside the 0-60 range.
     */
    public static Angle fromDMS(int degrees, int minutes, int seconds)
    {
        if (minutes < 0 || minutes >= 60)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", minutes);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (seconds < 0 || seconds >= 60)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", seconds);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(Math.Sign(degrees) * (Math.Abs(degrees) + minutes / 60d + seconds / 3600d));
    }

    public static Angle fromDMdS(int degrees, double minutes)
    {
        if (minutes < 0 || minutes >= 60)
        {
            String message = Logging.getMessage("generic.ArgumentOutOfRange", minutes);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(Math.Sign(degrees) * (Math.Abs(degrees) + minutes / 60d));
    }

    /**
     * Obtain an angle from a degrees, minute and seconds character string.
     * <p>eg:<pre>
     * 123 34 42
     * -123* 34' 42" (where * stands for the degree symbol)
     * +45* 12' 30" (where * stands for the degree symbol)
     * 45 12 30 S
     * 45 12 30 N
     * </p>
     *
     * @param dmsString the degrees, minute and second character string.
     *
     * @return the corresponding angle.
     *
     * @throws ArgumentException if dmsString is null or not properly formated.
     */
    public static Angle fromDMS(String dmsString)
    {
        if (dmsString == null)
        {
            String message = Logging.getMessage("nullValue.StringIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        // Check for string format validity
        string regex = "([-|\\+]?\\d{1,3}[d|D|\u00B0|\\s](\\s*\\d{1,2}['|\u2019|\\s])?"
            + "(\\s*\\d{1,2}[\"|\u201d|\\s])?\\s*([N|n|S|s|E|e|W|w])?\\s?)";
      Regex matcher = new Regex( regex );
      if ( !matcher.IsMatch( dmsString + " " ) )
      {
        string message = Logging.getMessage( "generic.ArgumentOutOfRange", dmsString );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
      // Replace degree, min and sec signs with space
      dmsString = dmsString.Replace( "[D|d|\u00B0|'|\u2019|\"|\u201d]", " " );
      // Replace multiple spaces with single ones
      dmsString = dmsString.Replace( "\\s+", " " );
      dmsString = dmsString.Trim();

      // Check for sign prefix and suffix
      int sign = 1;
      char suffix = dmsString.ToUpper()[dmsString.Length - 1];
      if ( !Char.IsDigit( suffix ) )
      {
        sign = (suffix == 'S' || suffix == 'W') ? -1 : 1;
        dmsString = dmsString.Substring( 0, dmsString.Length - 1 );
        dmsString = dmsString.Trim();
      }
      char prefix = dmsString[0];
      if ( !Char.IsDigit( prefix ) )
      {
        sign *= (prefix == '-') ? -1 : 1;
        dmsString = dmsString.Substring( 1, dmsString.Length );
      }

      // Extract degrees, minutes and seconds
      String[] DMS = dmsString.Split( ' ' );
      int d = Int32.Parse( DMS[0] );
      int m = DMS.Length > 1 ? Int32.Parse( DMS[1] ) : 0;
      int s = DMS.Length > 2 ? Int32.Parse( DMS[2] ) : 0;

      return fromDMS( d, m, s ).multiply( sign );
    }

    public readonly double degrees;
    public readonly double radians;

    public Angle(Angle angle)
    {
        this.degrees = angle.degrees;
        this.radians = angle.radians;
    }

    private Angle(double degrees, double radians)
    {
        this.degrees = degrees;
        this.radians = radians;
    }

    /**
     * Retrieves the size of this angle in degrees. This method may be faster than first obtaining the radians and then
     * converting to degrees.
     *
     * @return the size of this angle in degrees.
     */
    public double getDegrees()
    {
        return this.degrees;
    }

    /**
     * Retrieves the size of this angle in radians. This may be useful for <code>java.lang.Math</code> functions, which
     * generally take radians as trigonometric arguments. This method may be faster that first obtaining the degrees and
     * then converting to radians.
     *
     * @return the size of this angle in radians.
     */
    public double getRadians()
    {
        return this.radians;
    }

    /**
     * Obtains the sum of these two angles. Does not accept a null argument. This method is commutative, so
     * <code>a.add(b)</code> and <code>b.add(a)</code> are equivalent. Neither this angle nor angle is changed, instead
     * the result is returned as a new angle.
     *
     * @param angle the angle to add to this one.
     *
     * @return an angle whose size is the total of this angles and angles size.
     *
     * @throws ArgumentException if angle is null.
     */
    public Angle add(Angle angle)
    {
        if (angle == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(this.degrees + angle.degrees);
    }

    /**
     * Obtains the difference of these two angles. Does not accept a null argument. This method is not commutative.
     * Neither this angle nor angle is changed, instead the result is returned as a new angle.
     *
     * @param angle the angle to subtract from this angle.
     *
     * @return a new angle corresponding to this angle's size minus angle's size.
     *
     * @throws ArgumentException if angle is null.
     */
    public Angle subtract(Angle angle)
    {
        if (angle == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(this.degrees - angle.degrees);
    }

    /**
     * Multiplies this angle by <code>multiplier</code>. This angle remains unchanged. The result is returned as a new
     * angle.
     *
     * @param multiplier a scalar by which this angle is multiplied.
     *
     * @return a new angle whose size equals this angle's size multiplied by <code>multiplier</code>.
     */
    public Angle multiply(double multiplier)
    {
        return fromDegrees(this.degrees * multiplier);
    }

    /**
     * Divides this angle by another angle. This angle remains unchanged, instead the resulting value in degrees is
     * returned.
     *
     * @param angle the angle by which to divide.
     *
     * @return this angle's degrees divided by angle's degrees.
     *
     * @throws ArgumentException if angle is null.
     */
    public double divide(Angle angle)
    {
        if (angle == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        if (angle.getDegrees() == 0.0)
        {
            String message = Logging.getMessage("generic.DivideByZero");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return this.degrees / angle.degrees;
    }

    public  Angle addDegrees(double degrees)
    {
        return fromDegrees(this.degrees + degrees);
    }

    public  Angle subtractDegrees(double degrees)
    {
        return fromDegrees(this.degrees - degrees);
    }

    /**
     * Divides this angle by <code>divisor</code>. This angle remains unchanged. The result is returned as a new angle.
     * Behaviour is undefined if <code>divisor</code> equals zero.
     *
     * @param divisor the number to be divided by.
     *
     * @return a new angle equivalent to this angle divided by <code>divisor</code>.
     */
    public Angle divide(double divisor)
    {
        return fromDegrees(this.degrees / divisor);
    }

    public Angle addRadians(double radians)
    {
        return fromRadians(this.radians + radians);
    }

    public Angle subtractRadians(double radians)
    {
        return fromRadians(this.radians - radians);
    }

    /**
     * Computes the shortest distance between this and angle, as an angle.
     *
     * @param angle the angle to measure angular distance to.
     *
     * @return the angular distance between this and <code>value</code>.
     */
    public Angle angularDistanceTo(Angle angle)
    {
        if (angle == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        double differenceDegrees = angle.subtract(this).degrees;
        if (differenceDegrees < -180)
            differenceDegrees += 360;
        else if (differenceDegrees > 180)
            differenceDegrees -= 360;

        double absAngle = Math.Abs(differenceDegrees);
        return fromDegrees(absAngle);
    }

    /**
     * Obtains the sine of this angle.
     *
     * @return the trigonometric sine of this angle.
     */
    public double sin()
    {
        return Math.Sin(this.radians);
    }

    public double sinHalfAngle()
    {
        return Math.Sin(0.5 * this.radians);
    }

    public static Angle asin(double sine)
    {
        return fromRadians(Math.Asin(sine));
    }

    public static double arctanh(double radians)
    {
        return 0.5 * Math.Log((1 + radians) / (1 - radians));
    }

    /**
     * Obtains the cosine of this angle.
     *
     * @return the trigonometric cosine of this angle.
     */
    public double cos()
    {
        return Math.Cos(this.radians);
    }

    public double cosHalfAngle()
    {
        return Math.Cos(0.5 * this.radians);
    }

    public static Angle acos(double cosine)
    {   //Tom: this method is not used, should we delete it? (13th Dec 06)
        return fromRadians(Math.Acos(cosine));
    }

    /**
     * Obtains the tangent of half of this angle.
     *
     * @return the trigonometric tangent of half of this angle.
     */
    public double tanHalfAngle()
    {
        return Math.Tan(0.5 * this.radians);
    }

    public static Angle atan(double tan)
    {   //Tom: this method is not used, should we delete it? (13th Dec 06)
        return fromRadians(Math.Atan(tan));
    }

    /**
     * Obtains the average of two angles. This method is commutative, so <code>midAngle(m, n)</code> and
     * <code>midAngle(n, m)</code> are equivalent.
     *
     * @param a1 the first angle.
     * @param a2 the second angle.
     *
     * @return the average of <code>a1</code> and <code>a2</code> throws ArgumentException if either angle is
     *         null.
     */
    public static Angle midAngle(Angle a1, Angle a2)
    {
        if (a1 == null || a2 == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(0.5 * (a1.degrees + a2.degrees));
    }

    /**
     * Obtains the average of three angles. The order of parameters does not matter.
     *
     * @param a the first angle.
     * @param b the second angle.
     *
     * @return the average of <code>a1</code>, <code>a2</code> and <code>a3</code>
     *
     * @throws ArgumentException if <code>a</code> or <code>b</code> is null
     */
    public static Angle average(Angle a, Angle b)
    {
        if (a == null || b == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees(0.5 * (a.degrees + b.degrees));
    }

    /**
     * Obtains the average of three angles. The order of parameters does not matter.
     *
     * @param a the first angle.
     * @param b the second angle.
     * @param c the third angle.
     *
     * @return the average of <code>a1</code>, <code>a2</code> and <code>a3</code>.
     *
     * @throws ArgumentException if <code>a</code>, <code>b</code> or <code>c</code> is null.
     */
    public static Angle average(Angle a, Angle b, Angle c)
    {
        if (a == null || b == null || c == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return fromDegrees((a.degrees + b.degrees + c.degrees) / 3);
    }

    /**
     * Limits a specified angle to be within a specified minimum and maximum.
     * <p/>
     * The returned angle is undefined if min > max. Otherwise, this method's return value is equivalent to the
     * following:
     * <p/>
     * <ul> <li>min - If value < min</li> <li>max - If value > max</li> <li>value - If min <= value <= max</li> </ul>
     *
     * @param value The angle to clamp.
     * @param min   The minimum angle to clamp to.
     * @param max   The maximum angle to clamp to.
     *
     * @return The clamped angle.
     *
     * @throws ArgumentException if any argument is null.
     */
    public static Angle clamp(Angle value, Angle min, Angle max)
    {
        if (value == null || min == null || max == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        return value.degrees < min.degrees ? min : (value.degrees > max.degrees ? max : value);
    }

    /**
     * Linearly interpolates between two angles.
     *
     * @param amount the interpolant.
     * @param value1 the first angle.
     * @param value2 the second angle.
     *
     * @return a new angle between <code>value1</code> and <code>value2</code>.
     */
    public static Angle mix(double amount, Angle value1, Angle value2)
    {
        if (value1 == null || value2 == null)
        {
            String message = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        if (amount < 0)
            return value1;
        else if (amount > 1)
            return value2;

        Quaternion quat = Quaternion.slerp(
            amount,
            Quaternion.fromAxisAngle(value1, Vec4.UNIT_X),
            Quaternion.fromAxisAngle(value2, Vec4.UNIT_X));

        Angle angle = quat.getRotationX();
        if (Double.IsNaN(angle.degrees))
            return null;

        return angle;
    }

    /**
     * Compares this {@link Angle} with another. Returns a negative integer if this is the smaller angle, a positive
     * integer if this is the larger, and zero if both angles are equal.
     *
     * @param angle the angle to compare against.
     *
     * @return -1 if this angle is smaller, 0 if both are equal and +1 if this angle is larger.
     *
     * @throws ArgumentException if angle is null.
     */
    public int compareTo(Angle angle)
    {
        if (angle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        if (this.degrees < angle.degrees)
            return -1;

        if (this.degrees > angle.degrees)
            return 1;

        return 0;
    }

    public static double normalizedDegrees(double degrees)
    {
        double a = degrees % 360;
        return a > 180 ? a - 360 : a < -180 ? 360 + a : a;
    }

    public static double normalizedDegreesLatitude(double degrees)
    {
        double lat = degrees % 180;
        return lat > 90 ? 180 - lat : lat < -90 ? -180 - lat : lat;
    }

    public static double normalizedDegreesLongitude(double degrees)
    {
        double lon = degrees % 360;
        return lon > 180 ? lon - 360 : lon < -180 ? 360 + lon : lon;
    }

    public static Angle normalizedAngle(Angle unnormalizedAngle)
    {
        if (unnormalizedAngle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return fromDegrees(normalizedDegrees(unnormalizedAngle.degrees));
    }

    public static Angle normalizedLatitude(Angle unnormalizedAngle)
    {
        if (unnormalizedAngle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return fromDegrees(normalizedDegreesLatitude(unnormalizedAngle.degrees));
    }

    public static Angle normalizedLongitude(Angle unnormalizedAngle)
    {
        if (unnormalizedAngle == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return fromDegrees(normalizedDegreesLongitude(unnormalizedAngle.degrees));
    }

    public Angle normalize()
    {
        return normalizedAngle(this);
    }

    public Angle normalizedLatitude()
    {
        return normalizedLatitude(this);
    }

    public Angle normalizedLongitude()
    {
        return normalizedLongitude(this);
    }

    public static bool crossesLongitudeBoundary(Angle angleA, Angle angleB)
    {
        if (angleA == null || angleB == null)
        {
            String msg = Logging.getMessage("nullValue.AngleIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        // A segment cross the line if end pos have different longitude signs
        // and are more than 180 degrees longitude apart
        return (Math.Sign(angleA.degrees) != Math.Sign(angleB.degrees))
            && (Math.Abs(angleA.degrees - angleB.degrees) > 180);
    }

    public static bool isValidLatitude(double value)
    {
        return value >= -90 && value <= 90;
    }

    public static bool isValidLongitude(double value)
    {
        return value >= -180 && value <= 180;
    }

    public static Angle max(Angle a, Angle b)
    {
        return a.degrees >= b.degrees ? a : b;
    }

    public static Angle min(Angle a, Angle b)
    {
        return a.degrees <= b.degrees ? a : b;
    }

    /**
     * Obtains a <code>String</code> representation of this angle.
     *
     * @return the value of this angle in degrees and as a <code>String</code>.
     */
    public override string ToString()
    {
        return degrees.ToString() + '\u00B0';
    }

    /**
     * Forms a decimal degrees {@link String} representation of this {@link Angle}.
     *
     * @param digits the number of digits past the decimal point to include in the string.
     *
     * @return the value of this angle in decimal degrees as a string with the specified number of digits beyond the
     *         decimal point. The string is padded with trailing zeros to fill the number of post-decimal point
     *         positions requested.
     */
    public String toDecimalDegreesString(int digits)
    {
        if ((digits < 0) || (digits > 15))
        {
            String msg = Logging.getMessage("generic.ArgumentOutOfRange", digits);
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return String.Format("%." + digits + "f\u00B0", this.degrees);
    }

    /**
     * Obtains a {@link String} representation of this {@link Angle} formatted as degrees, minutes and seconds integer
     * values.
     *
     * @return the value of this angle in degrees, minutes, seconds as a string.
     */
    public String toDMSString()
    {
        double temp = this.degrees;
        int sign = (int) Math.Sign(temp);
        temp *= sign;
        int d = (int) Math.Floor(temp);
        temp = (temp - d) * 60d;
        int m = (int) Math.Floor(temp);
        temp = (temp - m) * 60d;
        int s = (int) Math.Round(temp);

        if (s == 60)
        {
            m++;
            s = 0;
        } // Fix rounding errors
        if (m == 60)
        {
            d++;
            m = 0;
        }

        return (sign == -1 ? "-" : "") + d + '\u00B0' + ' ' + m + '\u2019' + ' ' + s + '\u201d';
    }

    /**
     * Obtains a {@link String} representation of this {@link Angle} formatted as degrees and decimal minutes.
     *
     * @return the value of this angle in degrees and decimal minutes as a string.
     */
    public String toDMString()
    {
        double temp = this.degrees;
        int sign = (int) Math.Sign(temp);
        temp *= sign;
        int d = (int) Math.Floor(temp);
        temp = (temp - d) * 60d;
        int m = (int) Math.Floor(temp);
        temp = (temp - m) * 60d;
        int s = (int) Math.Round(temp);

        if (s == 60)
        {
            m++;
            s = 0;
        } // Fix rounding errors
        if (m == 60)
        {
            d++;
            m = 0;
        }

        double mf = s == 0 ? m : m + s / 60.0;

        return (sign == -1 ? "-" : "") + d + '\u00B0' + ' ' + String.Format("%5.2f", mf) + '\u2019';
    }

    public String toFormattedDMSString()
    {
        double temp = this.degrees;
        int sign = (int) Math.Sign(temp);

        temp *= sign;
        int d = (int) Math.Floor(temp);
        temp = (temp - d) * 60d;
        int m = (int) Math.Floor(temp);
        temp = (temp - m) * 60d;
        double s = Math.Round(temp * 100) / 100;  // keep two decimals for seconds

        if (s == 60)
        {
            m++;
            s = 0;
        } // Fix rounding errors
        if (m == 60)
        {
            d++;
            m = 0;
        }

        return String.Format("%4d\u00B0 %2d\u2019 %5.2f\u201d", sign * d, m, s);
    }

    public double[] toDMS()
    {
        double temp = this.degrees;
        int sign = (int) Math.Sign(temp);

        temp *= sign;
        int d = (int) Math.Floor(temp);
        temp = (temp - d) * 60d;
        int m = (int) Math.Floor(temp);
        temp = (temp - m) * 60d;
        double s = Math.Round(temp * 100) / 100;  // keep two decimals for seconds

        if (s == 60)
        {
            m++;
            s = 0;
        } // Fix rounding errors
        if (m == 60)
        {
            d++;
            m = 0;
        }

        return new double[] {sign * d, m, s};
    }

    /**
     * Obtains the amount of memory this {@link Angle} consumes.
     *
     * @return the memory footprint of this angle in bytes.
     */
    public long getSizeInBytes()
    {
        return sizeof(double) / 8;
    }

    public override bool Equals(Object o)
    {
        if (this == o)
            return true;
        if (o == null || GetType() != o.GetType())
            return false;

        Angle angle = (Angle) o;

        //noinspection RedundantIfStatement
        if (angle.degrees != this.degrees)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        ulong temp = (ulong)( degrees != +0.0d ? BitConverter.DoubleToInt64Bits(degrees) : 0L);
        return (int) (temp ^ (temp >> 32));
    }

    public int CompareTo( Angle other )
    {
      return this.degrees.CompareTo( other.getDegrees() );
    }
  }
}
