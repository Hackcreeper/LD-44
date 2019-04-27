namespace Fields
{
    public class WinField : Field
    {
        public override void OnEnter()
        {
            Game.Instance.Win();
        }
    }
}