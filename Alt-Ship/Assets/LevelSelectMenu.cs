using System;
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
    [Header("Key Name")] [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private string m_northActionName = "North";
    [SerializeField] private string m_eastActionName = "East";
    [SerializeField] private string m_southActionName = "South";
    [SerializeField] private string m_westActionName = "West";

    [Header("Button Reference")] public Button levelOne;
    public Button levelTwo;
    public Button levelThree;

    public Button startGame;
    public Button back;

    [Header("Prompt Reference")]
    [SerializeField] private Image lvlOnePrompt;
    [SerializeField] private Image lvlOneSheep;
    [SerializeField] private Image lvlTwoPrompt;
    [SerializeField] private Image lvlTwoSheep;
    [SerializeField] private Image lvlThreePrompt;
    [SerializeField] private Image lvlThreeSheep;

    [SerializeField] private GameObject startLevelParent;

    [SerializeField] private Image startPrompt;
    [SerializeField] private Image backPrompt;

    [Header("Other")] public SceneLoader sceneLoader;

    [SerializeField] private int selectbuttonIndex = 10;
    [SerializeField] private int menuIndex = 0;


    private int buttonCount = 2;
    private int lastButNum = 0;
    private bool southToggle = false;
    private bool toggleOne = false;
    private bool toggleTwo = false;

    private bool startIsActive = false;

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
        (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest) =
            __M_FindActions();
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
        (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest) =
            __M_FindActions();
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

    private void OnWest(InputAction.CallbackContext context)
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
                    if (SelectedLevelData.Instance == null)
                    {
                        Debug.Log("Null");
                        return;
                    }

                    SwapSheep();
                    lvlOneSheep.gameObject.SetActive(true);
                    EnableStart();

                    SelectedLevelData.Instance.levelChosen(0);
                }
                // Level Two
                else if (selectbuttonIndex == 1)
                {
                    if (SelectedLevelData.Instance == null)
                    {
                        Debug.Log("Null");
                        return;
                    }

                    SwapSheep();
                    lvlTwoSheep.gameObject.SetActive(true);
                    EnableStart();

                    SelectedLevelData.Instance.levelChosen(1);
                }
                // Level Three
                else if (selectbuttonIndex == 2)
                {
                    if (SelectedLevelData.Instance == null)
                    {
                        Debug.Log("Null");
                        return;
                    }

                    SwapSheep();
                    lvlThreeSheep.gameObject.SetActive(true);
                    EnableStart();

                    SelectedLevelData.Instance.levelChosen(2);
                }
            }
            else if (menuIndex == 1)
            {
                // Start Level
                if (selectbuttonIndex == 0)
                {
                    LoadSceneBySelection();
                }
                //Back
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

    public void LoadSceneBySelection()
    {
        int targetIndex = 3 + SelectedLevelData.Instance.levelChange;
        SelectedLevelData.Instance.finishedPurpose();

        if (Enum.IsDefined(typeof(SceneName), targetIndex))
        {
            var sceneName = (SceneName)targetIndex;
            sceneLoader.SceneName = sceneName;
            Debug.Log($"Load to {sceneName}");
            sceneLoader.Load();
        }
        else
        {
            Debug.LogError($"Invalid SceneName index: {targetIndex}");
            sceneLoader.SceneName = SceneName.Undefined;
        }
    }

    public void ChooseLevel(int num)
    {
        SelectedLevelData.Instance.levelChosen(num);
    }


    private (InputAction actionNorth, InputAction actionEast, InputAction actionSouth, InputAction actionWest)
        __M_FindActions()
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
            SwapSelection();
            lvlOnePrompt.gameObject.SetActive(true);
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 1 && !toggleOne)
        {
            if (levelTwo == null) return;
            levelTwo.Select();
            SwapSelection();
            lvlTwoPrompt.gameObject.SetActive(true);
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 2 && !toggleOne)
        {
            if (levelThree == null) return;
            levelThree.Select();
            SwapSelection();
            lvlThreePrompt.gameObject.SetActive(true);
            toggleOne = !toggleOne;
        }
    }

    private void MenuButtonHighlight()
    {
        if (selectbuttonIndex == 0 && !toggleOne)
        {
            if (startGame == null) return;
            startGame.Select();
            SwapSelection();
            startPrompt.gameObject.SetActive(true);
            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 1 && !toggleOne)
        {
            if (back == null) return;
            back.Select();
            SwapSelection();
            backPrompt.gameObject.SetActive(true);
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
        else if (menuIndex == 1 && !toggleTwo)
        {
            if (horizontalMovement != 0 && !toggleTwo)
            {
                if (selectbuttonIndex == 0)
                {
                    selectbuttonIndex = 1;
                }
                else if (selectbuttonIndex != 0 && startIsActive)
                {
                    selectbuttonIndex = 0;
                }
                toggleTwo = !toggleTwo;
                toggleOne = !toggleOne;
            }
        }

        if (verticleMovement != 0 && !toggleTwo)
        {
            if (menuIndex == 0)
            {
                if (verticleMovement != 0 && startIsActive)
                {
                    // 0-1
                    menuIndex = 1;
                    lastButNum = selectbuttonIndex;
                    selectbuttonIndex = 0;
                    buttonChange();
                }
                else if (verticleMovement > tolerance || verticleMovement < -tolerance)
                {
                    // 1-1
                    menuIndex = 1;
                    lastButNum = selectbuttonIndex;
                    selectbuttonIndex = 1;
                    buttonChange();
                }
            }
            else if (menuIndex == 1)
            {
                if (selectbuttonIndex == 0)
                {
                    if (verticleMovement > tolerance || verticleMovement < -tolerance)
                    {
                        menuIndex = 0;
                        selectbuttonIndex = 2;
                        buttonChange();
                    }
                }
                else if (selectbuttonIndex == 1)
                {
                    if (verticleMovement > tolerance || verticleMovement < -tolerance)
                    {
                        menuIndex = 0;
                        selectbuttonIndex = 0;
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

    private void SwapSelection()
    {
        lvlOnePrompt.gameObject.SetActive(false);
        lvlTwoPrompt.gameObject.SetActive(false);
        lvlThreePrompt.gameObject.SetActive(false);
        startPrompt.gameObject.SetActive(false);
        backPrompt.gameObject.SetActive(false);
    }

    private void SwapSheep()
    {
        lvlOneSheep.gameObject.SetActive(false);
        lvlTwoSheep.gameObject.SetActive(false);
        lvlThreeSheep.gameObject.SetActive(false);
    }

    private void EnableStart()
    {
        if (startLevelParent.activeSelf == false)
        {
            startLevelParent.SetActive(true);
            startIsActive = true;
        }
    }

}