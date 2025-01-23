using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class MovementInput : MonoBehaviour
    {
        [SerializeField] private PlayerController m_controller;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            m_controller.UserInput = new Vector3(x, 0, z);
        }
    }
}