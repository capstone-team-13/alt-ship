using Accord.Math;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Key Name")]
    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private string m_northActionName = "North";
    [SerializeField] private string m_eastActionName = "East";
    [SerializeField] private string m_southActionName = "South";
    [SerializeField] private string m_westActionName = "West";

    [Header("Button Reference")]
    public Button levelOne;
    public Button levelTwo;
    public Button levelThree;

    public Button startGame;
    public Button back;

    [Header("Other")]
    public SceneLoader sceneLoader;

    [SerializeField] private int selectbuttonIndex = 10;
    [SerializeField] private int menuIndex = 0;


    private int buttonCount = 2;
    private int lastButNum = 0;
    private bool southToggle = false;
    private bool toggleOne = false;
    private bool toggleTwo = false;

    private float tolerance = .7f;


    private void OnEnable()
    {
        EnableHandlers();
    }

    private void OnDisable()
    {
        DisableHandlers();
    }

    private void Update()
    {

        InputDetection();

        if (menuIndex == 0)
        {
            LevelButtonHighlight();
        }

        if (menuIndex == 1)
        {
            MenuButtonHighlight();
        }

    }

    private void EnableHandlers()
    {
        (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest) = __M_FindActions();
        if (actionNorth != null)
        {
            actionNorth.performed += OnNorth;
            actionNorth.canceled += OnNorth;
            actionNorth.Enable();
        }

        if (actionEast != null)
        {
            actionEast.performed += OnEast;
            actionEast.canceled += OnEast;
            actionEast.Enable();
        }

        if (actionSouth != null)
        {
            actionSouth.performed += OnSouth;
            actionSouth.canceled += OnSouth;
            actionSouth.Enable();
        }

        if (actionWest != null)
        {
            actionWest.performed += OnWest;
            actionWest.canceled += OnWest;
            actionWest.Enable();
        }
    }

    private void DisableHandlers()
    {
        (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest) = __M_FindActions();
        if (actionNorth != null)
        {
            actionNorth.performed -= OnNorth;
            actionNorth.canceled -= OnNorth;
            actionNorth.Disable();
        }

        if (actionEast != null)
        {
            actionEast.performed -= OnEast;
            actionEast.canceled -= OnEast;
            actionEast.Disable();
        }

        if (actionSouth != null)
        {
            actionSouth.performed -= OnSouth;
            actionSouth.canceled -= OnSouth;
            actionSouth.Disable();
        }

        if (actionWest != null)
        {
            actionWest.performed -= OnWest;
            actionWest.canceled -= OnWest;
            actionWest.Disable();
        }
    }

    private void OnNorth(InputAction.CallbackContext context)
    {

    }

    private void OnEast(InputAction.CallbackContext context)
    {

    }

    private void OnSouth(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0 && !southToggle)
        {
            if (menuIndex == 0)
            {
                // Level One
                if (selectbuttonIndex == 0)
                {
                    
                    if (SelectedLevelData.Instance == null) return;
                    SelectedLevelData.Instance.levelChosen(0);
                }
                // Level Two
                else if (selectbuttonIndex == 1)
                {
                    if (SelectedLevelData.Instance == null) return;
                    SelectedLevelData.Instance.levelChosen(1);
                }
                // Level Three
                else if (selectbuttonIndex == 2)
                {
                    if (SelectedLevelData.Instance == null) return;
                    SelectedLevelData.Instance.levelChosen(2);
                }
            }
            else if (menuIndex == 1)
            {
                // Start Level
                if (selectbuttonIndex == 0)
                {
                    sceneLoader.Load();
                }
                // Back
                else if (selectbuttonIndex == 1)
                {
                    sceneLoader.MainMenu();
                }
            }
            southToggle = !southToggle;
        }

        if (context.ReadValue<float>() == 0 && southToggle)
        {
            southToggle = !southToggle;
        }
    }

    private void OnWest(InputAction.CallbackContext context)
    {

    }

    private (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest) __M_FindActions()
    {
        InputAction actionNorth = m_inputActionAsset.FindAction(m_northActionName);
        InputAction actionEast = m_inputActionAsset.FindAction(m_eastActionName);
        InputAction actionSouth = m_inputActionAsset.FindAction(m_southActionName);
        InputAction actionWest = m_inputActionAsset.FindAction(m_westActionName);


        if (actionNorth == null)
        {
            throw new System.Exception($"Join action '{m_northActionName}' not found in InputActionAsset.");
        }

        if (actionEast == null)
        {
            throw new System.Exception($"Exit action '{m_eastActionName}' not found in InputActionAsset.");
        }

        if (actionSouth == null)
        {
            throw new System.Exception($"Exit action '{m_southActionName}' not found in InputActionAsset.");
        }

        if (actionEast == null)
        {
            throw new System.Exception($"Exit action '{m_westActionName}' not found in InputActionAsset.");
        }

        return (actionNorth, actionEast, actionSouth, actionWest);
    }

    private void LevelButtonHighlight()
    {
        if (selectbuttonIndex == 0 && !toggleOne)
        {
            if (levelOne == null) return;
            levelOne.Select();
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 1 && !toggleOne)
        {
            if (levelTwo == null) return;
            levelTwo.Select();
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 2 && !toggleOne)
        {
            if (levelThree == null) return;
            levelThree.Select();
            toggleOne = !toggleOne;
        }
    }

    private void MenuButtonHighlight()
    {
        if (selectbuttonIndex == 0 && !toggleOne)
        {
            if (startGame == null) return;
            startGame.Select();
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 1 && !toggleOne)
        {
            if (back == null) return;
            back.Select();
            toggleOne = !toggleOne;
        }
    }

    private void InputDetection()
    {
        float verticleMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        if (verticleMovement != 0 && selectbuttonIndex == 10 || horizontalMovement != 0 && selectbuttonIndex == 10)
        {
            selectbuttonIndex = 0;
            toggleTwo = !toggleTwo;
        }

        if (menuIndex == 0 && !toggleTwo)
        {
            if (horizontalMovement != 0 && !toggleTwo)
            {
                if (horizontalMovement < -tolerance)
                {
                    if (selectbuttonIndex > 0)
                    {
                        selectbuttonIndex--;
                    }
                    else
                    {
                        selectbuttonIndex = buttonCount;
                    }
                    toggleTwo = !toggleTwo;
                    toggleOne = !toggleOne;
                }
                else if (horizontalMovement > tolerance)
                {
                    if (selectbuttonIndex < buttonCount)
                    {
                        selectbuttonIndex++;
                    }
                    else if (selectbuttonIndex == buttonCount)
                    {
                        selectbuttonIndex = 0;
                    }
                    toggleTwo = !toggleTwo;
                    toggleOne = !toggleOne;
                }
            }
        }

        if (verticleMovement != 0 && !toggleTwo)
        {
            if (menuIndex == 0)
            {
                if (verticleMovement < -tolerance)
                {
                    // 0-1
                    menuIndex = 1;
                    lastButNum = selectbuttonIndex;
                    selectbuttonIndex = 0;
                    buttonChange();
                }
                else if(verticleMovement > tolerance)
                {
                    // 1-1
                    menuIndex = 1;
                    lastButNum = selectbuttonIndex;
                    selectbuttonIndex = 1;
                    buttonChange();
                }
            }
            else if(menuIndex == 1)
            {
                if (selectbuttonIndex == 0)
                {
                    if(verticleMovement > tolerance)
                    {
                        menuIndex = 0;
                        selectbuttonIndex = lastButNum;
                        buttonChange();
                    }
                    else if (verticleMovement < -tolerance)
                    {
                        selectbuttonIndex = 1;
                        buttonChange();
                    }
                }
                else if (selectbuttonIndex == 1)
                {
                    if (verticleMovement > tolerance)
                    {
                        selectbuttonIndex = 0;
                        buttonChange();
                    }
                    else if (verticleMovement < -tolerance)
                    {
                        menuIndex = 0;
                        selectbuttonIndex = lastButNum;
                        buttonChange();
                    }
                }
            }
        }

        if (horizontalMovement == 0 && verticleMovement == 0 && toggleTwo)
        {
            toggleTwo = !toggleTwo;
        }

    }

    private void buttonChange()
    {
        toggleTwo = !toggleTwo;
        toggleOne = false;
    }



}
