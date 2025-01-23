using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Update()
    {
        // 检测 T 键是否在当前帧被按下
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("T key pressed!");
            Panel.SetActive(!Panel.activeSelf); // 切换教程面板的显示状态
        }
    }
}
