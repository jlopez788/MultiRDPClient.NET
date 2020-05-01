using System;

namespace Database
{
    public static class Utility
    {
        public static void Try(this Action action, Action<Exception> onException = null, Action onFinal = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
            finally
            {
                onFinal?.Invoke();
            }
        }
    }
}