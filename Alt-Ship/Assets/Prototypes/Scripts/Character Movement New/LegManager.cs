using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class LegManager : MonoBehaviour
    {
        [SerializeField] LegController[] m_legs;
        private int m_currentLegIndex;

        private float m_timePastSinceLastStep;


        [UsedImplicitly]
        private void FixedUpdate()
        {
            _M_Tick();

            m_timePastSinceLastStep += Time.fixedDeltaTime;
            if (_M_ShouldSwitchLeg())
            {
                m_currentLegIndex = (m_currentLegIndex + 1) % m_legs.Length;
                m_timePastSinceLastStep = 0.0f;
            }
        }

        private void _M_Tick()
        {
            var currentMovingLeg = m_legs[m_currentLegIndex];
            currentMovingLeg.OnTick();
        }

        private bool _M_ShouldSwitchLeg()
        {
            return m_timePastSinceLastStep > 0.5f;
        }
    }
}