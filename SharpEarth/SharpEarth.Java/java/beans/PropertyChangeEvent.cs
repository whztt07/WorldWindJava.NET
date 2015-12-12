/**
 * A "PropertyChange" event gets delivered whenever a bean changes a "bound"
 * or "constrained" property.  A PropertyChangeEvent object is sent as an
 * argument to the PropertyChangeListener and VetoableChangeListener methods.
 * <P>
 * Normally PropertyChangeEvents are accompanied by the name and the old
 * and new value of the changed property.  If the new value is a primitive
 * type (such as int or boolean) it must be wrapped as the
 * corresponding java.lang.* Object type (such as Integer or Boolean).
 * <P>
 * Null values may be provided for the old and the new values if their
 * true values are not known.
 * <P>
 * An event source may send a null object as the name to indicate that an
 * arbitrary set of if its properties have changed.  In this case the
 * old and new values should also be null.
 */

using System.Text;
using java.util;

namespace java.beans
{
  public class PropertyChangeEvent : EventObject
  {
    private const long serialVersionUID = 7042693688939648123L;


    /**
     * Constructs a new {@code PropertyChangeEvent}.
     *
     * @param source        the bean that fired the event
     * @param propertyName  the programmatic name of the property that was changed
     * @param oldValue      the old value of the property
     * @param newValue      the new value of the property
     *
     * @throws IllegalArgumentException if {@code source} is {@code null}
     */
    public PropertyChangeEvent( object source, string propertyName,
                               object oldValue, object newValue ) : base(source)
    {
      this.propertyName = propertyName;
      this.newValue = newValue;
      this.oldValue = oldValue;
    }

    /**
     * Gets the programmatic name of the property that was changed.
     *
     * @return  The programmatic name of the property that was changed.
     *          May be null if multiple properties have changed.
     */
    public string getPropertyName()
    {
      return propertyName;
    }

    /**
     * Gets the new value for the property, expressed as an Object.
     *
     * @return  The new value for the property, expressed as an Object.
     *          May be null if multiple properties have changed.
     */
    public object getNewValue()
    {
      return newValue;
    }

    /**
     * Gets the old value for the property, expressed as an Object.
     *
     * @return  The old value for the property, expressed as an Object.
     *          May be null if multiple properties have changed.
     */
    public object getOldValue()
    {
      return oldValue;
    }

    /**
     * Sets the propagationId object for the event.
     *
     * @param propagationId  The propagationId object for the event.
     */
    public void setPropagationId( object propagationId )
    {
      this.propagationId = propagationId;
    }

    /**
     * The "propagationId" field is reserved for future use.  In Beans 1.0
     * the sole requirement is that if a listener catches a PropertyChangeEvent
     * and then fires a PropertyChangeEvent of its own, then it should
     * make sure that it propagates the propagationId field from its
     * incoming event to its outgoing event.
     *
     * @return the propagationId object associated with a bound/constrained
     *          property update.
     */
    public object getPropagationId()
    {
      return propagationId;
    }

    /**
     * name of the property that changed.  May be null, if not known.
     * @serial
     */
    private string propertyName;

    /**
     * New value for property.  May be null if not known.
     * @serial
     */
    private object newValue;

    /**
     * Previous value for property.  May be null if not known.
     * @serial
     */
    private object oldValue;

    /**
     * Propagation ID.  May be null.
     * @serial
     * @see #getPropagationId
     */
    private object propagationId;

    /**
     * Returns a string representation of the object.
     *
     * @return a string representation of the object
     *
     * @since 1.7
     */
    public new string toString()
    {
      StringBuilder sb = new StringBuilder( this.GetType().Name );
      sb.Append( "[propertyName=" ).Append( getPropertyName() );
      appendTo( sb );
      sb.Append( "; oldValue=" ).Append( getOldValue() );
      sb.Append( "; newValue=" ).Append( getNewValue() );
      sb.Append( "; propagationId=" ).Append( getPropagationId() );
      sb.Append( "; source=" ).Append( getSource() );
      return sb.Append( "]" ).ToString();
    }

    void appendTo( StringBuilder sb )
    {
    }
  }
}
