using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.xml.sax
{
  public class InputSource
  {
    private StringReader stringReader;

    public InputSource(StringReader stringReader)
    {
      this.stringReader = stringReader;
    }
  }
}
