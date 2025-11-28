public class DefendCommand : ICommand
{
    public void Execute(Player_Movement player)
    {
        player.Defensa();
    }
}