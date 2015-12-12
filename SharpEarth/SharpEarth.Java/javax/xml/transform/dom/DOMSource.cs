using java.org.w3c.dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javax.xml.transform.dom
{
  public class DOMSource
  {
    private Document doc;

    public DOMSource(Document doc)
    {
      this.doc = doc;
    }
  }
}
