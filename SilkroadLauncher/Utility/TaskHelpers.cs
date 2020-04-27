using System.Threading;
using System.Threading.Tasks;
namespace SilkroadLauncher.Utility
{
    /// <summary>
    /// Extensions helper for asynchronized tasks
    /// </summary>
    public static class TaskHelpers
    {
        /// <summary>
        /// Await the Task but returns inmediatly if the cancellation has been set
        /// </summary>
        /// <param name="Token">The cancellation token</param>
        public static async Task WaitAsync(this Task Task, CancellationToken Token)
        {
            var tcs = new TaskCompletionSource<object>();
            using (Token.Register(() => tcs.TrySetCanceled(Token), useSynchronizationContext: false))
                await await Task.WhenAny(Task, tcs.Task).ConfigureAwait(false);
        }
        /// <summary>
        /// Await the Task but returns inmediatly if the cancellation has been set
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="Token">The cancellation token</param>
        public static async Task<T> WaitAsync<T>(this Task<T> Task, CancellationToken Token)
        {
            var tcs = new TaskCompletionSource<T>();
            using (Token.Register(() => tcs.TrySetCanceled(Token), useSynchronizationContext: false))
                return await await System.Threading.Tasks.Task.WhenAny(Task, tcs.Task).ConfigureAwait(false);
        }
    }
}
