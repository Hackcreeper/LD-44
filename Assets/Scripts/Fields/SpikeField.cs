using UnityEngine;

namespace Fields
{
    public class SpikeField : Field
    {
        [SerializeField]
        private Animator animator;
        
        public override void OnStay(Player player)
        {
            Game.Instance.GetSpikeAudio().Play();
            
            animator.Play("Active");
            
            player.Hurt(2);
            
            Game.Instance.Wait(1);
        }
        
        protected override bool AllowTrapPlacement() => false;
    }
}