using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEarth.java.net
{
  public class URL : java.io.File
  {
    private string v;

    public URL( string v )
    {
      this.v = v;
    }

    internal string getPath()
    {
      throw new NotImplementedException();
    }
  }
}
