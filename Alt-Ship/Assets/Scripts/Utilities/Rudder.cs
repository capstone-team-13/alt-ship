using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class Rudder : MonoBehaviour
{
    private bool m_steering;

    #region Unity Callbacks

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

    public void Interact(IInteractable interactable)
    {
        m_steering = !m_steering;
        interactable.InteractionName = m_steering ? "Stop Steering" : "Start Steer";
    }

    #endregion
}