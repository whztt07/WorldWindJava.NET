/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.formats.nitfs{

/**
 * @author Lado Garakanidze
 * @version $Id: NITFSImageBand.java 1171 2013-02-11 21:45:02Z dcollins $
 */
class NITFSImageBand
{
    private String representation;
    private String significanceForImageCategory;
    private String imageFilterCondition;
    private String stdImageFilterCode;
    private short numOfLookupTables;
    private short numOfLookupTableEntries;
    // public int[]  lookupTablesOffset; // one byte per entry per band
    private byte[][] lut;

    private boolean isGrayImage;
    private boolean hasTransparentEntry;

    public boolean isGrayImage()
    {
        return this.isGrayImage;
    }

    public boolean isHasTransparentEntry()
    {
        return this.hasTransparentEntry;
    }

    public String getRepresentation()
    {
        return this.representation;
    }

    public short getNumOfLookupTables()
    {
        return this.numOfLookupTables;
    }

    public short getNumOfLookupTableEntries()
    {
        return this.numOfLookupTableEntries;
    }

    public NITFSImageBand(java.nio.ByteBuffer buffer)
    {
        this.representation = NITFSUtil.getString(buffer, 2);
        this.significanceForImageCategory = NITFSUtil.getString(buffer, 6);
        this.imageFilterCondition = NITFSUtil.getString(buffer, 1);
        this.stdImageFilterCode = NITFSUtil.getString(buffer, 3);
        this.numOfLookupTables = NITFSUtil.getShortNumeric(buffer, 1);
        this.numOfLookupTableEntries = NITFSUtil.getShortNumeric(buffer, 5);
        if (0 < this.numOfLookupTables && 0 < this.numOfLookupTableEntries)
        {
            this.lut = new byte[this.numOfLookupTables][this.numOfLookupTableEntries];
            for (int j = 0; j < this.numOfLookupTables; j++)
            {
                buffer.get(this.lut[j], 0, this.numOfLookupTableEntries);
            }
        }

        this.isGrayImage = (1 == this.numOfLookupTables);
        this.hasTransparentEntry = (217 == this.numOfLookupTableEntries);
    }

    /**
     * Returns if the specified color code is reserved for overlays generated by application software.
     *
     * @param colorIndex the color code to test.
     *
     * @return true of the color code is a reserved color code, and false otherwise.
     */
    public final boolean isReservedApplicationCode(int colorIndex)
    {
        // The color code is an application-specific reserved code if exceeds the color lookup table size.
        return colorIndex >= this.numOfLookupTableEntries;
    }

    public final int lookupR5G6B5(int colorIndex)
    {
        int r, g, b;
        if (3 == this.numOfLookupTables)
        {
            r = (0x00FF & this.lut[0][colorIndex]) >> 3;
            g = (0x00FF & this.lut[1][colorIndex]) >> 2;
            b = (0x00FF & this.lut[2][colorIndex]) >> 3;
        }
        else
        {
            int gray = 0x00FF & this.lut[0][ colorIndex ];
            r = gray >> 3;
            g = gray >> 2;
            b = gray >> 3;
        }
        return 0x00FFFF & ((r << 11) | (g << 5) | b );
    }


    public final int lookupRGB(int colorIndex)
    {
        int r, g, b;
        if (3 == this.numOfLookupTables)
        {
            r = (0x00FF & this.lut[0][colorIndex]);
            g = (0x00FF & this.lut[1][colorIndex]);
            b = (0x00FF & this.lut[2][colorIndex]);
        }
        else
        {
            r = g = b = 0x00FF & this.lut[0][ colorIndex ];
        }
        return (int) (0x00FFFFFFL & (long)((r << 16) | (g << 8) | b ));
    }

    public final int lookupGray(int colorIndex)
    {

        if (3 == this.numOfLookupTables)
        {
            int r = (0x00FF & this.lut[0][colorIndex]);
            int g = (0x00FF & this.lut[1][colorIndex]);
            int b = (0x00FF & this.lut[2][colorIndex]);

            return (30 * r + 59 * g + 11 * b)/100;
        }
        else
        {
            return (0x00FF & this.lut[0][colorIndex]);
        }
    }
}
}
