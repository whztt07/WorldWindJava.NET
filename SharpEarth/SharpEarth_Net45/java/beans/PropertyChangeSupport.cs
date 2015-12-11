using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.beans;

namespace SharpEarth.java.beans
{
  public class PropertyChangeSupport
  {
    private object sourceBean;

    public PropertyChangeSupport(object sourceBean)
    {
      this.sourceBean = sourceBean;
    }

    internal void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
    {
      throw new NotImplementedException();
    }

    internal void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
    {
      throw new NotImplementedException();
    }

    internal void addPropertyChangeListener(PropertyChangeListener listener)
    {
      throw new NotImplementedException();
    }

    internal void removePropertyChangeListener(PropertyChangeListener listener)
    {
      throw new NotImplementedException();
    }

    internal void firePropertyChange(PropertyChangeEvent propertyChangeEvent)
    {
      throw new NotImplementedException();
    }

    internal void firePropertyChange(System.String propertyName, System.Object oldValue, System.Object newValue)
    {
      throw new NotImplementedException();
    }
  }
}
