/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.nio.ByteBuffer;
namespace SharpEarth.formats.vpf{


/**
 * @author dcollins
 * @version $Id: VPFDataBuffer.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface VPFDataBuffer
{
    Object get(int index);

    Object getBackingData();

    bool hasValue(int index);

    void read(ByteBuffer byteBuffer);

    void read(ByteBuffer byteBuffer, int length);
}
}
