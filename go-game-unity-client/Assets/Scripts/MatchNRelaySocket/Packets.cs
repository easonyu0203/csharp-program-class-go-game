
public class SocketIOEventNames
{
    public static string PlayerData = "PlayerData";
    public static string RequestMatch = "requestMatch";
    public static string RelayData = "relayData";
    public static string Ticket = "ticket";
}

public class PlayerDataPck
{
    public string id { get; private set; }

    public PlayerDataPck(string id)
    {
        this.id = id;
    }
}

public class RequestMatchPck
{
}

public class TicketPck
{
    public string P2PConnectMethod { get; private set; }

    public TicketPck(string p2PConnectMethod)
    {
        P2PConnectMethod = p2PConnectMethod;
    }
}