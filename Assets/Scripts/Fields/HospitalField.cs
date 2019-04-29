namespace Fields
{
    public class HospitalField : Field
    {
        public override void OnStay(Player player)
        {
            Game.Instance.GetHospitalAudio().Play();
            
            player.Heal(4);
            
            Game.Instance.Wait(1);
        }
        
        protected override bool AllowTrapPlacement() => false;
    }
}