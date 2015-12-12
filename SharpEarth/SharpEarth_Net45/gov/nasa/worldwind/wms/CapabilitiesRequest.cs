/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SharpEarth.java.net;
using SharpEarth.util;

namespace SharpEarth.wms
{
/**
 * @author tag
 * @version $Id: CapabilitiesRequest.java 1171 2013-02-11 21:45:02Z dcollins $
 */
  public sealed class CapabilitiesRequest : Request
  {
    /** Construct an OGC GetCapabilities request using the default service. */
    public CapabilitiesRequest()
    {
    }

    /**
     * Constructs a request for the default service, WMS, and a specified server.
     *
     * @param uri the address of the web service.
     *
     * @throws ArgumentException if the uri is null.
     * @throws URISyntaxException       if the web service address is not a valid URI.
     */
    public CapabilitiesRequest( URI uri ) : base( uri, null )
    {
      if ( uri == null )
      {
        var message = Logging.getMessage( "nullValue.URIIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
    }

    /**
     * Constructs a request for a specified service at a specified server.
     *
     * @param uri     the address of the web service.
     * @param service the service name. Common names are WMS, WFS, WCS, etc.
     *
     * @throws ArgumentException if the uri or service name is null.
     * @throws URISyntaxException       if the web service address is not a valid URI.
     */
    public CapabilitiesRequest( URI uri, string service ) : base( uri, service )
    {
      if ( uri == null )
      {
        var message = Logging.getMessage( "nullValue.URIIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      if ( service == null )
      {
        var message = Logging.getMessage( "nullValue.WMSServiceNameIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }
    }

    protected void initialize( string service )
    {
      base.initialize( service );
      setParam( "REQUEST", "GetCapabilities" );
      setParam( "VERSION", "1.3.0" );
    }
  }
}
