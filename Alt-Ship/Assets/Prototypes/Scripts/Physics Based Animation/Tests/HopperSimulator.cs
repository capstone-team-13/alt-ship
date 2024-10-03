using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class HopperSimulator : MonoBehaviour
    {
        [Header("Sensors")] private CollisionSensor m_groundSensor;

        private float3 m_targetVelocity;

        [Header("State Management")] private int m_stateIndex;
        private const int MAX_STATE_COUNT = 2;
        private System.Action[] m_onFixedUpdate;

        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            m_onFixedUpdate = new System.Action[] { __M_Stance, __M_Flight };
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            m_groundSensor.OnCollided.AddListener(__M__OnGround);
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            m_groundSensor.OnCollided.RemoveListener(__M__OnGround);
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            m_groundSensor.OnCollided.RemoveListener(__M__OnGround);
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            m_onFixedUpdate[m_stateIndex]();
        }

        #endregion

        #region Internal

        private void __M__OnGround(Collision collisionInfo)
        {
            Debug.Log("Collided");
        }

        private void __M_SwitchState()
        {
            ++m_stateIndex;
            m_stateIndex %= MAX_STATE_COUNT;
        }

        private void __M_SwitchState(int index)
        {
            m_stateIndex = index;
        }

        private void __M_Stance()
        {
        }

        private void __M_Flight()
        {
        }

        #endregion
    }
}