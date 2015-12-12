using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEarth.java.awt
{
  public class Rectangle
  {
    public int x;
    public int y;
    public int width;
    public int height;

    public Rectangle()
    {
    }

    public Rectangle( Rectangle viewport )
    {
      x = viewport.x;
      y = viewport.y;
      width = viewport.width;
      height = viewport.height;
    }

    public Rectangle( int viewport, int i, int viewport1, int v8 )
    {
      throw new NotImplementedException();
    }

    public double getWidth()
    {
      throw new NotImplementedException();
    }

    public double getHeight()
    {
      throw new NotImplementedException();
    }
  }
}
