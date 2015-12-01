/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.net.URI;
using java.io;
using SharpEarth.util.Logging;
namespace SharpEarth.ogc.collada.io{



/**
 * Represents a COLLADA document read from an input stream.
 *
 * @author pabercrombie
 * @version $Id: ColladaInputStream.java 660 2012-06-26 16:13:11Z pabercrombie $
 */
public class ColladaInputStream implements ColladaDoc
{
    /** The {@link java.io.InputStream} specified to the constructor. */
    protected InputStream inputStream;

    /** The URI of this COLLADA document. May be {@code null}. */
    protected URI uri;

    /**
     * Construct a <code>ColladaInputStream</code> instance.
     *
     * @param sourceStream the COLLADA stream.
     * @param uri          the URI of this COLLADA document. This URI is used to resolve relative references. May be
     *                     {@code null}.
     *
     * @throws ArgumentException if the specified input stream is null.
     * @throws IOException              if an error occurs while attempting to read from the stream.
     */
    public ColladaInputStream(InputStream sourceStream, URI uri) throws IOException
    {
        if (sourceStream == null)
        {
            String message = Logging.getMessage("nullValue.InputStreamIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        this.inputStream = sourceStream;
        this.uri = uri;
    }

    /**
     * Returns the input stream reference passed to the constructor.
     *
     * @return the input stream reference passed to the constructor.
     */
    public InputStream getInputStream() throws IOException
    {
        return this.inputStream;
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

        if (this.uri != null)
        {
            URI remoteFile = uri.resolve(path);
            if (remoteFile != null)
                return remoteFile.toString();
        }
        return null;
    }
}
}
