using NodeCanvas.Framework;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    public class SelectTargetPosition : ActionTask
    {
        public BBParameter<Vector3> TargetPosition;

        protected override string OnInit()
        {
            return null;
        }

        protected override void OnExecute()
        {
            var newPosition = agent.transform.position;

            const int range = 3;
            newPosition.x = Random.Range(newPosition.x - range, newPosition.x + range);
            newPosition.z = Random.Range(newPosition.z - range, newPosition.z + range);

            TargetPosition.value = newPosition;
            EndAction(true);
        }
    }
}