namespace Fields
{
    public class WinField : Field
    {
        public override void OnEnter(Player player)
        {
            Game.Instance.Win(player);
        }
        
        protected override bool AllowTrapPlacement() => false;
    }
}