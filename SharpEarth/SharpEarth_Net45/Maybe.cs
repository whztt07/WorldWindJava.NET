using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpEarth
{
  public static class MaybeExtensions
  {
    public static Maybe<T> ToMaybe<T>( this T value )
    {
      return Maybe<T>.Of( value );
    }
  }

  public sealed class Maybe<T> : IEnumerable<T>
  {
    public static Maybe<T> Empty = new Maybe<T>();
    
    public static Maybe<T> Of( T value )
    {
      return new Maybe<T>( value );
    }

    private readonly IEnumerable<T> value;

    private Maybe()
    {
      this.value = new T[0];
    }

    private Maybe( T value )
    {
      this.value = value == null ?
        new T[0] :
        new T[1] { value };
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.value.GetEnumerator();
    }
  }
}
