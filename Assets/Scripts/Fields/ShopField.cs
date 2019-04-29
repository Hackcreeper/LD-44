namespace Fields
{
    public class ShopField : Field
    {
        public override void OnEnter(Player player)
        {
            Game.Instance.GetCamera().Zoom();
            Game.Instance.ShowShopDialog();
            
            Game.Instance.Wait(float.MaxValue);
        }
        
        protected override bool AllowTrapPlacement() => false;
    }
}