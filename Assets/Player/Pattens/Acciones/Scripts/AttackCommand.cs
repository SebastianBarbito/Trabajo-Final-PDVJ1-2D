public class AttackCommand : ICommand
{
    public void Execute(Player_Movement player)
    {
        player.Atacando();
    }
}

