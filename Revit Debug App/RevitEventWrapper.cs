using Autodesk.Revit.UI;
using System;

namespace RevitTemplate
{
    /// <summary>
    /// Class for creating Argument (Wrapped) External Events
    /// </summary>
    /// <typeparam name="TType">The Class type being wrapped for the External Event Handler.</typeparam>
    public abstract class RevitEventWrapper<TType> : IExternalEventHandler
    {
        private object _lock;
        private TType _savedArgs;
        private ExternalEvent _revitEvent;

        public RevitEventWrapper()
        {
            _revitEvent = ExternalEvent.Create(this);
            _lock = new object();
        }

        public void Execute(UIApplication app)
        {
            TType args;

            lock (_lock)
            {
                args = _savedArgs;
                _savedArgs = default(TType);
            }

            Execute(app, args);
        }

        public string GetName()
        {
            return GetType().Name;
        }

        public void Raise(TType args)
        {
            lock (_lock)
            {
                _savedArgs = args;
            }

            _revitEvent.Raise();
        }

        public abstract void Execute(UIApplication app, TType args);
    }

    public abstract class SimpleExternalEvent : IExternalEventHandler
    {
        public Func<UIDocument, string> SomeMethod;

        public SimpleExternalEvent(Func<UIDocument, string> someMethod)
        {
            //var temp = someMethod;
            this.SomeMethod = someMethod;
        }

        public void Execute(UIApplication uiApp)
        {
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            string returnString = SomeMethod(uiDoc);
        }

        public string GetName()
        {
            return "Simple External Event";
        }
    }

    //public abstract class ExternalEventExample : IExternalEventHandler
    //{
    //    public void Execute(UIApplication uiApp)
    //    {
    //        UIDocument uiDoc = uiApp.ActiveUIDocument;
    //        RevisionCloud cloud = UtilST.GetSingleUserSelection(uiDoc) as RevisionCloud;

    //        string readString = cloud.ToString();
    //        TaskDialog.Show("External Event", readString);
    //    }

    //    public string GetName()
    //    {
    //        return "External Event Example";
    //    }
    //}
    //public class EventHandlerWithStringArg : RevitEventWrapper<string>
    //{
    //    public override void Execute(UIApplication uiApp, string args)
    //    {
    //        // Do your processing here with "args"
    //        TaskDialog.Show("External Event", args);
    //    }
    //}
}