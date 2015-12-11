using System;

namespace SharpEarth.java.lang
{
  public class IllegalStateException : Exception
  {
    public IllegalStateException(string message)
      : base(message)
    {

    }

    public IllegalStateException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
