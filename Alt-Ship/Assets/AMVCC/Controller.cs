using UnityEngine;

namespace EE.AMVCC
{
    public interface IController
    {
        void OnNotified(string eventType, Object sender, params object[] data);
    }
}
