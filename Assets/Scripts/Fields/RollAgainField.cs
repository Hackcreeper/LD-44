namespace Fields
{
    public class RollAgainField : Field
    {
        public override void OnStay(Player player)
        {
            Game.Instance.ReRunCurrentPlayer();
            Game.Instance.ShowRollAgainInfo();
            
            Game.Instance.Wait(2f);
        }

        public override bool AllowTrapPlacement() => false;
    }
}