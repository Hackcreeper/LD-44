namespace Fields
{
    public class SpikeField : Field
    {
        public override void OnStay(Player player)
        {
            // TODO: Animation
            
            player.Hurt(2);
        }
    }
}