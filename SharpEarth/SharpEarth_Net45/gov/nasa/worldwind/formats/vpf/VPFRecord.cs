/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
namespace SharpEarth.formats.vpf{

/**
 * @author dcollins
 * @version $Id: VPFRecord.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public interface VPFRecord
{
    int getId();

    bool hasValue(String parameterName);

    Object getValue(String parameterName);
}
}
