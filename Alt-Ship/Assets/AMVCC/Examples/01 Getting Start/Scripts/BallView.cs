using JetBrains.Annotations;
using Events = EE.AMVCC.Examples.BounceBallApplication.Events;

namespace EE.AMVCC.Examples
{
    public class BallView : BounceElement
    {
        public void ScaleDown()
        {
            transform.localScale *= 0.9f;
        }

        [UsedImplicitly]
        private void OnCollisionEnter()
        {
            App.Notify(Events.Ball.HitGround, gameObject, null);
        }
    }
}
