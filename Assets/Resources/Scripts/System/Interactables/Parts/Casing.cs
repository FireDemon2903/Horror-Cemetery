public class Casing : Collectable, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(PlayerControler.Parts.Casing);
    }
}
