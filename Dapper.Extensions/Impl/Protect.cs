using System;

namespace Dapper
{
    class Protect
    {
        public static void Against(bool condition, string message)
        {
            if (!condition) return;

            throw new InvalidOperationException(message);
        }

        public static void Against(Func<bool> condition, string message)
        {
            if (!condition.Invoke()) return;

            throw new InvalidOperationException(message);
        }

        public static void Against<TExcecao>(bool condition, string message) where TExcecao : Exception
        {
            if (!condition) return;

            throw (TExcecao)Activator.CreateInstance(typeof(TExcecao), message);
        }

        public static void Against<TExcecao>(Func<bool> condition, string message) where TExcecao : Exception
        {
            if (!condition.Invoke()) return;

            throw (TExcecao)Activator.CreateInstance(typeof(TExcecao), message);
        }
    }
}
