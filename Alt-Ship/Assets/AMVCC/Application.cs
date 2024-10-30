using UnityEngine;

namespace EE.AMVCC
{
    public abstract class Application<TModel, TView, TController> : MonoBehaviour
        where TModel : Model
        where TView : View
        where TController : IController
    {
        public TModel Model;
        public TView View;
        public TController Controller;

        public virtual void Notify(string eventType, Object sender, params object[] data)
        {
            Controller.OnNotified(eventType, sender, data);
        }
    }

}
