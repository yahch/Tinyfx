using System;
using Tinyfx.Utils;

namespace Tinyfx.Cores
{
    public static class TryExecute
    {
        public static void Execute(Action act, Action<Exception> onError = null)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
                onError?.Invoke(ex);
            }
        }

        public static T Execute<T>(Func<T> func, Func<Exception, T> onError = null)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
                return onError != null ? onError(ex) : default(T);
            }
        }
    }
}