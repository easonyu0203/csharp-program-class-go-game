using System;
using System.Collections;
using UnityEngine;

public interface IMatchMaker
{
    bool HaveRegister { get; }
    event Action<Ticket> GetTicketEvent;
    void RegisterPlayer(IPlayer player);
    void RequestMatch();
}

[RequireComponent(typeof(IMatchNRelayClient))]
public class MatchMaker : MonoBehaviour, IMatchMaker
{
    public bool HaveRegister { get; private set; }
    public event Action<Ticket> GetTicketEvent;
    private IMatchNRelayClient _matchNRelayClient;

    public void RegisterPlayer(IPlayer player)
    {
        StartCoroutine(DoCoroutine());

        IEnumerator DoCoroutine()
        {
            yield return new WaitUntil(() => _matchNRelayClient.IsConnected);
            _matchNRelayClient.SendPlayerData(player);
            HaveRegister = true;
        }
    }


    public void RequestMatch()
    {
        StartCoroutine(DoCoroutine());

        IEnumerator DoCoroutine()
        {
            yield return new WaitUntil(() => _matchNRelayClient.IsConnected);
            Debug.Assert(HaveRegister);
            _matchNRelayClient.RequestMatch();
        }
    }

    private void Awake()
    {
        _matchNRelayClient = GetComponent<IMatchNRelayClient>();
        _matchNRelayClient.ConnectedEvent += OnConnectMatchServer;
    }

    private void OnConnectMatchServer()
    {
        _matchNRelayClient.GetTicketPckEvent += OnGetTicketPck;
    }

    private void OnGetTicketPck(TicketPck ticketPck)
    {
        GetTicketEvent?.Invoke(new Ticket(ticketPck.P2PConnectMethod));
    }

    private void OnDestroy()
    {
        if (_matchNRelayClient != null) _matchNRelayClient.GetTicketPckEvent -= OnGetTicketPck;
    }
}

public class Ticket
{
    public string P2PConnectMethod { get; private set; }

    public Ticket(string p2PConnectMethod)
    {
        P2PConnectMethod = p2PConnectMethod;
    }
}