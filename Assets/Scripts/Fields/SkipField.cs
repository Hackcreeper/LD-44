namespace Fields
{
    public class SkipField : Field
    {
        public override void OnStay(Player player)
        {
            player.SkipNextTurn(); 
        }

        public override bool AllowTrapPlacement() => false;
    }
}