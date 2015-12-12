using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javax.xml.stream
{
  public class XMLStreamException : Exception
  {
    public XMLStreamException(string message)
      : base(message)
    {

    }
  }
}
