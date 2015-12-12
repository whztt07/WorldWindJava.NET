using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace java.awt
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
  }
}
