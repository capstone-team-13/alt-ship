using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControlManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private string m_northActionName = "North";
    [SerializeField] private string m_eastActionName = "East";
    [SerializeField] private string m_southActionName = "South";
    [SerializeField] private string m_westActionName = "West";

    public SceneLoader sceneLoader;
    public TutorialManager tutorialManager;
    public GameObject creditsPage;

    [Header("Button Reference")]
    public Button start;
    public Button exit;
    public Button credits;
    public Button tutorial;
    public Button tutNext;
    public Button tutPrev;
    public Image startPrompt;
    public Image tutPrompt;
    public Image creditsPrompt;
    public Image exitPrompt;

    [SerializeField] private int selectbuttonIndex = 10;

    private int menuIndex = 0;

    private int buttonCount = 3;

    private bool toggleOne = false;
    private bool toggleTwo = false;
    private bool southToggle = false;

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
            MenuButtonHighlight();
        }

        if(menuIndex == 1)
        {
            TutorialButtonHighlight();
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


    // Button Functions
    private void OnNorth(InputAction.CallbackContext context)
    {

    }

    private void OnEast(InputAction.CallbackContext context)
    {
        if (menuIndex == 1)
        {
            tutorialManager.CloseTutorial();
            menuIndex = 0;
            selectbuttonIndex = 1;
            toggleOne = false;
            toggleTwo = false;
        }
        else if (menuIndex == 2) 
        {
            creditsPage.SetActive(false);
            menuIndex = 0;
            selectbuttonIndex = 2;
        }
    }

    private void OnSouth(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0 && !southToggle)
        {
            if (menuIndex == 0)
            {
                if (selectbuttonIndex == 0)
                {
                    sceneLoader.Load();
                }
                else if (selectbuttonIndex == 1)
                {
                    tutorialManager.OpenTutorial();
                    menuIndex = 1;
                    selectbuttonIndex = 1;
                    toggleOne = false;
                }
                else if (selectbuttonIndex == 2)
                {
                    if(creditsPage != null)
                    {
                        creditsPage.SetActive(true);
                        menuIndex = 2;
                    }
                }
                else if (selectbuttonIndex == 3)
                {
                    Application.Quit();
                }
            }
            else if (menuIndex == 1)
            {
                if (selectbuttonIndex == 0)
                {
                    tutorialManager.PrevPage();
                }
                else if (selectbuttonIndex == 1)
                {
                    tutorialManager.NextPage();
                }
            }
            southToggle = !southToggle;
        }

        if(context.ReadValue<float>() == 0 && southToggle)
        {
            southToggle = !southToggle;
        }

    }

    private void OnWest(InputAction.CallbackContext context)
    {
        Debug.Log("West");
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

    private void MenuButtonHighlight()
    {
        if (selectbuttonIndex == 0 && !toggleOne)
        {
            if (start == null) return;
            start.Select();

            SwapPrompt();
            startPrompt.gameObject.SetActive(true);

            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 1 && !toggleOne)
        {
            if (tutorial == null) return;
            tutorial.Select();

            SwapPrompt();
            tutPrompt.gameObject.SetActive(true);

            toggleOne = !toggleOne;
        }
        else if (selectbuttonIndex == 2 && !toggleOne)
        {
            if(credits == null) return;
            credits.Select();

            SwapPrompt();
            creditsPrompt.gameObject.SetActive(true);

            toggleOne= !toggleOne;
        }
        else if (selectbuttonIndex == 3 && !toggleOne)
        {
            if (exit == null) return;
            exit.Select();

            SwapPrompt();
            exitPrompt.gameObject.SetActive(true);

            toggleOne = !toggleOne;
        }
    }

    private void TutorialButtonHighlight()
    {
        if(selectbuttonIndex == 0 && !toggleOne)
        {
            if (tutPrev == null) return;
            tutPrev.Select();
            toggleOne = !toggleOne;
        }
        else if(selectbuttonIndex == 1 && !toggleOne)
        {
            if (tutNext == null) return;
            tutNext.Select();
            toggleOne = !toggleOne;
        }
    }

    private void InputDetection()
    {
        float verticleMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        Vector2 dpad = Gamepad.current.dpad.ReadValue();

        float dpadX = dpad.x;
        float dpadY = dpad.y;

        if (menuIndex == 0)
        {

            if (verticleMovement != 0 && !toggleTwo || dpadY != 0 && !toggleTwo)
            {
                if (verticleMovement != 0 && selectbuttonIndex == 10 || dpadY != 0 && selectbuttonIndex == 10)
                {
                    selectbuttonIndex = 0;
                    toggleTwo = !toggleTwo;
                }
                else if (verticleMovement > 0 || dpadY > 0)
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
                else if (verticleMovement < 0 || dpadY < 0)
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

            else if (verticleMovement == 0 && dpadY == 0 && toggleTwo)
            {
                toggleTwo = !toggleTwo;
            }
        }

        if (menuIndex == 1)
        {
            if (tutorialManager.currentPageIndex != 0 && tutorialManager.currentPageIndex != tutorialManager.tutorialPages.Count - 1)
            {
                if (horizontalMovement != 0 && !toggleTwo || dpadX != 0 && !toggleTwo)
                {
                    if (selectbuttonIndex == 1)
                    {
                        selectbuttonIndex = 0;
                    }
                    else if (selectbuttonIndex == 0)
                    {
                        selectbuttonIndex = 1;
                    }
                    toggleTwo = !toggleTwo;
                    toggleOne = !toggleOne;
                }
                else if (horizontalMovement == 0 && toggleTwo && dpadX == 0 && toggleTwo)
                {
                    toggleTwo = !toggleTwo;
                }
            }
            else if(tutorialManager.currentPageIndex == 0 && selectbuttonIndex != 1)
            {
                selectbuttonIndex = 1;
                toggleOne = false;
            }
            else if (tutorialManager.currentPageIndex == tutorialManager.tutorialPages.Count - 1 && selectbuttonIndex != 0)
            {
                selectbuttonIndex = 0;
                toggleOne = false;
            }
        }

    }

    public void OpenCreditButton()
    {
        if (creditsPage != null)
        {
            creditsPage.SetActive(true);
        }
    }

    public void CloseCreditButton()
    {
        if (creditsPage != null)
        {
            creditsPage.SetActive(false);
        }
    }

    private void SwapPrompt()
    {
        startPrompt.gameObject.SetActive(false);
        tutPrompt.gameObject.SetActive(false);
        creditsPrompt.gameObject.SetActive(false);
        exitPrompt.gameObject.SetActive(false);
    }

}
