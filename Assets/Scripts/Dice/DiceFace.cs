using UnityEngine;

namespace Dice
{
    public class DiceFace : MonoBehaviour
    {
        [SerializeField]
        private int eyes;

        public int GetEyes() => eyes;
    }
}