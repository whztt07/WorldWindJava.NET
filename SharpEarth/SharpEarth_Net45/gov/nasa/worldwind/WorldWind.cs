/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Runtime.CompilerServices;
using java.beans;
using SharpEarth.util;
using SharpEarth.avlist;
using SharpEarth.cache;
using SharpEarth.exception;
using SharpEarth.formats.tiff;
using SharpEarth.retrieve;
using java.lang;

namespace SharpEarth{




/**
 * @author Tom Gaskins
 * @version $Id: WorldWind.java 1171 2013-02-11 21:45:02Z dcollins $
 */
public sealed class WorldWind
{
    public static readonly String SHUTDOWN_EVENT = "gov.nasa.worldwind.ShutDown";

    // Altitude modes
    public static readonly int ABSOLUTE = 0;
    public static readonly int CLAMP_TO_GROUND = 1;
    public static readonly int RELATIVE_TO_GROUND = 2;
    public static readonly int CONSTANT = 3;

    // Path types (Don't use these. Use the AVKey versions instead. Only Polyline still uses these.)
    public readonly static int GREAT_CIRCLE = 0;
    public readonly static int LINEAR = 1;
    public readonly static int RHUMB_LINE = 2;

    // Anti-alias hints
    public readonly static int ANTIALIAS_DONT_CARE = GL.GL_DONT_CARE;
    public readonly static int ANTIALIAS_FASTEST = GL.GL_FASTEST;
    public readonly static int ANTIALIAS_NICEST = GL.GL_NICEST;

    private static WorldWind instance = new WorldWind();

    private WWObjectImpl wwo;
    private MemoryCacheSet memoryCacheSet;
    private FileStore dataFileStore;
    private RetrievalService remoteRetrievalService;
    private RetrievalService localRetrievalService;
    private TaskService taskService;
    private ScheduledTaskService scheduledTaskService;
    private NetworkStatus networkStatus;
    private SessionCache sessionCache;

    private WorldWind() // Singleton, prevent public instantiation.
    {
        this.initialize();
    }

    private void initialize()
    {
        this.wwo = new WWObjectImpl();
        this.remoteRetrievalService = (RetrievalService) createConfigurationComponent(
            AVKey.RETRIEVAL_SERVICE_CLASS_NAME);
        this.localRetrievalService = (RetrievalService) createConfigurationComponent(
            AVKey.RETRIEVAL_SERVICE_CLASS_NAME);
        this.taskService = (TaskService) createConfigurationComponent(AVKey.TASK_SERVICE_CLASS_NAME);
        this.dataFileStore = (FileStore) createConfigurationComponent(AVKey.DATA_FILE_STORE_CLASS_NAME);
        this.memoryCacheSet = (MemoryCacheSet) createConfigurationComponent(AVKey.MEMORY_CACHE_SET_CLASS_NAME);
        this.networkStatus = (NetworkStatus) createConfigurationComponent(AVKey.NETWORK_STATUS_CLASS_NAME);
        this.sessionCache = (SessionCache) createConfigurationComponent(AVKey.SESSION_CACHE_CLASS_NAME);
        this.scheduledTaskService = new BasicScheduledTaskService();

        // Seems like an unlikely place to load the tiff reader, but do it here nonetheless.
        IIORegistry.getDefaultInstance().registerServiceProvider(GeotiffImageReaderSpi.inst());
    }

    private void dispose()
    {
        if (this.taskService != null)
            this.taskService.shutdown(true);
        if (this.remoteRetrievalService != null)
            this.remoteRetrievalService.shutdown(true);
        if (this.localRetrievalService != null)
            this.localRetrievalService.shutdown(true);
        if (this.memoryCacheSet != null)
            this.memoryCacheSet.clear();
        if (this.sessionCache != null)
            this.sessionCache.clear();
        if (this.scheduledTaskService != null)
            this.scheduledTaskService.shutdown(true);
    }

    /**
     * Reinitialize World Wind to its initial ready state. Shut down and restart all World Wind services and clear all
     * World Wind memory caches. Cache memory will be released at the next JVM garbage collection.
     * <p/>
     * Call this method to reduce World Wind's current resource usage to its initial, empty state. This is typically
     * required by applets when the user leaves the applet page.
     * <p/>
     * The state of any open {@link WorldWindow} objects is indeterminate subsequent to invocation of this method. The
     * core WorldWindow objects attempt to shut themselves down cleanly during the call, but their resulting window
     * state is undefined.
     * <p/>
     * World Wind can continue to be used after calling this method.
     */
    public static synchronized void shutDown()
    {
        instance.wwo.firePropertyChange(SHUTDOWN_EVENT, null, -1);
        instance.dispose();
        instance = new WorldWind();
    }

