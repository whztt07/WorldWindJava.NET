/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.formats.nitfs{
/**
 * @author Lado Garakanidze
 * @version $Id: NITFSSymbolSegment.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class NITFSSymbolSegment : NITFSSegment
{
    public NITFSSymbolSegment(java.nio.ByteBuffer buffer, int headerStartOffset, int headerLength, int dataStartOffset, int dataLength)
    {
        super(NITFSSegmentType.SYMBOL_SEGMENT, buffer, headerStartOffset, headerLength, dataStartOffset, dataLength);

        this.restoreBufferPosition();
    }
}
}
