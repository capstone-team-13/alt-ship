using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private BodyController m_body;
        [SerializeField] private LegManager m_leg;

        private INotifiable[] m_pieces;

        private void Start()
        {
            m_pieces = GetComponentsInChildren<INotifiable>();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 userInput = _M_GetInput();
            // m_body.Move(userInput);

            var jumpInput = Input.GetAxis("Jump");
            bool jumpPressed = jumpInput > 0;
            // if (jumpPressed) m_body.Jump();

            foreach (INotifiable piece in m_pieces)
                piece.Notify(userInput, jumpInput);
        }

        #region Internal

        private Vector3 _M_GetInput()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            return new Vector3(x, 0, z);
        }

        private void _M_SolveConstraint()
        {
        }

        #endregion
    }
}