/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */
using java.util.logging.Level;
using javax.xml.stream.XMLStreamException;
using org.w3c.dom;
using SharpEarth.util;
using SharpEarth.ogc.wms.WMSCapabilities;
using SharpEarth.ogc.wcs.wcs100.WCS100Capabilities;
using SharpEarth.ogc.OGCCapabilities;
using SharpEarth.exception;
using SharpEarth.avlist;
using System;
using java.org.w3c.dom;
using.QName.SharpEarth.ogc;

namespace SharpEarth{



/**
 * A basic implementation of the {@link Factory} interface.
 *
 * @author tag
 * @version $Id: BasicFactory.java 2072 2014-06-21 21:20:25Z tgaskins $
 */
public class BasicFactory : Factory
{
    /**
     * Static method to create an object from a factory and configuration source.
     *
     * @param factoryKey   the key identifying the factory in {@link Configuration}.
     * @param configSource the configuration source. May be any of the types listed for {@link
     *                     #createFromConfigSource(object, SharpEarth.avlist.AVList)}
     *
     * @return a new instance of the requested object.
     *
     * @throws ArgumentException if the factory key is null, or if the configuration source is null or an empty
     *                                  string.
     */
    public static object create(string factoryKey, object configSource)
    {
        if (factoryKey == null)
        {
            string message = Logging.getMessage("generic.FactoryKeyIsNull");
            throw new ArgumentException(message);
        }

        if (WWUtil.isEmpty(configSource))
        {
            string message = Logging.getMessage("generic.ConfigurationSourceIsInvalid", configSource);
            throw new ArgumentException(message);
        }

        Factory factory = (Factory) WorldWind.createConfigurationComponent(factoryKey);
        return factory.createFromConfigSource(configSource, null);
    }

    /**
     * Static method to create an object from a factory, a configuration source, and an optional configuration parameter
     * list.
     *
     * @param factoryKey   the key identifying the factory in {@link Configuration}.
     * @param configSource the configuration source. May be any of the types listed for {@link
     *                     #createFromConfigSource(object, SharpEarth.avlist.AVList)}
     * @param parameters       key-value parameters to override or supplement the information provided in the specified
     *                     configuration source. May be null.
     *
     * @return a new instance of the requested object.
     *
     * @throws ArgumentException if the factory key is null, or if the configuration source is null or an empty
     *                                  string.
     */
    public static object create(string factoryKey, object configSource, AVList parameters)
    {
        if (factoryKey == null)
        {
            string message = Logging.getMessage("generic.FactoryKeyIsNull");
            throw new ArgumentException(message);
        }

        if (WWUtil.isEmpty(configSource))
        {
            string message = Logging.getMessage("generic.ConfigurationSourceIsInvalid", configSource);
            throw new ArgumentException(message);
        }

        Factory factory = (Factory) WorldWind.createConfigurationComponent(factoryKey);
        return factory.createFromConfigSource(configSource, parameters);
    }

