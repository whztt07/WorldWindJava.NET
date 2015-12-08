/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.formats.tiff{

/**
 * @author Lado Garakanidze
 * @version $Id: GeoTiff.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public abstract class GeoTiff
{
    public static readonly int Undefined   = 0;
    public static readonly int UserDefined = 32767;
    
    // Geotiff extension tags
    public static class Tag
    {
        public static readonly int MODEL_PIXELSCALE        = 33550;
        public static readonly int MODEL_TIEPOINT          = 33922;
        public static readonly int MODEL_TRANSFORMATION    = 34264;
        public static readonly int GEO_KEY_DIRECTORY       = 34735;
        public static readonly int GEO_DOUBLE_PARAMS       = 34736;
        public static readonly int GEO_ASCII_PARAMS        = 34737;

        public static readonly int GDAL_NODATA             = 42113;
    }


    public static class GeoKeyHeader
    {
        public static readonly int KeyDirectoryVersion = 1;
        public static readonly int KeyRevision = 1;
        public static readonly int MinorRevision = 0;
        
    }

    public static class GeoKey
    {
        public static readonly int ModelType           = 1024; // see GeoTiff.ModelType values
        public static readonly int RasterType          = 1025; // see GeoTiff.RasterType values

        public static readonly int GeographicType      = 2048; // see GeoTiff.GCS for values or Section 6.3.2.1 Codes
        // GeoKey Requirements for User-Defined geographic CS:
        public static readonly int GeogCitation            = 2049; // ASCII
        public static readonly int GeogGeodeticDatum       = 2050; // SHORT, See section 6.3.2.2 Geodetic Datum Codes
        public static readonly int GeogPrimeMeridian       = 2051; // SHORT, Section 6.3.2.4 Codes
        public static readonly int GeogLinearUnits         = 2052; // Double, See GeoTiff.Unit.Liner or Section 6.3.1.3 Codes
        public static readonly int GeogLinearUnitSize      = 2053; // Double, meters
        public static readonly int GeogAngularUnits        = 2054; // Short, See GeoTiff.Units.Angular or Section 6.3.1.4 Codes
        public static readonly int GeogAngularUnitSize     = 2055; // Double, radians
        public static readonly int GeogEllipsoid           = 2056; // Short, See Section 6.3.2.3 Codes 
        public static readonly int GeogAzimuthUnits        = 2060; // Short, Section 6.3.1.4 Codes
        public static readonly int GeogPrimeMeridianLong   = 2061; // DOUBLE, See GeoTiff.Units.Angular

        // 6.2.3 Projected CS Parameter Keys
        public static readonly int ProjectedCSType             = 3072; /*  Section 6.3.3.1 codes */
        public static readonly int PCSCitation                 = 3073; /*  documentation */
        public static readonly int Projection                  = 3074; /*  Section 6.3.3.2 codes */
        public static readonly int ProjCoordTrans              = 3075; /*  Section 6.3.3.3 codes */
        public static readonly int ProjLinearUnits             = 3076; /*  Section 6.3.1.3 codes */
        public static readonly int ProjLinearUnitSize          = 3077; /*  meters */
        public static readonly int ProjStdParallel1            = 3078; /*  GeogAngularUnit */
        public static readonly int ProjStdParallel2            = 3079; /*  GeogAngularUnit */
        public static readonly int ProjNatOriginLong           = 3080; /*  GeogAngularUnit */
        public static readonly int ProjNatOriginLat            = 3081; /*  GeogAngularUnit */
        public static readonly int ProjFalseEasting            = 3082; /*  ProjLinearUnits */
        public static readonly int ProjFalseNorthing           = 3083; /*  ProjLinearUnits */
        public static readonly int ProjFalseOriginLong         = 3084; /*  GeogAngularUnit */
        public static readonly int ProjFalseOriginLat          = 3085; /*  GeogAngularUnit */
        public static readonly int ProjFalseOriginEasting      = 3086; /*  ProjLinearUnits */
        public static readonly int ProjFalseOriginNorthing     = 3087; /*  ProjLinearUnits */
        public static readonly int ProjCenterLong              = 3088; /*  GeogAngularUnit */
        public static readonly int ProjCenterLat               = 3089; /*  GeogAngularUnit */
        public static readonly int ProjCenterEasting           = 3090; /*  ProjLinearUnits */
        public static readonly int ProjCenterNorthing          = 3091; /*  ProjLinearUnits */
        public static readonly int ProjScaleAtNatOrigin        = 3092; /*  ratio */
        public static readonly int ProjScaleAtCenter           = 3093; /*  ratio */
        public static readonly int ProjAzimuthAngle            = 3094; /*  GeogAzimuthUnit */
        public static readonly int ProjStraightVertPoleLong    = 3095; /*  GeogAngularUnit */
        // Aliases:
        public static readonly int ProjStdParallel = ProjStdParallel1;
        public static readonly int ProjOriginLong = ProjNatOriginLong;
        public static readonly int ProjOriginLat = ProjNatOriginLat;
        public static readonly int ProjScaleAtOrigin = ProjScaleAtNatOrigin;

        // 6.2.4 Vertical CS Keys
        public static readonly int VerticalCSType    = 4096; /* Section 6.3.4.1 codes */
        public static readonly int VerticalCitation  = 4097; /* ASCII */
        public static readonly int VerticalDatum     = 4098; /* Section 6.3.4.2 codes */
        public static readonly int VerticalUnits     = 4099; /* Section 6.3.1.3 codes */
    }

    public static class ModelType
    {
        public static readonly int Undefined   =     0;
        public static readonly int Projected   =     1;
        public static readonly int Geographic  =     2;
        public static readonly int Geocentric  =     3;
        public static readonly int UserDefined = 32767;

        public static readonly int DEFAULT = Geographic;
    }

    public static class RasterType
    {
        public static readonly int Undefined           =     0; // highly not recomended to use
        public static readonly int RasterPixelIsArea   =     1;
        public static readonly int RasterPixelIsPoint  =     2;
        public static readonly int UserDefined         = 32767; // highly not recomended to use
    }


    public static class Unit
    {
        public static readonly int Undefined   =     0;
        public static readonly int UserDefined = 32767;

        //6.3.1.3 Linear Units Codes
        public static class Linear
        {
            public static readonly int Meter                       = 9001;
            public static readonly int Foot                        = 9002;
            public static readonly int Foot_US_Survey              = 9003;
            public static readonly int Foot_Modified_American      = 9004;
            public static readonly int Foot_Clarke                 = 9005;
            public static readonly int Foot_Indian                 = 9006;
            public static readonly int Link                        = 9007;
            public static readonly int Link_Benoit                 = 9008;
            public static readonly int Link_Sears                  = 9009;
            public static readonly int Chain_Benoit                = 9010;
            public static readonly int Chain_Sears                 = 9011;
            public static readonly int Yard_Sears                  = 9012;
            public static readonly int Yard_Indian                 = 9013;
            public static readonly int Fathom                      = 9014;
            public static readonly int Mile_International_Nautical = 9015;
        }

        // 6.3.1.4 Angular Units Codes
        // These codes shall be used for any key that requires specification of an angular unit of measurement.
        public static class Angular
        {
            public static readonly int Angular_Radian              = 9101;
            public static readonly int Angular_Degree              = 9102;
            public static readonly int Angular_Arc_Minute          = 9103;
            public static readonly int Angular_Arc_Second          = 9104;
            public static readonly int Angular_Grad                = 9105;
            public static readonly int Angular_Gon                 = 9106;
            public static readonly int Angular_DMS                 = 9107;
            public static readonly int Angular_DMS_Hemisphere      = 9108;
        }
    }

    // Geogrphic Coordinate System (GCS)
    public static class GCS
    {
        public static readonly int Undefined   = 0;
        public static readonly int UserDefined = 32767;

        public static readonly int NAD_83      = 4269;
        public static readonly int WGS_72      = 4322;
        public static readonly int WGS_72BE    = 4324;
        public static readonly int WGS_84      = 4326;

        public static readonly int DEFAULT = WGS_84;
    }

    // Geogrphic Coordinate System Ellipsoid (GCSE)
    public static class GCSE
    {
        public static readonly int WGS_84 = 4030;
    }

    // Projected Coordinate System (PCS)
    public static class PCS
    {
        public static readonly int Undefined   =     0;
        public static readonly int UserDefined = 32767;
    }

    public class ProjectedCS
    {
        public static ProjectedCS Undefined = new ProjectedCS( 0, 0, "Undefined");

        private string name;
        private int    epsg;
        private int    datum;

        private ProjectedCS(int epsg, int datum, string name )
        {
            this.name = name;
            this.epsg = epsg;
            this.datum = datum;
        }

        public int getEPSG()
        {
            return this.epsg;
        }

        public int getDatum()
        {
            return this.datum;
        }

        public string getName()
        {
            return this.name;
        }
    }



    // Vertical Coordinate System (VCS) 
    public static class VCS
    {
        // [ 1, 4999] = Reserved
        // [ 5000, 5099] = EPSG Ellipsoid Vertical CS Codes
        // [ 5100, 5199] = EPSG Orthometric Vertical CS Codes
        // [ 5200, 5999] = Reserved EPSG
        // [ 6000, 32766] = Reserved
        // [32768, 65535] = Private User Implementations
        
        public static readonly int Undefined   =     0;
        public static readonly int UserDefined = 32767;

        public static readonly int Airy_1830_ellipsoid = 5001;
        public static readonly int Airy_Modified_1849_ellipsoid = 5002;
        public static readonly int ANS_ellipsoid = 5003;
        public static readonly int Bessel_1841_ellipsoid = 5004;
        public static readonly int Bessel_Modified_ellipsoid = 5005;
        public static readonly int Bessel_Namibia_ellipsoid = 5006;
        public static readonly int Clarke_1858_ellipsoid = 5007;
        public static readonly int Clarke_1866_ellipsoid = 5008;
        public static readonly int Clarke_1880_Benoit_ellipsoid = 5010;
        public static readonly int Clarke_1880_IGN_ellipsoid = 5011;
        public static readonly int Clarke_1880_RGS_ellipsoid = 5012;
        public static readonly int Clarke_1880_Arc_ellipsoid = 5013;
        public static readonly int Clarke_1880_SGA_1922_ellipsoid = 5014;
        public static readonly int Everest_1830_1937_Adjustment_ellipsoid = 5015;
        public static readonly int Everest_1830_1967_Definition_ellipsoid = 5016;
        public static readonly int Everest_1830_1975_Definition_ellipsoid = 5017;
        public static readonly int Everest_1830_Modified_ellipsoid = 5018;
        public static readonly int GRS_1980_ellipsoid = 5019;
        public static readonly int Helmert_1906_ellipsoid = 5020;
        public static readonly int INS_ellipsoid = 5021;
        public static readonly int International_1924_ellipsoid = 5022;
        public static readonly int International_1967_ellipsoid = 5023;
        public static readonly int Krassowsky_1940_ellipsoid = 5024;
        public static readonly int NWL_9D_ellipsoid = 5025;
        public static readonly int NWL_10D_ellipsoid = 5026;
        public static readonly int Plessis_1817_ellipsoid = 5027;
        public static readonly int Struve_1860_ellipsoid = 5028;
        public static readonly int War_Office_ellipsoid = 5029;
        public static readonly int WGS_84_ellipsoid = 5030;
        public static readonly int GEM_10C_ellipsoid = 5031;
        public static readonly int OSU86F_ellipsoid = 5032;
        public static readonly int OSU91A_ellipsoid = 5033;
        // Orthometric Vertical CS;
        public static readonly int Newlyn = 5101;
        public static readonly int North_American_Vertical_Datum_1929 = 5102;
        public static readonly int North_American_Vertical_Datum_1988 = 5103;
        public static readonly int Yellow_Sea_1956 = 5104;
        public static readonly int Baltic_Sea = 5105;
        public static readonly int Caspian_Sea = 5106;
            
        public static readonly int DEFAULT = Undefined;
    }
}

}
