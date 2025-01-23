using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.AMVCC
{
    public class Application : MonoBehaviour
    {
        #region Unity Callbacks

        [UsedImplicitly]
        protected virtual void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (m_instance != this)
            {
                Destroy(gameObject);
            }

            m_controllers = FindObjectsOfType<MonoBehaviour>().OfType<IController>().ToList();
        }

        #endregion

        #region API

        public static Application Instance
        {
            get
            {
                if (m_instance != null) return m_instance;
                m_instance = FindObjectOfType<Application>();
                return m_instance != null ? m_instance : null;
            }
        }

        public void Push<TCommand>(TCommand command) where TCommand : ICommand
        {
            foreach (IController controller in m_controllers) controller.Notify(command);
        }

        public void RegisterController(IController controller)
        {
            m_controllers.Add(controller);
        }

        public void UnregisterController(IController controller)
        {
            m_controllers.Remove(controller);
        }

        #endregion

        #region Internal

        private static Application m_instance;

        private List<IController> m_controllers;

        #endregion
    }
}