    /**
     * Creates an object from a general configuration source. The source can be one of the following: <ul> <li>{@link
     * java.net.URL}</li> <li>{@link java.io.File}</li> <li>{@link java.io.InputStream}</li> <li>{@link Element}</li>
     * <li>{@link SharpEarth.ogc.OGCCapabilities}</li>
     * <li>{@link SharpEarth.ogc.wcs.wcs100.WCS100Capabilities}</li>
     * <li>{@link string} holding a file name, a name of a resource on the classpath, or a string representation of a
     * URL</li></ul>
     * <p/>
     *
     * @param configSource the configuration source. See above for supported types.
     * @param parameters       key-value parameters to override or supplement the information provided in the specified
     *                     configuration source. May be null.
     *
     * @return the new object.
     *
     * @throws ArgumentException if the configuration source is null or an empty string.
     * @throws WWUnrecognizedException  if the source type is unrecognized.
     * @throws WWRuntimeException       if object creation fails. The exception indicating the source of the failure is
     *                                  included as the {@link Exception#initCause(Throwable)}.
     */
    public object createFromConfigSource(object configSource, AVList parameters)
    {
        if (WWUtil.isEmpty(configSource))
        {
            string message = Logging.getMessage("generic.ConfigurationSourceIsInvalid", configSource);
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        object o = null;

        try
        {
            if (configSource is Element)
            {
                o = this.doCreateFromElement((Element) configSource, parameters);
            }
            else if (configSource is OGCCapabilities)
                o = this.doCreateFromCapabilities((OGCCapabilities) configSource, parameters);
            else if (configSource is WCS100Capabilities)
                o = this.doCreateFromCapabilities((WCS100Capabilities) configSource, parameters);
            else
            {
                Document doc = WWXML.openDocument(configSource);
                if (doc != null)
                    o = this.doCreateFromElement(doc.getDocumentElement(), parameters);
            }
        }
        catch (Exception e)
        {
            string msg = Logging.getMessage("generic.CreationFromConfigurationFileFailed", configSource);
            throw new WWRuntimeException(msg, e);
        }

        return o;
    }

    /**
     * Create an object such as a layer or elevation model given a local OGC capabilities document containing named
     * layer descriptions.
     *
     * @param capsFileName the path to the capabilities file. The file must be either an absolute path or a relative
     *                     path available on the classpath. The file contents must be a valid OGC capabilities
     *                     document.
     * @param parameters       a list of configuration properties. These properties override any specified in the
     *                     capabilities document. The list should contain the {@link AVKey#LAYER_NAMES} property for
     *                     services that define layer, indicating which named layers described in the capabilities
     *                     document to create. If this argumet is null or contains no layers, the first named layer is
     *                     used.
     *
     * @return the requested object.
     *
     * @throws ArgumentException if the file name is null or empty.
     * @throws IllegalStateException    if the capabilites document contains no named layer definitions.
     * @throws WWRuntimeException       if an error occurs while opening, reading or parsing the capabilities document.
     *                                  The exception indicating the source of the failure is included as the {@link
     *                                  Exception#initCause(Throwable)}.
     */
    public object createFromCapabilities(string capsFileName, AVList parameters)
    {
        if (WWUtil.isEmpty(capsFileName))
        {
            string message = Logging.getMessage("nullValue.FilePathIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        WMSCapabilities caps = new WMSCapabilities(capsFileName);

        try
        {
            caps.parse();
        }
        catch (javax.xml.stream.XMLStreamException e)
        {
            string message = Logging.getMessage("generic.CannotParseCapabilities", capsFileName);
            Logging.logger().log(Level.SEVERE, message, e);
            throw new WWRuntimeException(message, e);
        }

        return this.doCreateFromCapabilities(caps, parameters);
    }

    /**
     * Implemented by subclasses to perform the actual object creation. This default implementation always returns
     * null.
     *
     * @param caps   the capabilities document.
     * @param parameters a list of configuration properties. These properties override any specified in the capabilities
     *               document. The list should contain the {@link AVKey#LAYER_NAMES} property for services that define
     *               layers, indicating which named layers described in the capabilities document to create. If this
     *               argumet is null or contains no layers, the first named layer is used.
     *
     * @return the requested object.
     */
    protected object doCreateFromCapabilities(OGCCapabilities caps, AVList parameters)
    {
        return null;
    }

    /**
     * Implemented by subclasses to perform the actual object creation. This default implementation always returns
     * null.
     *
     * @param caps   the capabilities document.
     * @param parameters a list of configuration properties. These properties override any specified in the capabilities
     *               document.
     *
     * @return the requested object.
     */
    protected object doCreateFromCapabilities(WCS100Capabilities caps, AVList parameters)
    {
        return null;
    }

    protected object doCreateFromElement(Element domElement, AVList parameters) throws Exception
    {
        return null;
    }
}
}
