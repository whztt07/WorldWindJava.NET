/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using java.net;
using SharpEarth.java.net;
using SharpEarth.util;

namespace SharpEarth.wms
{
/**
 * This class provides a means to construct an OGC web service request, such as WMS GetMap or WFS GetCapabilities.
 *
 * @author tag
 * @version $Id: Request.java 1171 2013-02-11 21:45:02Z dcollins $
 */

  public abstract class Request
  {
    // Use a TreeMap to hold the query parameters so that they'll always be attached to the
    // URL query string in the same order. This allows a simple string comparison to
    // determine whether two url strings address the same document.
    private readonly SortedDictionary<string, string> queryParams = new SortedDictionary<string, string>();
    private URI uri;
    /** Constructs a request for the default service, WMS. */

    protected Request()
    {
      initialize( null );
    }

    /**
     * Constructs a request for the default service, WMS, and a specified server.
     *
     * @param uri the address of the web service. May be null when this constructor invoked by subclasses.
     *
     * @throws URISyntaxException if the web service address is not a valid URI.
     */

    protected Request( URI uri ) : this( uri, null )
    {
    }

    /**
     * Constructs a request for a specified service at a specified server.
     *
     * @param uri     the address of the web service. May be null.
     * @param service the service name. Common names are WMS, WFS, WCS, etc. May by null when this constructor is
     *                invoked by subclasses.
     *
     * @throws URISyntaxException if the web service address is not a valid URI.
     */

    protected Request( URI uri, string service )
    {
      if ( uri != null )
      {
        try
        {
          setUri( uri );
        }
        catch ( URISyntaxException e )
        {
          Logging.logger().fine( Logging.getMessage( "generic.URIInvalid", uri.ToString() ) );
          throw e;
        }
      }

      initialize( service );
    }

    /**
     * Copy constructor. Performs a shallow copy.
     *
     * @param sourceRequest the request to copy.
     *
     * @throws ArgumentException if copy source is null.
     * @throws URISyntaxException       if the web service address is not a valid URI.
     */

    protected Request( Request sourceRequest )
    {
      if ( sourceRequest == null )
      {
        var message = Logging.getMessage( "nullValue.CopyConstructorSourceIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      sourceRequest.copyParamsTo( this );
      setUri( sourceRequest.getUri() );
    }

    protected void initialize( string service )
    {
      queryParams.Add( "SERVICE", service ?? "WMS" );
      queryParams.Add( "EXCEPTIONS", "application/vnd.ogc.se_xml" );
    }

    private void copyParamsTo( Request destinationRequest )
    {
      if ( destinationRequest == null )
      {
        var message = Logging.getMessage( "nullValue.CopyTargetIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      foreach ( var entry in queryParams )
        destinationRequest.queryParams.Add( entry.Key, entry.Value );
    }

    protected void setUri( URI uri )
    {
      if ( uri == null )
      {
        var message = Logging.getMessage( "nullValue.URIIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      try
      {
        this.uri = new URI( uri.getScheme(), uri.getUserInfo(), uri.getHost(), uri.getPort(), uri.getPath(),
          buildQueryString( uri.getQuery() ), null );
      }
      catch ( URISyntaxException e )
      {
        var message = Logging.getMessage( "generic.URIInvalid", uri.ToString() );
        Logging.logger().fine( message );
        throw e;
      }
    }

    public string getRequestName()
    {
      return getParam( "REQUEST" );
    }

    public string getVersion()
    {
      return getParam( "VERSION" );
    }

    public void setVersion( string version )
    {
      if ( version == null )
      {
        var message = Logging.getMessage( "nullValue.WMSVersionIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      setParam( "VERSION", version );
    }

    public string getService()
    {
      return getParam( "SERVICE" );
    }

    public void setService( string service )
    {
      if ( service == null )
      {
        var message = Logging.getMessage( "nullValue.WMSServiceNameIsNull" );
        Logging.logger().severe( message );
        throw new ArgumentException( message );
      }

      setParam( "SERVICE", service );
    }

    public void setParam( string key, string value )
    {
      if ( key != null )
        queryParams.Add( key, value );
    }

    public string getParam( string key )
    {
      string value;
      return queryParams.TryGetValue( key, out value ) ? value : null;
    }

    public URI getUri()
    {
      if ( uri == null )
        return null;

      try
      {
        return new URI( uri.getScheme(), uri.getUserInfo(), uri.getHost(), uri.getPort(),
          uri.getPath(), buildQueryString( uri.getQuery() ), null );
      }
      catch ( URISyntaxException e )
      {
        var message = Logging.getMessage( "generic.URIInvalid", uri.ToString() );
        Logging.logger().fine( message );
        throw e;
      }
    }

    private string buildQueryString( string existingQueryString )
    {
      if ( existingQueryString.Length > 1 && !existingQueryString.EndsWith( "&" ) )
        existingQueryString += "&";
      var queryString = new StringBuilder( existingQueryString );


      foreach ( var entry in queryParams )
      {
        queryString.Append( $"{entry.Key}={entry.Value}&" );
      }


      var finalString = queryString.ToString();

      // Remove a trailing ampersand
      if ( string.IsNullOrWhiteSpace( existingQueryString ) )
      {
        finalString = finalString.TrimEnd( '&' );
      }

      return finalString;
    }

    public override string ToString()
    {
      var errorMessage = "Error converting wms-request URI to string.";
      try
      {
        var fullUri = getUri();
        return fullUri != null ? fullUri.ToString() : errorMessage;
      }
      catch ( URISyntaxException e )
      {
        return errorMessage;
      }
    }
  }
}