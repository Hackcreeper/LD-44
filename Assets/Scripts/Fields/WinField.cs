namespace Fields
{
    public class WinField : Field
    {
        public override void OnEnter(Player player)
        {
            Game.Instance.Win();
        }
    }
}