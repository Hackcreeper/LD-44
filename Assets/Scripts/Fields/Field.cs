using UnityEngine;

namespace Fields
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        private Field nextField;

        public Field GetNext() => nextField;

        public virtual void OnEnter()
        {
            Debug.Log("Entered field");
        }
    }
}
