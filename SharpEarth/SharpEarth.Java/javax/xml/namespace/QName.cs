using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javax.xml.namespaces
{
  public class QName
  {
    private string v;
    private string xLINK_URI;

    public QName(string xLINK_URI, string v)
    {
      this.xLINK_URI = xLINK_URI;
      this.v = v;
    }

    internal QName getLocalPart()
    {
      throw new NotImplementedException();
    }
  }
}
