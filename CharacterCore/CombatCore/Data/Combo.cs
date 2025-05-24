using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Create Combo", fileName = "Combo", order = 0)]
    public class Combo : ScriptableObject
    {
        public Attack[] AttackSet;
    }
}