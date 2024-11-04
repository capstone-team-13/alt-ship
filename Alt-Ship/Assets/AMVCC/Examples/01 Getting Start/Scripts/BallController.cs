using JetBrains.Annotations;
using UnityEngine;
using static EE.AMVCC.Examples.BallCommand;

namespace EE.AMVCC.Examples
{
    public class BallController : Controller<BallModel, BallView>
    {
        [Header("Concrete Reference")]
        [SerializeField] private Rigidbody m_rigidbody;

        public override void Notify<TCommand>(TCommand command)
        {
            if (command is not BallCommand) return;

            switch (command)
            {
                case LogCommand logCommand:
                    Debug.Log($"@OnLogCommand {logCommand.Message}.");
                    break;
            }
        }

        [UsedImplicitly]
        private void OnCollisionEnter()
        {
            var force = new Vector3(0, 9.81f, 0);
            m_rigidbody.AddForce(force, ForceMode.Impulse);

            IncreaseBounceCount();

            Application.Instance.Push(new BounceCommand(force));
        }

        private void IncreaseBounceCount()
        {
            ++Model.BounceCount;
            View.BounceText.text = $"Bounce: {Model.BounceCount}";
        }
    }
}