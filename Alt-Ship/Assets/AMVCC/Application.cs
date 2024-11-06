using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

namespace EE.AMVCC
{
    public class Application : MonoBehaviour
    {
        private static Application m_instance;

        private IController[] m_controllers;

        public static Application Instance
        {
            get
            {
                if (m_instance != null) return m_instance;

                m_instance = FindObjectOfType<Application>();

                if (m_instance != null) return m_instance;

                var singletonObject = new GameObject("Application");
                m_instance = singletonObject.AddComponent<Application>();
                DontDestroyOnLoad(singletonObject);
                return m_instance;
            }
        }

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

            m_controllers = FindObjectsOfType<MonoBehaviour>().OfType<IController>().ToArray();
        }

        public void Push<TCommand>(TCommand command) where TCommand : ICommand
        {
            foreach (var controller in m_controllers) controller.Notify(command);
        }
    }
}