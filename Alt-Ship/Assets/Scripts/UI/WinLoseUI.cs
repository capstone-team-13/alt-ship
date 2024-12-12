using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] private Image m_image;
    [SerializeField] private Sprite[] m_sprites;

    [UsedImplicitly]
    private void Awake()
    {
        // 0 - lose, 1 - win
        if (!PlayerPrefs.HasKey("Game Result")) return;
        var result = PlayerPrefs.GetInt("Game Result");
        m_image.sprite = m_sprites[result];
    }
}