    public static MemoryCacheSet getMemoryCacheSet()
    {
        return instance.memoryCacheSet;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public static MemoryCache getMemoryCache(string key)
    {
        return instance.memoryCacheSet.getCache(key);
    }

    public static FileStore getDataFileStore()
    {
        return instance.dataFileStore;
    }

    public static RetrievalService getRetrievalService()
    {
        return instance.remoteRetrievalService;
    }

    public static RetrievalService getRemoteRetrievalService()
    {
        return instance.remoteRetrievalService;
    }

    public static RetrievalService getLocalRetrievalService()
    {
        return instance.localRetrievalService;
    }

    public static TaskService getTaskService()
    {
        return instance.taskService;
    }

    /**
     * Get the scheduled task service. This service can be used to scheduled tasks that execute after a delay, or
     * execute repeatedly.
     *
     * @return the scheduled task service.
     */
    public static ScheduledTaskService getScheduledTaskService()
    {
        return instance.scheduledTaskService;
    }

    public static NetworkStatus getNetworkStatus()
    {
        return instance.networkStatus;
    }

    public static SessionCache getSessionCache()
    {
        return instance.sessionCache;
    }

    /**
     * Indicates whether World Wind will attempt to connect to the network to retrieve data or for other reasons.
     *
     * @return <code>true</code> if World Wind is in off-line mode, <code>false</code> if not.
     *
     * @see NetworkStatus
     */
    public static bool isOfflineMode()
    {
        return getNetworkStatus().isOfflineMode();
    }

    /**
     * Indicate whether World Wind should attempt to connect to the network to retrieve data or for other reasons. The
     * default value for this attribute is <code>false</code>, indicating that the network should be used.
     *
     * @param offlineMode <code>true</code> if World Wind should use the network, <code>false</code> otherwise
     *
     * @see NetworkStatus
     */
    public static void setOfflineMode(bool offlineMode)
    {
        getNetworkStatus().setOfflineMode(offlineMode);
    }

    /**
     * @param className the full name, including package names, of the component to create
     *
     * @return the new component
     *
     * @throws WWRuntimeException       if the <code>Object</code> could not be created
     * @throws ArgumentException if <code>className</code> is null or zero length
     */
    public static object createComponent(string className)
    {
        if (className == null || className.Length == 0)
        {
            Logging.logger().severe("nullValue.ClassNameIsNull");
            throw new ArgumentException(Logging.getMessage("nullValue.ClassNameIsNull"));
        }

        try
        {
            Class c = Class.forName(className.Trim());
            return c.newInstance();
        }
        catch (Exception e)
        {
            Logging.logger().log(Level.SEVERE, "WorldWind.ExceptionCreatingComponent", className);
            throw new WWRuntimeException(Logging.getMessage("WorldWind.ExceptionCreatingComponent", className), e);
        }
    }

    /**
     * @param classNameKey the key identifying the component
     *
     * @return the new component
     *
     * @throws IllegalStateException    if no name could be found which corresponds to <code>classNameKey</code>
     * @throws ArgumentException if <code>classNameKey<code> is null
     * @throws WWRuntimeException       if the component could not be created
     */
    public static object createConfigurationComponent(string classNameKey)
    {
        if (classNameKey == null || classNameKey.Length == 0)
        {
            Logging.logger().severe("nullValue.ClassNameKeyNullZero");
            throw new ArgumentException(Logging.getMessage("nullValue.ClassNameKeyNullZero"));
        }

        String name = Configuration.getStringValue(classNameKey);
        if (name == null)
        {
            Logging.logger().log(Level.SEVERE, "WorldWind.NoClassNameInConfigurationForKey", classNameKey);
            throw new WWRuntimeException(
                Logging.getMessage("WorldWind.NoClassNameInConfigurationForKey", classNameKey));
        }

        try
        {
            return WorldWind.createComponent(name.Trim());
        }
        catch (Exception e)
        {
            Logging.logger().log(Level.SEVERE, "WorldWind.UnableToCreateClassForConfigurationKey", name);
            throw new IllegalStateException(
                Logging.getMessage("WorldWind.UnableToCreateClassForConfigurationKey", name), e);
        }
    }

    public static void setValue(String key, Object value)
    {
        instance.wwo.setValue(key, value);
    }

    public static void setValue(String key, String value)
    {
        instance.wwo.setValue(key, value);
    }

    public static Object getValue(String key)
    {
        return instance.wwo.getValue(key);
    }

    public static String getStringValue(String key)
    {
        return instance.wwo.getStringValue(key);
    }

    public static bool hasKey(String key)
    {
        return instance.wwo.hasKey(key);
    }

    public static void removeKey(String key)
    {
        instance.wwo.removeKey(key);
    }

    public static void addPropertyChangeListener(String propertyName, PropertyChangeListener listener)
    {
        instance.wwo.addPropertyChangeListener(propertyName, listener);
    }

    public static void removePropertyChangeListener(String propertyName, PropertyChangeListener listener)
    {
        instance.wwo.removePropertyChangeListener(propertyName, listener);
    }

    public static void addPropertyChangeListener(PropertyChangeListener listener)
    {
        instance.wwo.addPropertyChangeListener(listener);
    }

    public static void removePropertyChangeListener(PropertyChangeListener listener)
    {
        instance.wwo.removePropertyChangeListener(listener);
    }
}
}
