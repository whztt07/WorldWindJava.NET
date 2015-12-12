/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.io;
using SharpEarth.util;
namespace SharpEarth.ogc.collada.io{



/**
 * Represents a COLLADA document read from a file.
 *
 * @author pabercrombie
 * @version $Id: ColladaFile.java 660 2012-06-26 16:13:11Z pabercrombie $
 */
public class ColladaFile : ColladaDoc
{
    /** File from which COLLADA content is read. */
    protected File colladaFile;

    /**
     * Create a new instance from a file.
     *
     * @param file COLLADA file from which to read content.
     */
    public ColladaFile(File file)
    {
        if (file == null)
        {
            String message = Logging.getMessage("nullValue.FileIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.colladaFile = file;
    }

    /** {@inheritDoc} */
    public InputStream getInputStream() throws IOException
    {
        return new FileInputStream(this.colladaFile);
    }

    /** {@inheritDoc} */
    public String getSupportFilePath(String path)
    {
        if (path == null)
        {
            String message = Logging.getMessage("nullValue.FilePathIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        File pathFile = new File(path);
        if (pathFile.isAbsolute())
            return null;

        pathFile = new File(this.colladaFile.getParentFile(), path);

        return pathFile.exists() ? pathFile.getPath() : null;
    }
}
}
