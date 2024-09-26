using UnityEngine;

namespace EE.Prototype.OOP
{
    public class GoalView : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private float padding = 10f;
        [SerializeField] private Transform m_goalObject;
        [SerializeField] private RectTransform m_goalImage;

        private void Update()
        {
            Vector3 directionToGoal = m_goalObject.position - Camera.main.transform.position;
            float dotProduct = Vector3.Dot(Camera.main.transform.forward, directionToGoal.normalized);
            m_goalImage.gameObject.SetActive(dotProduct >= 0);

            Vector3 goalWorldPosition = m_goalObject.position + offset;
            Vector3 goalScreenPosition = Camera.main.WorldToScreenPoint(goalWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_goalImage.parent as RectTransform, goalScreenPosition, null, out Vector2 localPoint);

            localPoint.x = Mathf.Clamp(localPoint.x, padding, Screen.width - m_goalImage.rect.width - padding);
            localPoint.y = Mathf.Clamp(localPoint.y, padding, Screen.height - m_goalImage.rect.height - padding);

            m_goalImage.anchoredPosition = localPoint;
        }
    }
}
