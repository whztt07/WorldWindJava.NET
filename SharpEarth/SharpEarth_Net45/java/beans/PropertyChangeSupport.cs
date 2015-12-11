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
  }
}
