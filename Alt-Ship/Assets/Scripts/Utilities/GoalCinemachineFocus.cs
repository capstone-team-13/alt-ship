using Cinemachine;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Application = EE.AMVCC.Application;

public class GoalCinemachineFocus : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_camera;
    [SerializeField] private CinemachineVirtualCamera[] m_stops;
    [SerializeField] private float m_minMoveDuration = 1.25f;
    [SerializeField] private float m_maxMoveDuration = 5.0f;
    [SerializeField] private float m_startDelay = 1f;

    public UnityEvent<string> StopStarted;

    [UsedImplicitly]
    private void Start()
    {
        m_camera.Priority = 100;
        MoveCameraToTarget();
    }

    void MoveCameraToTarget()
    {
        var camTransform = m_camera.transform;
        var cameraSequence = DOTween.Sequence();

        cameraSequence.AppendInterval(m_startDelay);

        foreach (var virtualCamera in m_stops)
        {
            var distance = Vector3.Distance(camTransform.position, virtualCamera.transform.position);
            var dynamicDuration = Mathf.Min(Mathf.Max(m_minMoveDuration, distance / 50f), m_maxMoveDuration);

            Tween moveTween = camTransform.DOMove(virtualCamera.transform.position, dynamicDuration)
                .OnStart(() => { StopStarted.Invoke(virtualCamera.gameObject.name); })
                .SetEase(Ease.InOutQuad);

            Tween fovTween = DOTween.To(() => m_camera.m_Lens.FieldOfView,
                    x => m_camera.m_Lens.FieldOfView = x,
                    virtualCamera.m_Lens.FieldOfView,
                    dynamicDuration)
                .SetEase(Ease.InOutQuad);

            if (virtualCamera.LookAt != null)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(virtualCamera.LookAt.position - camTransform.position);

                Tween lookTween = camTransform.DORotateQuaternion(targetRotation, dynamicDuration)
                    .SetEase(Ease.InOutQuad);

                cameraSequence.Append(moveTween).Join(fovTween).Join(lookTween);
            }
            else cameraSequence.Append(moveTween).Join(fovTween);
        }

        cameraSequence.OnComplete(() =>
        {
            m_camera.Priority = -1;
            Application.Instance.Push(new GameCommand.GameStart(Time.time));
        });
    }
}