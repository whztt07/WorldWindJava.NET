﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEarth.java.io;

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

    public InputStream openStream()
    {
      throw new NotImplementedException();
    }
  }
}
