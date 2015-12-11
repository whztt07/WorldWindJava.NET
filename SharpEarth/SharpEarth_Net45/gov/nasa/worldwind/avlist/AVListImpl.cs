/*
 * Copyright (C) 2012 United States Government as represented by the Administrator of the
 * National Aeronautics and Space Administration.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using java.util;
using java.beans;
using SharpEarth.util;
using SharpEarth.exception;
using SharpEarth.view.orbit;
using SharpEarth.java.beans;

namespace SharpEarth.avlist{



/**
 * An implementation class for the {@link AVList} interface. Classes implementing <code>AVList</code> can subclass or
 * aggregate this class to provide default <code>AVList</code> functionality. This class maintains a hash table of
 * attribute-value pairs.
 * <p/>
 * This class implements a notification mechanism for attribute-value changes. The mechanism provides a means for
 * objects to observe attribute changes or queries for certain keys without explicitly monitoring all keys. See {@link
 * java.beans.PropertyChangeSupport}.
 *
 * @author Tom Gaskins
 * @version $Id: AVListImpl.java 2255 2014-08-22 17:36:32Z tgaskins $
 */
public class AVListImpl : AVList
{
    // Identifies the property change support instance in the avlist
    private static readonly string PROPERTY_CHANGE_SUPPORT = "avlist.PropertyChangeSupport";

    // To avoid unnecessary overhead, this object's hash map is created only if needed.
    private Dictionary<string, object> _avList;

    /** Creates an empty attribute-value list. */
    public AVListImpl()
    {
    }

    /**
     * Constructor enabling aggregation
     *
     * @param sourceBean The bean to be given as the source for any events.
     */
    public AVListImpl(object sourceBean)
    {
        if (sourceBean != null)
            this.setValue(PROPERTY_CHANGE_SUPPORT, new PropertyChangeSupport(sourceBean));
    }

    private bool hasAvList()
    {      
      return this._avList != null;
    }

    private Dictionary<string,object> createAvList()
    {
        if (!this.hasAvList())
        {
            // The map type used must accept null values. java.util.concurrent.ConcurrentHashMap does not.
            this._avList = new Dictionary<string, object>(1);
        }
        return this._avList;
    }

