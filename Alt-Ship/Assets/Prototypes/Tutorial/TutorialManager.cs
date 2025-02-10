using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    // 整个教程面板
    public GameObject tutorialPanel;

    // 五页教程
    public List<GameObject> tutorialPages;

    private int currentPageIndex = 0;

    void Start()
    {
        // 一开始先把教程面板关掉
        tutorialPanel.SetActive(false);
    }

    void Update()
    {
        // 检测 ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 如果教程面板此时是激活的，那就关掉；否则就什么都不做
            if (tutorialPanel.activeSelf)
            {
                CloseTutorial();
            }
        }
    }

    // 按下“打开教程”按钮时调用这个方法
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
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
    }

    // 用于 ESC 或者“关闭”按钮退出
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    // 显示当前页面，其它页面隐藏
    private void ShowPage(int index)
    {
        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == index);
        }
    }
}
