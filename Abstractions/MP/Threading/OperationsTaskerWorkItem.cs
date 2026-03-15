
using System;

namespace MP.Threading
{
    /// <summary>
    /// Represents an item in a <see cref="IOperationsTasker"/>-implementing class work queue.
    /// </summary>
    public class OperationsTaskerWorkItem
    {
        private System.Delegate dlg;
        private System.Object[] arguments;
        
        /// <summary>
        /// Creates a new tasker work item by the specified delegate that represents the method to execute and the additional arguments to be passed to the method once executed.
        /// </summary>
        /// <param name="delegate">The delegate that represents the method to execute.</param>
        /// <param name="arguments">The additional arguments required to execute the delegate.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="delegate"/> was <see langword="null"/>.</exception>
        public OperationsTaskerWorkItem(System.Delegate @delegate , params System.Object[] arguments)
        {
            ArgumentNullException.ThrowIfNull(@delegate);
            this.dlg = @delegate;
            this.arguments = arguments;
        }

        /// <summary>
        /// Represents the delegate that encapsulates the method to be executed.
        /// </summary>
        public System.Delegate MethodToExecute => dlg;

        /// <summary>
        /// The additional arguments to pass to the method represented by <see cref="MethodToExecute"/> , once that method is executed.
        /// </summary>
        public System.Object[] AdditionalMethodArguments => arguments;
    }
}