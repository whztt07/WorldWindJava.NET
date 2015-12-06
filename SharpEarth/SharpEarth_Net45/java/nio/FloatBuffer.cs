using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEarth.geom;

namespace SharpEarth.java.nio
{
  // See usage in SharpEarth.geom.Triangle
  public class FloatBuffer
  {
    public float get( int i )
    {
      throw new NotImplementedException();
    }

    public float get()
    {// note that this increments the index buffer position
      throw new NotImplementedException();
    }

    internal void rewind()
    {
      throw new NotImplementedException();
    }

    internal int limit()
    {
      throw new NotImplementedException();
    }

    internal int position()
    {
      throw new NotImplementedException();
    }

    internal object put( float v )
    {
      throw new NotImplementedException();
    }
  }
}
