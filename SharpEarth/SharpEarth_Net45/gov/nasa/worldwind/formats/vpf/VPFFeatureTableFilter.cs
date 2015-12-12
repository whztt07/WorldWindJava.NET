/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.io.FileFilter;
using SharpEarth.util;
namespace SharpEarth.formats.vpf{



/**
 * @author dcollins
 * @version $Id: VPFFeatureTableFilter.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class VPFFeatureTableFilter : FileFilter
{
    /** Creates a VPFFeatureTableFilter, but otherwise does nothing. */
    public VPFFeatureTableFilter()
    {
    }

    /**
     * Returns true if the specified file is a Feature Table.
     *
     * @param file the file in question.
     *
     * @return true if the file should be accepted; false otherwise.
     *
     * @throws ArgumentException if the file is null.
     */
    public bool accept(java.io.File file)
    {
        if (file == null)
        {
            String msg = Logging.getMessage("nullValue.FileIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        return VPFUtils.getFeatureTypeName(file.getName()) != null;
    }
}
}
