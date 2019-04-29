using UnityEngine;

namespace Fields
{
    public class JumpShortcutField : Field, IShortcutField
    {
        [SerializeField]
        private Field target;

        public override void OnEnter(Player player)
        {
            Game.Instance.GetCamera().Zoom();
            Game.Instance.ShowShortcutDialog(GetPrice() / 2, player, this);
            
            target.SpawnArrows();
            
            Game.Instance.Wait(float.MaxValue);
        }

        public void Accepted(Player player)
        {
            target.ClearArrows();
            
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            
            player.Hurt(GetPrice());
            if (player.GetHealth() <= 0)
            {
                Game.Instance.Wait(.1f, true);
                return;
            }
            
            TowerAnimation.Instance.StartAnimation(player, target);
        }

        public void Canceled(Player player)
        {
            target.ClearArrows();
            
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();

            player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
            player.SetField(player.GetField().GetNext());
        }

        public int GetPrice() => 6;
        
        public override bool AllowTrapPlacement() => false;
    }
} 