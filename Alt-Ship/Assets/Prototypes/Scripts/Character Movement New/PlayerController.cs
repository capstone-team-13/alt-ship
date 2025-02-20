using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Animator controller for player
        /// </summary>
        [Header("References")] [SerializeField]
        private Animator m_animator = null;

        [SerializeField] private Transform m_position;
        [SerializeField] private Transform m_rotation;

        [UsedImplicitly]
        private void LateUpdate()
        {
            Vector3 currentInput = UserInput;
            m_animator.SetFloat("inputX", currentInput.x);
            m_animator.SetFloat("inputZ", currentInput.z);
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.yellow;
            Debug.DrawRay(transform.position + Vector3.up * 0.01f, Rotation.forward * 5.0f);
        }

        private Vector3 m_userInput;

        private float m_jumpInput;

        public Vector3 UserInput
        {
            get => m_userInput;
            set => m_userInput = value;
        }

        public float JumpInput
        {
            get => m_jumpInput;
            set => m_jumpInput = value;
        }

        public Transform Position => m_position;

        public Transform Rotation
        {
            get => m_rotation;
            set => m_rotation = value;
        }
    }
}