using UnityEngine;

namespace EE.Levels
{
    // A rule process game states provided by level manager
    // It runs parallel and no conflict with others
    public abstract class Setting : MonoBehaviour
    {
        public void Execute(LevelManager states)
        {
            OnExecute(states);
        }

        protected virtual void OnExecute(LevelManager states)
        {
            
        }
    }
}
