using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyHintUI : MonoBehaviour
{
    public enum SpriteType
    {
        UNKNOWN = -1,
        XBOX = 0,
        PLAYSTATION = 1,
    }

    #region Editor API

    [SerializeField] private Image m_iconImage;
    [SerializeField] private TMP_Text m_contentText;

    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private float m_spriteSwitchingDuration = 1.0f;

    private Tween m_buttonPressedTween;

    #endregion

    #region API

    public void SetSpriteType(SpriteType spriteType)
    {
        m_spriteType = spriteType;

        if (m_spriteType == SpriteType.UNKNOWN)
        {
            m_currentShowingSpriteIndex = 0;
            __M_ShowSpritesSequentially();
        }
        else m_spriteShowingTween?.Kill();
    }

    public void PlayPressButtonEffect()
    {
        if (m_iconImage == null) return;

        m_buttonPressedTween?.Kill();

        m_buttonPressedTween = m_iconImage.transform.DOScale(0.8f, 0.08f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                m_iconImage.transform.DOScale(1.1f, 0.12f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        m_iconImage.transform.DOScale(1f, 0.1f)
                            .SetEase(Ease.OutQuad);
                    });
            });
    }

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        m_iconImage.sprite = m_sprites[0];
    }

    [UsedImplicitly]
    private void Start()
    {
        __M_ShowSpritesSequentially();
    }

    [UsedImplicitly]
    private void LateUpdate()
    {
        if (m_spriteType != SpriteType.UNKNOWN) __M_ShowSprites();
    }

    #endregion

    #region Internal

    private SpriteType m_spriteType = SpriteType.UNKNOWN;

    // SpriteType == UNKNOWN ONLY
    private Tween m_spriteShowingTween;
    private int m_currentShowingSpriteIndex;

    // Call once when status changed
    private void __M_ShowSpritesSequentially()
    {
        m_spriteShowingTween =
            DOTween.To(() => m_currentShowingSpriteIndex, x => m_currentShowingSpriteIndex = x,
                    m_sprites.Length - 1,
                    m_spriteSwitchingDuration * m_sprites.Length)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart)
                .OnUpdate(() => { m_iconImage.sprite = m_sprites[m_currentShowingSpriteIndex]; });
    }

    private void __M_ShowSprites()
    {
        m_iconImage.sprite = m_sprites[(int)m_spriteType];
    }

    #endregion
}