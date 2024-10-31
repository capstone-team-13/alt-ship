using UnityEngine;
using static EE.AMVCC.Examples.BounceBallApplication.Events;
using Object = UnityEngine.Object;

namespace EE.AMVCC.Examples
{
    public class BounceController : BounceElement, IController
    {
        public void OnNotified(string eventType, Object sender, params object[] data)
        {
            var model = App.Model;
            if (model.GameEnd) return;

            switch (eventType)
            {
                case Game.End:
                    model.GameEnd = true;
                    Debug.Log("Game End");
                    break;
                case Ball.HitGround:
                    ++model.Bounces;
                    model.Ball.velocity = new Vector3(0, 9.81f, 0);
                    App.View.Ball.ScaleDown();
                    Debug.Log($"{sender.name} bounced.");

                    if (model.Bounces >= model.BounceToWin)
                        App.Notify(Game.End, gameObject, null);
                    break;

            }
        }
    }
}