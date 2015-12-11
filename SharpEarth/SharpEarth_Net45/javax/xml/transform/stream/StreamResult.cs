using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEarth.java.io;

namespace SharpEarth.javax.xml.transform.stream
{
  public class StreamResult
  {
    private StringWriter stringWriter;

    public StreamResult(StringWriter stringWriter)
    {
      this.stringWriter = stringWriter;
    }
  }
}
