using Autodesk.Revit.UI;

namespace RevitTemplate
{
    /// <summary>
    /// Class for creating Argument (Wrapped) External Events
    /// </summary>
    /// <typeparam name="TType">The Class type being wrapped for the External Event Handler.</typeparam>
    public abstract class RevitEventWrapper<TType> : IExternalEventHandler
    {
        private readonly object _lock;
        private TType _savedArgs;
        private readonly ExternalEvent _revitEvent;

        /// <summary>
        /// Class for wrapping methods for execution within a "valid" Revit API context.
        /// </summary>
        protected RevitEventWrapper()
        {
            _revitEvent = ExternalEvent.Create(this);
            _lock = new object();
        }

        /// <summary>
        /// Wraps the "Execution" method in a valid Revit API context.
        /// </summary>
        /// <param name="app">Revit UI Application to use as the "wrapper" API context.</param>
        public void Execute(UIApplication app)
        {
            TType args;

            lock (_lock)
            {
                args = _savedArgs;
                _savedArgs = default;
            }

            Execute(app, args);
        }

        /// <summary>
        /// Get the name of the operation.
        /// </summary>
        /// <returns>Operation Name.</returns>
        public string GetName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Execute the wrapped external event in a valid Revit API context.
        /// </summary>
        /// <param name="args">Arguments that could be passed to the execution method.</param>
        public void Raise(TType args)
        {
            lock (_lock)
            {
                _savedArgs = args;
            }

            _revitEvent.Raise();
        }

        /// <summary>
        /// Override void which wraps the "Execution" method in a valid Revit API context.
        /// </summary>
        /// <param name="app">Revit UI Application to use as the "wrapper" API context.</param>
        /// <param name="args">Arguments that could be passed to the execution method.</param>
        public abstract void Execute(UIApplication app, TType args);
    }
}