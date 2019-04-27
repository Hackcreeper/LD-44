using UnityEngine;

namespace Fields
{
    public class WinField : Field
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log("WIN!");
        }
    }
}