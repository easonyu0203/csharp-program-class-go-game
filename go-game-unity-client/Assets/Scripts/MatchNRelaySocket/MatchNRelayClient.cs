using System;
using System.Collections;
using System.Threading.Tasks;
using dotenv.net;
using SocketIOClient;
using UnityEngine;


public interface IMatchNRelayClient
{
    event Action ConnectedEvent;
    event Action<TicketPck> GetTicketPckEvent;
    event Action<string> RelayRecvEvent;
    bool IsConnected { get; }
    void SendPlayerData(IPlayer player);
    void RequestMatch();
    void RelaySend(string payLoad);
}

/// <summary>
/// socket to match making server & relay server
/// try to connect to server when load
/// </summary>
public class MatchNRelayClient : MonoBehaviour, IMatchNRelayClient
{
    public event Action ConnectedEvent; // emit when socket is readied
    public event Action<TicketPck> GetTicketPckEvent;
    public event Action<string> RelayRecvEvent;
    public bool IsConnected => _socket.Connected;

    private SocketIO _socket;
    private TicketPck _ticketPck = null;
    private string _relayPayLoad = null;

    public void SendPlayerData(IPlayer player)
    {
        Debug.Assert(IsConnected);
        PlayerDataPck playerDataPck = new PlayerDataPck(player.Id);
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.PlayerData, playerDataPck));
    }

    public void RequestMatch()
    {
        Debug.Assert(IsConnected);
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.RequestMatch, new RequestMatchPck()));
    }

    public void RelaySend(string payLoad)
    {
        Task.Run(() => _socket.EmitAsync(SocketIOEventNames.RelayData, payLoad));
    }

    private void Awake()
    {
        string url = "";
        // create socket
        if (Application.isEditor)
        {
            (string host, string port) = GetHostNPort();
            url = $"http://{host}:{port}";
        }
        else
        {
            url = NetworkSetting.URL;
        }

        _socket = new SocketIO(url);

        // connect to server
        print($"try to connect: {url}");
        Task.Run(() => _socket.ConnectAsync());
    }

    private void Start()
    {
        StartCoroutine(MainThreadBusyWaitingConnect());
        StartCoroutine(MainThreadBusyWaitingTicketPck());
        StartCoroutine(MainThreadBusyWaitingGameDataPck());
    }

    private IEnumerator MainThreadBusyWaitingConnect()
    {
        // busy wait until connected to server
        yield return new WaitUntil(() => IsConnected);

        // emit connect event
        print("connect to server");
        _socket.On(SocketIOEventNames.Ticket, (res) =>
        {
            TicketPck ticketPck = res.GetValue<TicketPck>();
            _ticketPck = ticketPck;
        });
        _socket.On(SocketIOEventNames.RelayData, (res) =>
        {
            string playLoad = res.GetValue<string>();
            _relayPayLoad = playLoad;
        });
        ConnectedEvent?.Invoke();
    }

    private IEnumerator MainThreadBusyWaitingTicketPck()
    {
        while (true)
        {
            yield return new WaitUntil(() => _ticketPck != null);

            //emit event
            GetTicketPckEvent?.Invoke(_ticketPck);
            // set null again
            _ticketPck = null;
        }
    }

    private IEnumerator MainThreadBusyWaitingGameDataPck()
    {
        while (true)
        {
            yield return new WaitUntil(() => _relayPayLoad != null);

            RelayRecvEvent?.Invoke(_relayPayLoad);
            _relayPayLoad = null;
        }
    }

    private void OnDestroy()
    {
        if (_socket != null)
        {
            Task.Run(() => _socket.DisconnectAsync());
            print("Disconnect from server");
        }
    }

    // return host, port
    private Tuple<string, string> GetHostNPort()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();
        string host, port;
        if (!envVars.TryGetValue("HOST", out host) || !envVars.TryGetValue("PORT", out port))
        {
            throw new ArgumentException(".env not found or not have host, port value");
        }

        return new Tuple<string, string>(host, port);
    }
}