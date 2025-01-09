using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    // 显示教程面板
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    // 隐藏教程面板
    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
