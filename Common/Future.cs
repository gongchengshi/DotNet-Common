using System;

namespace Gongchengshi
{
   public class Future<T>
   {
      public delegate R FutureDelegate<R>();
      public Future(FutureDelegate<T> @delegate)
      {
         _delegate = @delegate;
         _result = @delegate.BeginInvoke(null, null);
      }
      private FutureDelegate<T> _delegate;
      private IAsyncResult _result;
      private T _pValue;
      private bool _hasValue = false;
      private T Value
      {
         get
         {
            if (!_hasValue)
            {
               if (!_result.IsCompleted)
                  _result.AsyncWaitHandle.WaitOne();
               _pValue = _delegate.EndInvoke(_result);
               _hasValue = true;
            }
            return _pValue;
         }
      }
      public static implicit operator T(Future<T> f)
      {
         return f.Value;
      }
   }
}
