using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public List<GameObject> tutorialPages;

    public int currentPageIndex = 0;

    public Button prevButton;
    public Button nextButton;

    void Start()
    {
        // 一开始先把教程面板关掉
        tutorialPanel.SetActive(false);

        // 绑定按钮事件
        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);

        // 初始化按钮状态
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(tutorialPages.Count > 1); // 只有超过一页时才显示
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!tutorialPanel.activeSelf)
            {
                OpenTutorial();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tutorialPanel.activeSelf)
            {
                CloseTutorial();
            }
        }
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
        CheckButtons();
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialPages.Count - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
            CheckButtons();
        }
    }

    public void PrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
            CheckButtons();
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == index);
        }
    }

    private void CheckButtons()
    {
        prevButton.gameObject.SetActive(currentPageIndex > 0);
        nextButton.gameObject.SetActive(currentPageIndex < tutorialPages.Count - 1);
    }
}
