/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.formats.tiff{

/**
 * @author Lado Garakanidze
 * @version $Id: Tiff.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public static class Tiff
{
    public static readonly int Undefined = 0;

    public static class Type
    {
        public static readonly int BYTE = 1;
        public static readonly int ASCII = 2;
        public static readonly int SHORT = 3;
        public static readonly int LONG = 4;
        public static readonly int RATIONAL = 5;
        public static readonly int SBYTE = 6;
        public static readonly int UNDEFINED = 7;
        public static readonly int SSHORT = 8;
        public static readonly int SLONG = 9;
        public static readonly int SRATIONAL = 10;
        public static readonly int FLOAT = 11;
        public static readonly int DOUBLE = 12;
    }

    public static class Tag
    {
        // Baseline Tiff 6.0 tags...
        public static readonly int IMAGE_WIDTH = 256;
        public static readonly int IMAGE_LENGTH = 257;
        public static readonly int BITS_PER_SAMPLE = 258;
        public static readonly int COMPRESSION = 259;
        public static readonly int PHOTO_INTERPRETATION = 262;

        public static readonly int DOCUMENT_NAME = 269;
        public static readonly int IMAGE_DESCRIPTION = 270;
        public static readonly int DEVICE_MAKE = 271; // manufacturer of the scanner or video digitizer
        public static readonly int DEVICE_MODEL = 272; // model name/number of the scanner or video digitizer
        public static readonly int STRIP_OFFSETS = 273;
        public static readonly int ORIENTATION = 274;

        public static readonly int SAMPLES_PER_PIXEL = 277;
        public static readonly int ROWS_PER_STRIP = 278;
        public static readonly int STRIP_BYTE_COUNTS = 279;
        public static readonly int MIN_SAMPLE_VALUE = 280;
        public static readonly int MAX_SAMPLE_VALUE = 281;
        public static readonly int X_RESOLUTION = 282;
        public static readonly int Y_RESOLUTION = 283;
        public static readonly int PLANAR_CONFIGURATION = 284;
        public static readonly int RESOLUTION_UNIT = 296;

        public static readonly int SOFTWARE_VERSION = 305; // Name and release # of the software that created the image
        public static readonly int DATE_TIME = 306; // uses format "YYYY:MM:DD HH:MM:SS"
        public static readonly int ARTIST = 315;
        public static readonly int COPYRIGHT = 315; // same as ARTIST

        public static readonly int TIFF_PREDICTOR = 317;
        public static readonly int COLORMAP = 320;
        public static readonly int TILE_WIDTH = 322;
        public static readonly int TILE_LENGTH = 323;
        public static readonly int TILE_OFFSETS = 324;
        public static readonly int TILE_COUNTS = 325;

        // Tiff extensions...
        public static readonly int SAMPLE_FORMAT = 339;  // SHORT array of samplesPerPixel size
    }

    // The orientation of the image with respect to the rows and columns.
    public static class Orientation
    {
        // 1 = The 0th row represents the visual top of the image,
        // and the 0th column represents the visual left-hand side.
        public static readonly int Row0_IS_TOP__Col0_IS_LHS = 1;

        //2 = The 0th Row represents the visual top of the image,
        // and the 0th column represents the visual right-hand side.
        public static readonly int Row0_IS_TOP__Col0_IS_RHS = 2;

        //3 = The 0th row represents the visual bottom of the image,
        // and the 0th column represents the visual right-hand side.
        public static readonly int Row0_IS_BOTTOM__Col0_IS_RHS = 3;

        //4 = The 0th row represents the visual bottom of the image,
        // and the 0th column represents the visual left-hand side.
        public static readonly int Row0_IS_BOTTOM__Col0_IS_LHS = 4;

        //5 = The 0th row represents the visual left-hand side of the image,
        // and the 0th column represents the visual top.
        public static readonly int Row0_IS_LHS__Col0_IS_TOP = 5;

        //6 = The 0th row represents the visual right-hand side of the image,
        // and the 0th column represents the visual top.
        public static readonly int Row0_IS_RHS__Col0_IS_TOP = 6;

        //7 = The 0th row represents the visual right-hand side of the image,
        // and the 0th column represents the visual bottom.
        public static readonly int Row0_IS_RHS__Col0_IS_BOTTOM = 7;

        public static readonly int DEFAULT = Row0_IS_TOP__Col0_IS_LHS;
    }

    public static class BitsPerSample
    {
        public static readonly int MONOCHROME_BYTE = 8;
        public static readonly int MONOCHROME_UINT8 = 8;
        public static readonly int MONOCHROME_UINT16 = 16;
        public static readonly int ELEVATIONS_INT16 = 16;
        public static readonly int ELEVATIONS_FLOAT32 = 32;
        public static readonly int RGB = 24;
        public static readonly int YCbCr = 24;
        public static readonly int CMYK = 32;
    }

    public static class SamplesPerPixel
    {
        public static readonly int MONOCHROME = 1;
        public static readonly int RGB = 3;
        public static readonly int RGBA = 4;
        public static readonly int YCbCr = 3;
        public static readonly int CMYK = 4;
    }

    // The color space of the image data
    public static class Photometric
    {
        public static readonly int Undefined = -1;

        // 0 = WhiteIsZero
        // For bilevel and grayscale images: 0 is imaged as white.
        // 2**BitsPerSample-1 is imaged as black.
        // This is the normal value for Compression=2
        public static readonly int Grayscale_WhiteIsZero = 0;

        // 1 = BlackIsZero
        // For bilevel and grayscale images: 0 is imaged as black.
        // 2**BitsPerSample-1 is imaged as white.
        // If this value is specified for Compression=2, the image should display and print reversed.
        public static readonly int Grayscale_BlackIsZero = 1;

        // 2 = RGB
        // The RGB value of (0,0,0) represents black, (255,255,255) represents white,
        // assuming 8-bit components.
        // Note! For PlanarConfiguration=1, the components are stored in the indicated order:
        // first Red, then Green, then Blue.
        // For PlanarConfiguration = 2, the StripOffsets for the component planes are stored
        // in the indicated order: first the Red component plane StripOffsets,
        // then the Green plane StripOffsets, then the Blue plane StripOffsets.
        public static readonly int Color_RGB = 2;

        // 3 = Palette color
        // In this model, a color is described with a single component.
        // The value of the component is used as an index into the red, green and blue curves in
        // the ColorMap field to retrieve an RGB triplet that defines the color.
        //
        // Note!!
        // When PhotometricInterpretation=3 is used, ColorMap must be present and SamplesPerPixel must be 1.
        public static readonly int Color_Palette = 3;

        // 4 = Transparency Mask.
        // This means that the image is used to define an irregularly shaped region of another
        // image in the same TIFF file.
        //
        // SamplesPerPixel and BitsPerSample must be 1.
        //
        // PackBits compression is recommended.
        // The 1-bits define the interior of the region; the 0-bits define the exterior of the region.
        //
        // A reader application can use the mask to determine which parts of the image to
        // display. Main image pixels that correspond to 1-bits in the transparency mask are
        // imaged to the screen or printer, but main image pixels that correspond to 0-bits in
        // the mask are not displayed or printed.
        // The image mask is typically at a higher resolution than the main image, if the
        // main image is grayscale or color so that the edges can be sharp.
        public static readonly int Transparency_Mask = 4;

        public static readonly int CMYK = 5;

        public static readonly int YCbCr = 6;

        // There is no default for PhotometricInterpretation, and it is required.
    }

    public static class Compression
    {
        public static readonly int NONE = 1;
        public static readonly int LZW = 5;
        public static readonly int JPEG = 6;
        public static readonly int PACKBITS = 32773;
    }

    public static class PlanarConfiguration
    {
        // CHUNKY
        // The component values for each pixel are stored contiguously.
        // The order of the components within the pixel is specified by PhotometricInterpretation.
        // For example, for RGB data, the data is stored as RGBRGBRGB...
        public static readonly int CHUNKY = 1;

        // PLANAR
        // The components are stored in separate component planes.
        // The values in StripOffsets and StripByteCounts are then arranged as
        // a 2-dimensional array, with SamplesPerPixel rows and StripsPerImage columns.
        // (All of the columns for row 0 are stored first, followed by the columns of row 1, and so on.)
        //
        // PhotometricInterpretation describes the type of data stored in each component plane.
        // For example, RGB data is stored with the Red components in one component plane,
        // the Green in another, and the Blue in another.
        //
        // Note!
        // If SamplesPerPixel is 1, PlanarConfiguration is irrelevant, and need not be included.
        public static readonly int PLANAR = 2;

        public static readonly int DEFAULT = CHUNKY;
    }

    public static class ResolutionUnit
    {
        public static readonly int NONE = 1;
        public static readonly int INCH = 2;
        public static readonly int CENTIMETER = 3;
    }

    public static class SampleFormat
    {
        public static readonly int UNSIGNED = 1;
        public static readonly int SIGNED = 2;
        public static readonly int IEEEFLOAT = 3;
        public static readonly int UNDEFINED = 4;
    }
}
}
