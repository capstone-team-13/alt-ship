using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class Rudder : MonoBehaviour
{
    [Header("Player Interaction")] [SerializeField]
    private float m_interactionRadius = 5.0f;

    [SerializeField] private LayerMask m_playerLayer;
    [SerializeField] private Interactable m_interactable;

    private bool m_steering;

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        m_interactable.CanInteract = __M_CanInteract;
    }

    [UsedImplicitly]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_interactionRadius);
    }

    [UsedImplicitly]
    private void Update()
    {
        if (!m_steering) return;

        // TODO: Refactor using new input system
        var rotationSign = 0.0f;
        if (Input.GetKey(KeyCode.J)) rotationSign = -1;
        else if (Input.GetKey(KeyCode.K)) rotationSign = 1;

        if (rotationSign != 0)
        {
            Debug.Log("Steering");
            Application.Instance.Push(new ShipCommand.Steer(rotationSign));
        }
    }

    #endregion


    #region API

    public void Interact()
    {
        m_steering = true;
        Debug.Log("Start Steering...");
    }

    #endregion

    #region Internal

    private bool __M_CanInteract()
    {
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_playerLayer);
        // TODO: Add condition that player behind the cannon

        // TODO: Better State Management
        if (m_steering && colliders.Length <= 0) m_steering = false;

        return !m_steering && colliders.Length > 0;
    }

    #endregion
}