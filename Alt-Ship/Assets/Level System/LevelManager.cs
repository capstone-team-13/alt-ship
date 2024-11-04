using System.Collections.Generic;
using UnityEngine;

namespace EE.Levels
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Dictionary<string, object> m_facts;
        [SerializeField] private Setting[] m_rules;

        public void Update()
        {

        }
    }
}
