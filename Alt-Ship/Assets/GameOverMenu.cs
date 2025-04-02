using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private string m_northActionName = "North";
    [SerializeField] private string m_southActionName = "South";

    public SceneLoader sceneLoader;
    private bool southToggle = false;


    private void OnEnable()
    {
        EnableHandlers();
    }

    private void EnableHandlers()
    {
        (InputAction actionNorth, InputAction actionSouth) = __M_FindActions();

        if (actionNorth != null)
        {
            actionNorth.performed += OnNorth;
            actionNorth.canceled += OnNorth;
            actionNorth.Enable();
        }

        if (actionSouth != null)
        {
            actionSouth.performed += OnSouth;
            actionSouth.canceled += OnSouth;
            actionSouth.Enable();
        }
    }

    private void DisableHandlers()
    {
        (InputAction actionNorth, InputAction actionSouth) = __M_FindActions();
        if (actionNorth != null)
        {
            actionNorth.performed -= OnNorth;
            actionNorth.canceled -= OnNorth;
            actionNorth.Disable();
        }

        if (actionSouth != null)
        {
            actionSouth.performed -= OnSouth;
            actionSouth.canceled -= OnSouth;
            actionSouth.Disable();
        }
    }

    private void OnNorth(InputAction.CallbackContext context)
    {

    }

    private void OnSouth(InputAction.CallbackContext context)
    {
        if (southToggle) return;

        sceneLoader.Load();
        southToggle = true;

    }

    private (InputAction actionNorth, InputAction actionSouth) __M_FindActions()
    {
        InputAction actionNorth = m_inputActionAsset.FindAction(m_northActionName);
        InputAction actionSouth = m_inputActionAsset.FindAction(m_southActionName);

        if (actionNorth == null)
        {
            throw new System.Exception($"Join action '{m_northActionName}' not found in InputActionAsset.");
        }

        if (actionSouth == null)
        {
            throw new System.Exception($"Exit action '{m_southActionName}' not found in InputActionAsset.");
        }

        return (actionNorth, actionSouth);
    }

}
