using System;
using UnityEngine;

public interface IP2PSocket
{
    event Action<string> RecvEvent;
    void Send(string payLoad);
}

[RequireComponent(typeof(IMatchNRelayClient))]
public class Relayer : MonoBehaviour, IP2PSocket
{
    public event Action<string> RecvEvent;
    private IMatchNRelayClient _matchNRelayClient;

    public void Send(string payload)
    {
        _matchNRelayClient.RelaySend(payload);
    }

    private void Awake()
    {
        _matchNRelayClient = GetComponent<IMatchNRelayClient>();
        _matchNRelayClient.RelayRecvEvent += OnRelayRecv;
    }

    private void OnRelayRecv(string payload)
    {
        RecvEvent?.Invoke(payload);
    }

    private void OnDestroy()
    {
        _matchNRelayClient.RelayRecvEvent -= OnRelayRecv;
    }
}