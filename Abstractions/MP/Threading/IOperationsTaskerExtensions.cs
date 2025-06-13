
using System;
using System.Threading.Tasks;

namespace MP.Threading
{
    /// <summary>
    /// Adds extension methods focusing around the <see cref="IOperationsTasker"/> interface.
    /// </summary>
    public static class IOperationsTaskerExtensions
    {
        /// <summary>
        /// Queues the specified parameterless method to be executed by the tasker.
        /// </summary>
        /// <param name="tasker">The operations tasker to queue the specified parameterless method to.</param>
        /// <param name="methoditem">The method to queue and execute into the operation tasker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="methoditem"/> was <see langword="null"/>.</exception>
        public static void Add(this IOperationsTasker tasker, Action methoditem)
        {
            if (methoditem is null) {
                throw new ArgumentNullException(nameof(methoditem));
            }
            tasker.Add(new(methoditem));
        }

        /// <summary>
        /// Queues the specified special method to be executed by the tasker.
        /// </summary>
        /// <typeparam name="T">The delegate type of the method to execute.</typeparam>
        /// <param name="tasker">The operations tasker to queue the specified method to.</param>
        /// <param name="method">The method to queue and execute into the operation tasker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> was <see langword="null"/>.</exception>
        public static void Add<T>(this IOperationsTasker tasker, T method) where T : Delegate
        {
            if (method is null) { 
                throw new ArgumentNullException(nameof(method));
            }
            tasker.Add(new(method));
        }

        /// <summary>
        /// Queues the specified special method with the specified arguments to be executed by the tasker.
        /// </summary>
        /// <typeparam name="T">The delegate type of the method to execute.</typeparam>
        /// <param name="tasker">The operations tasker to queue the specified method to.</param>
        /// <param name="method">The method to queue and execute into the operation tasker.</param>
        /// <param name="arguments">The arguments that the <paramref name="method"/> requires so that it can execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> was <see langword="null"/>.</exception>
        public static void Add<T>(this IOperationsTasker tasker, T method, params System.Object[] arguments) where T : Delegate
        {
            if (method is null) {
                throw new ArgumentNullException(nameof(method));
            }
            tasker.Add(new(method , arguments: arguments));
        }

        /// <summary>
        /// Starts the tasker thread asyncronously. <br />
        /// The implemenentation code of the <see cref="IOperationsTasker.Run"/> method may be long , so
        /// this method becomes suitable to run the code to start it asyncronously.
        /// </summary>
        /// <param name="tasker">The operations tasker to start it's tasker thread asyncronously.</param>
        /// <returns>A <see cref="ValueTask"/> representing the running task.</returns>
        public static ValueTask RunAsync(this IOperationsTasker tasker) => new(Task.Run(tasker.Run));

        /// <summary>
        /// Stops and processes all the tasker thread work items asyncronously. <br />
        /// The <see cref="IOperationsTasker.StopAndProcessAll"/> method may require a long time to finish,
        /// so this method becomes suitable to run the code to process all these asyncronously.
        /// </summary>
        /// <param name="tasker">The operations tasker to start the <see cref="IOperationsTasker.StopAndProcessAll"/> method asyncronously.</param>
        /// <returns>A <see cref="ValueTask"/> representing the running task.</returns>
        public static ValueTask StopAndProcessAllAsync(this IOperationsTasker tasker) => new(Task.Run(tasker.StopAndProcessAll));
    }
}