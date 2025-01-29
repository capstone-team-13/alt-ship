using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;
        [SerializeField] private float m_rotationSpeed = 10.0f;
        private float m_direction;

        [UsedImplicitly]
        private void Update()
        {
            var mouseX = Input.GetAxis("Mouse X");

            mouseX = Mathf.Clamp(mouseX, -2, 2);
            mouseX = Remap(mouseX, -2, 2, -1, 1);

            m_direction = Mathf.Lerp(m_direction, mouseX, m_rotationSpeed * Time.deltaTime);

            Debug.Log($"MouseX: {mouseX}, Direction: {m_direction}");

            m_animator.SetFloat("Direction", m_direction);
        }

        private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Clamp(toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin), toMin, toMax);
        }
    }
}