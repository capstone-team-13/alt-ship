using UnityEngine;

namespace EE.AMVCC
{
    public abstract class Element<TModel, TView, TController> : MonoBehaviour
        where TModel : Model
        where TView : View
        where TController : IController
    {
        protected Application<TModel, TView, TController> App;

        public void Start()
        {
            App = FindObjectOfType<Application<TModel, TView, TController>>();
            if (App == null) Debug.LogError($"Application<{typeof(TModel).Name}, {typeof(TView).Name}, {typeof(TController).Name}> component not found in the root object. Ensure that the root object has an Application component matching these types.");
        }
    }
}
