using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject Panel;

    // 显示教程面板
    public void ShowTutorial()
    {
        Panel.SetActive(true);
    }

    // 隐藏教程面板
    public void HideTutorial()
    {
        Panel.SetActive(false);
    }
}
