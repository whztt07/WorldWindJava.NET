using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEarth.java.org.w3c.dom;

namespace SharpEarth.javax.xml.transform.dom
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
