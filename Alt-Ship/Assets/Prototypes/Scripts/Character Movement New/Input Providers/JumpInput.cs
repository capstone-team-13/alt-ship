using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class JumpInput : MonoBehaviour
    {
        [SerializeField] private PlayerController m_controller;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            var jump = Input.GetAxis("Jump");

            m_controller.JumpInput = jump;
        }
    }
}