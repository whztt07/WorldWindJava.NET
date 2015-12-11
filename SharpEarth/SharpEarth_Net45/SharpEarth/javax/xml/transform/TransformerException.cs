using System;
using System.Runtime.Serialization;

namespace SharpEarth.javax.xml.transform
{
  [Serializable]
  internal class TransformerException : Exception
  {
    public TransformerException()
    {
    }

    public TransformerException(string message) : base(message)
    {
    }

    public TransformerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TransformerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}