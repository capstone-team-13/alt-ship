using UnityEngine;

namespace EE.QC
{
    public class LegReference : MonoBehaviour
    {
        [SerializeField] private Transform m_hipJoint;
        [SerializeField] private Transform m_upperJoint;
        [SerializeField] private Transform m_lowerJoint;

        public void Tick(LegFrame frame)
        {
            if (frame == null) return;

            var lerpSpeed = 5.0f * Time.deltaTime;

            m_hipJoint.transform.position =
                Vector3.Lerp(m_hipJoint.transform.position, frame.positions[0], lerpSpeed);
            m_upperJoint.transform.position =
                Vector3.Lerp(m_upperJoint.transform.position, frame.positions[1], lerpSpeed);
            m_lowerJoint.transform.position =
                Vector3.Lerp(m_lowerJoint.transform.position, frame.positions[2], lerpSpeed);

            m_hipJoint.transform.rotation =
                Quaternion.Slerp(m_hipJoint.transform.rotation, frame.rotations[0], lerpSpeed);
            m_upperJoint.transform.rotation =
                Quaternion.Slerp(m_upperJoint.transform.rotation, frame.rotations[1], lerpSpeed);
            m_lowerJoint.transform.rotation =
                Quaternion.Slerp(m_lowerJoint.transform.rotation, frame.rotations[2], lerpSpeed);
        }
    }
}