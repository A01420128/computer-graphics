// FINAL PROJECT
//
// Javier Flores
// Enrique Orduna
// Jose Tlacuilo
//

public class PlayerInfo
{
    public string name;

    public static PlayerInfo[] players;

    public PlayerInfo(string name)
    {
        this.name = name;
    }

    public static void SetPlayers(string first, string second, string third)
    {
        PlayerInfo firstP = new PlayerInfo(first);
        PlayerInfo secondP = new PlayerInfo(second);
        PlayerInfo thirdP = new PlayerInfo(third);

        PlayerInfo.players = new PlayerInfo[] { firstP, secondP, thirdP };
    }
}
