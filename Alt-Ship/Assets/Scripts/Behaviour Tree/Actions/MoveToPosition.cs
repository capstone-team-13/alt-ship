using NodeCanvas.Framework;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    public class MoveToPosition : ActionTask
    {
        public BBParameter<Vector3> TargetPosition;
        public BBParameter<SheepModel> Sheep;

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit()
        {
            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            if (TargetPosition.value == Vector3.zero || __M_ReachGoal())
                EndAction(false);
        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
            // TODO: Refactor using Navmesh
            // current headwind is the Navmesh setting for moving platform
            var direction = (TargetPosition.value - agent.transform.position).normalized;
            agent.transform.Translate(direction * Sheep.value.Speed * Time.deltaTime);


            if (__M_ReachGoal()) EndAction(true);
        }

        private bool __M_ReachGoal()
        {
            const float threshold = 0.1f;
            return (TargetPosition.value - agent.transform.position).magnitude < threshold;
        }
    }
}