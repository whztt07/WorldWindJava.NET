using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEarth.java
{
  public static class System
  {
    private static readonly DateTime Jan1st1970 = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

    public static long currentTimeMillis()
    {
      return (long)( DateTime.UtcNow - Jan1st1970 ).TotalMilliseconds;
    }

    public static long getTime( this DateTime datetime)
    {
      return (long)( DateTime.UtcNow - datetime ).TotalMilliseconds;
    }

    public static bool after( this DateTime time, DateTime when )
    {
      return time.getTime() > when.getTime();
    }
  }
}
