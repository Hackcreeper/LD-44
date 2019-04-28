using UnityEngine;

namespace Fields
{
    public class JumpShortcutField : Field, IShortcutField
    {
        [SerializeField]
        private Field target;
        
        public override void OnStay(Player player)
        {
            Game.Instance.GetCamera().Zoom();
            Game.Instance.ShowShortcutDialog(3, player, this);
            
            Game.Instance.Wait(float.MaxValue);
        }

        public void Accepted(Player player)
        {
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();
            
            player.Hurt(6);
            
            player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
            player.SetField(target);            
        }

        public void Canceled(Player player)
        {
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();
            Game.Instance.Wait(.1f);
        }
    }
} 