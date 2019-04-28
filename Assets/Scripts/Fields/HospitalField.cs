namespace Fields
{
    public class HospitalField : Field
    {
        public override void OnStay(Player player)
        {
            player.Heal(4);
        }
    }
}