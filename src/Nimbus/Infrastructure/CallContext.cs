using System.Threading;
using Nimbus.ConcurrentCollections;
using NullGuard;

namespace Nimbus.Infrastructure
{
    //http://www.cazzulino.com/callcontext-netstandard-netcore.html
    public static class CallContext
    {
        static ThreadSafeDictionary<string, AsyncLocal<object>> state = new ThreadSafeDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void LogicalSetData(string name, [AllowNull]object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object LogicalGetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
}