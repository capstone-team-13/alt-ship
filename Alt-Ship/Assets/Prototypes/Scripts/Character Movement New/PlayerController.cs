using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private BodyController m_body;
        [SerializeField] private LegManager m_leg;

        private INotifiable[] m_pieces;

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

        [UsedImplicitly]
        private void Start()
        {
            m_pieces = GetComponentsInChildren<INotifiable>();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            var userInput = UserInput;
            var jumpInput = JumpInput;

            foreach (INotifiable piece in m_pieces)
                piece.Notify(userInput, jumpInput);
        }

        public void SetTargetPositionXZ(Vector3 position)
        {
            m_body.SetTargetPositionXZ(position);
        }
    }
}