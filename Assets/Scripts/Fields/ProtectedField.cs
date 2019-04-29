namespace Fields
{
    public class ProtectedField : Field
    {
        protected override bool AllowTrapPlacement() => false;
    }
}