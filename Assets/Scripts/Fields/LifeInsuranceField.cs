namespace Fields
{
    public class LifeInsuranceField : Field
    {
        public override void OnStay(Player player)
        {
            Game.Instance.GetCamera().Zoom();
            Game.Instance.ShowLifeInsuranceDialog();
            
            Game.Instance.Wait(float.MaxValue);
        }

        public override bool AllowTrapPlacement() => false;
    }
}