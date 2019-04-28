using UnityEngine;

namespace Fields
{
    public class JumpToField : Field
    {
        [SerializeField]
        private Field[] previousFields;

        [SerializeField]
        private bool backwards;
        
        public override void OnStay(Player player)
        {
            var random = backwards ? Random.Range(-4, 0) : Random.Range(1, 5);
            
            if (random < 0)
            {
                player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
                player.SetField(previousFields[random * -1 - 1]);
                
                Game.Instance.ShowWalkInfo(random * -1, "backwards");
            }
            else
            {
                Game.Instance.IncreaseRemaining(random);
                Game.Instance.ShowWalkInfo(random, "forward");
            }
            
            Game.Instance.Wait(.1f, false);
        }
    }
}