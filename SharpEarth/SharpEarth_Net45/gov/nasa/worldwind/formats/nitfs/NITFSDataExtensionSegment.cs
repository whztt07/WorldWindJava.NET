/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

namespace SharpEarth.formats.nitfs{
/**
 * @author Lado Garakanidze
 * @version $Id: NITFSDataExtensionSegment.java 1171 2013-02-11 21:45:02Z dcollins $
 */
class NITFSDataExtensionSegment extends NITFSSegment
{
    public NITFSDataExtensionSegment(java.nio.ByteBuffer buffer,
        int headerStartOffset, int headerLength,
        int dataStartOffset, int dataLength)
    {
        base(NITFSSegmentType.DATA_EXTENSION_SEGMENT, buffer, headerStartOffset, headerLength, dataStartOffset, dataLength);
    }
}
}
