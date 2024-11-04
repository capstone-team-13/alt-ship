using UnityEngine;

namespace EE.AMVCC
{
    public interface IController
    {
        void Notify<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public abstract class Controller<TModel, TView> : MonoBehaviour, IController
        where TModel : Model, new()
        where TView : View
    {
        [SerializeField] private TModel m_model;
        [SerializeField] private TView m_view;

        protected TModel Model => m_model;
        protected TView View => m_view;
        public abstract void Notify<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public abstract class Controller<TModel> : MonoBehaviour, IController
        where TModel : Model, new()
    {
        [SerializeField] private TModel m_model;
        protected TModel Model => m_model;
        public abstract void Notify<TCommand>(TCommand command) where TCommand : ICommand;
    }
}