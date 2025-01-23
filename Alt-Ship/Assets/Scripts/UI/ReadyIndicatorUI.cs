using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ReadyIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Image m_readyImage;

    private Tween m_colorSwitchTween;
    private Tween m_readyImageTween;

    public void SetColor(Color targetColor, float duration)
    {
        m_colorSwitchTween?.Kill();
        m_colorSwitchTween = m_backgroundImage
            .DOColor(targetColor, duration)
            .SetEase(Ease.InOutQuart);
    }

    public void SetReadyImageActive(bool show)
    {
        var popupScale = new Vector3(1.2f, 1.2f, 1f);

        m_readyImageTween?.Kill();

        if (show)
        {
            m_readyImage.transform.localScale = Vector3.zero;

            Sequence popUpSequence = DOTween.Sequence();

            popUpSequence.Append(
                    m_readyImage.transform
                        .DOScale(popupScale, 0.5f)
                        .SetEase(Ease.OutBack)
                )
                .Append(
                    m_readyImage.transform
                        .DOScale(Vector3.one, 0.2f)
                        .SetEase(Ease.OutSine)
                );
        }
        else
        {
            m_readyImage.transform
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack);
        }
    }
}