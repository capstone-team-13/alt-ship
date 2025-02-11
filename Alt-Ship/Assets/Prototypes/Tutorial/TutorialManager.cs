using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    // 整个教程面板
    public GameObject tutorialPanel;

    // 五页教程
    public List<GameObject> tutorialPages;

    public int currentPageIndex = 0;

    public Button prevButton;
    public Button nextButton;

    void Start()
    {
        // 一开始先把教程面板关掉
        tutorialPanel.SetActive(false);
    }

    void Update()
    {
        // 按T键打开教程
        if (Input.GetKeyDown(KeyCode.T))
        {
            // 如果教程当前是关闭的，那么就打开
            if (!tutorialPanel.activeSelf)
            {
                OpenTutorial();
            }
        }

        // 按ESC键退出教程
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tutorialPanel.activeSelf)
            {
                CloseTutorial();
            }
        }
    }

    // 按下“打开教程”按钮或者按下T键时，也调用这个方法
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        currentPageIndex = 0;
        prevButton.gameObject.SetActive(false);
        ShowPage(currentPageIndex);
        checkButtons();
    }

    // “下一页”按钮
    public void NextPage()
    {
        currentPageIndex++;
        if (currentPageIndex >= tutorialPages.Count)
        {
            currentPageIndex = tutorialPages.Count - 1;
        }
        ShowPage(currentPageIndex);
        checkButtons();
    }

    // “上一页”按钮
    public void PrevPage()
    {
        currentPageIndex--;
        if (currentPageIndex < 0)
        {
            currentPageIndex = 0;
        }
        ShowPage(currentPageIndex);
        checkButtons();
    }

    // ESC或者“关闭”按钮退出
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    // 显示指定页，隐藏其他页
    private void ShowPage(int index)
    {
        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == index);
        }
    }

    private void checkButtons()
    {
        if (!prevButton.gameObject.activeSelf && currentPageIndex != 0)
        {
            prevButton.gameObject.SetActive(true);
        }
        else if (prevButton.gameObject.activeSelf && currentPageIndex == 0)
        {
            prevButton.gameObject.SetActive(false);
        }

        if (!nextButton.gameObject.activeSelf && currentPageIndex != tutorialPages.Count -1)
        {
            nextButton.gameObject.SetActive(true);
        }
        else if (nextButton.gameObject.activeSelf && currentPageIndex == tutorialPages.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
    }
}