    private Dictionary<string, object> avList(bool createIfNone)
    {
        if (createIfNone && !hasAvList())
            createAvList();

        return _avList;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public object getValue(string key)
    {
        if (key == null)
        {
            string message = Logging.getMessage("nullValue.AttributeKeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

      if ( hasAvList() )
        return _avList[key];

        return null;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public IEnumerable<object> getValues()
    {
      return avList( true ).Values.AsEnumerable();
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public Dictionary<string,object> getEntries()
    {
      return avList( true );
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public string getStringValue(string key)
    {
        if (key == null)
        {
            string msg = Logging.getMessage("nullValue.AttributeKeyIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }

      string value = this.getValue( key ) as string;
      if ( value != null )
        return value;
      string message = Logging.getMessage( "AVAAccessibleImpl.AttributeValueForKeyIsNotAString", key );
      Logging.logger().severe( message );
      throw new WWRuntimeException( message, new InvalidCastException() );
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public object setValue(string key, object value)
    {
        if (key == null)
        {
            string message = Logging.getMessage("nullValue.AttributeKeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

      object prevObject;
      this.avList( true ).TryGetValue( key, out prevObject );
      this.avList( true )[key] = value;
      return prevObject;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public AVList setValues(AVList list)
    {
        if (list == null)
        {
            String message = Logging.getMessage("nullValue.AttributesIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

      var entries = list.getEntries();
      foreach (var entry in entries)
      {
        setValue(entry.Key, entry.Value);
      }
      return this;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public bool hasKey(string key)
    {
        if (key == null)
        {
            string message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }
        return hasAvList() && _avList.ContainsKey( key );
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public object removeKey(string key)
    {
        if (key == null)
        {
            String message = Logging.getMessage("nullValue.KeyIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }


      object value = null;
      if ( hasAvList() && _avList.TryGetValue( key, out value ) )
        _avList.Remove( key );
      return value;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public AVList copy()
    {
        AVListImpl clone = new AVListImpl();
        if (hasAvList())
        {
          clone.createAvList();
          foreach ( var entry in _avList )
            clone._avList.Add( entry.Key, entry.Value );
        }
        return clone;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public AVList clearList()
    {
      if ( hasAvList() )
        _avList.Clear();
      return this;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    protected PropertyChangeSupport getChangeSupport()
    {
        Object pcs = this.getValue(PROPERTY_CHANGE_SUPPORT);
        if (pcs == null || !(pcs is PropertyChangeSupport))
        {
            pcs = new PropertyChangeSupport(this);
            this.setValue(PROPERTY_CHANGE_SUPPORT, pcs);
        }

        return (PropertyChangeSupport) pcs;
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public void addPropertyChangeListener(String propertyName, PropertyChangeListener listener)
    {
        if (propertyName == null)
        {
            String msg = Logging.getMessage("nullValue.PropertyNameIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if (listener == null)
        {
            String msg = Logging.getMessage("nullValue.ListenerIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().addPropertyChangeListener(propertyName, listener);
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
    {
        if (propertyName == null)
        {
            String msg = Logging.getMessage("nullValue.PropertyNameIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        if (listener == null)
        {
            String msg = Logging.getMessage("nullValue.ListenerIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().removePropertyChangeListener(propertyName, listener);
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public void addPropertyChangeListener(PropertyChangeListener listener)
    {
        if (listener == null)
        {
            String msg = Logging.getMessage("nullValue.ListenerIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().addPropertyChangeListener(listener);
    }

    [MethodImpl( MethodImplOptions.Synchronized )]
    public void removePropertyChangeListener(PropertyChangeListener listener)
    {
        if (listener == null)
        {
            String msg = Logging.getMessage("nullValue.ListenerIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().removePropertyChangeListener(listener);
    }

    public void firePropertyChange(PropertyChangeEvent propertyChangeEvent)
    {
        if (propertyChangeEvent == null)
        {
            String msg = Logging.getMessage("nullValue.PropertyChangeEventIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().firePropertyChange(propertyChangeEvent);
    }

    public void firePropertyChange(string propertyName, object oldValue, object newValue )
    {
        if (propertyName == null)
        {
            String msg = Logging.getMessage("nullValue.PropertyNameIsNull");
            Logging.logger().severe(msg);
            throw new ArgumentException(msg);
        }
        this.getChangeSupport().firePropertyChange(propertyName, oldValue, newValue);
    }

    // Static AVList utilities.
    public static string getStringValue(AVList avList, String key, String defaultValue)
    {
        String v = getStringValue(avList, key);
        return v != null ? v : defaultValue;
    }

    public static String getStringValue(AVList avList, String key)
    {
        try
        {
            return avList.getStringValue(key);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static int? getIntegerValue(AVList avList, string key, int? defaultValue)
    {
        int? v = getIntegerValue(avList, key);
        return v != null ? v : defaultValue;
    }

    public static int? getIntegerValue(AVList avList, string key)
    {
        object o = avList.getValue(key);
        if (o == null)
            return null;

        if (o is int?)
            return (int?) o;

        string v = getStringValue(avList, key);
        if (v == null)
            return null;

        try
        {
            return int.Parse(v);
        }
        catch (Exception e)
        {
            Logging.logger().log(Level.SEVERE, "Configuration.ConversionError", v);
            return null;
        }
    }

    public static long? getLongValue(AVList avList, string key, long? defaultValue)
    {
        long? v = getLongValue(avList, key);
        return v != null ? v : defaultValue;
    }

    public static long? getLongValue(AVList avList, string key)
    {
        Object o = avList.getValue(key);
        if (o == null)
            return null;

        if (o is long?)
            return (long?) o;

        string v = getStringValue(avList, key);
        if (v == null)
            return null;

        try
        {
            return long.Parse(v);
        }
        catch (Exception e)
        {
            Logging.logger().log(Level.SEVERE, "Configuration.ConversionError", v);
            return null;
        }
    }

    public static double? getDoubleValue(AVList avList, string key, double? defaultValue)
    {
      double? v = getDoubleValue(avList, key);
        return v != null ? v : defaultValue;
    }

    public static double? getDoubleValue(AVList avList, string key)
    {
        object o = avList.getValue(key);
        if (o == null)
            return null;

        if (o is double)
            return (double) o;

        string v = getStringValue(avList, key);
        if (v == null)
            return null;

        try
        {
            return double.Parse(v);
        }
        catch (Exception e)
        {
            Logging.logger().log(Level.SEVERE, "Configuration.ConversionError", v);
            return null;
        }
    }

    public void getRestorableStateForAVPair(string key, object value, RestorableSupport rs,
        RestorableSupport.StateObject context)
    {
        if (value == null)
            return;

        if (key.Equals(PROPERTY_CHANGE_SUPPORT))
            return;

        if (rs == null)
        {
            String message = Logging.getMessage("nullValue.RestorableStateIsNull");
            Logging.logger().severe(message);
            throw new ArgumentException(message);
        }

        rs.addStateValueAsString(context, key, value.ToString());
    }

    public static bool? getBooleanValue(AVList avList, string key, bool? defaultValue)
    {
        bool? v = getBooleanValue(avList, key);
        return v != null ? v : defaultValue;
    }

    public static bool? getBooleanValue(AVList avList, string key)
    {
        object o = avList.getValue(key);
        if (o == null)
        {
          return null;
        }
            
        if(o is bool)
        {
          return (bool)o;
        }

        if ( !( o is string ) )
        {
          return null;
        }

      string text = (string)o;
      bool result;
      if(bool.TryParse(text, out result))
      {
        return result;
      }
      Logging.logger().log( Level.SEVERE, "Configuration.ConversionError", new Exception().Message );
      return null;
    }
  }
}
