using System;
using System.Runtime.Serialization;

namespace org.xml.sax
{
  [Serializable]
  internal class SAXException : Exception
  {
    public SAXException()
    {
    }

    public SAXException(string message) : base(message)
    {
    }

    public SAXException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SAXException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}