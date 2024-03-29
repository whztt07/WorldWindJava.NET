/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using java.util.logging.Level;
using java.nio.ByteBuffer;
using java.net;
using SharpEarth.util;
namespace SharpEarth.retrieve{



/**
 * Retrieves resources identified by a jar url, which has the form jar:<url>!/{entry}, as in:
 * jar:http://www.foo.com/bar/baz.jar!/COM/foo/Quux.class. See {@link java.net.JarURLConnection} for a full description
 * of jar URLs.
 *
 * @author tag
 * @version $Id: JarRetriever.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public class JarRetriever : URLRetriever
{
    private int responseCode;
    private String responseMessage;

    public JarRetriever(URL url, RetrievalPostProcessor postProcessor)
    {
        super(url, postProcessor);
    }

    public int getResponseCode()
    {
        return this.responseCode;
    }

    public String getResponseMessage()
    {
        return this.responseMessage;
    }

    protected ByteBuffer doRead(URLConnection connection) throws Exception
    {
        if (connection == null)
        {
            String msg = Logging.getMessage("nullValue.ConnectionIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

        JarURLConnection htpc = (JarURLConnection) connection;
        this.responseCode = htpc.getContentLength() >= 0 ? HttpURLConnection.HTTP_OK : -1;
        this.responseMessage = this.responseCode >= 0 ? "OK" : "FAILED";

        String contentType = connection.getContentType();
        Logging.logger().log(Level.FINE, "HTTPRetriever.ResponseInfo", new Object[] {this.responseCode,
            connection.getContentLength(), contentType != null ? contentType : "content type not returned",
            connection.getURL()});

        if (this.responseCode == HttpURLConnection.HTTP_OK) // intentionally re-using HTTP constant
            return super.doRead(connection);

        return null;
    }
}
}
