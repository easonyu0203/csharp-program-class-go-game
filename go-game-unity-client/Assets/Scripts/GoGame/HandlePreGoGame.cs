using System;
using System.Collections;
using GoGameProtocol;
using UnityEngine;

namespace GoGame
{
    /// <summary>
    /// p2p
    /// 1. decide stone type
    /// 2. set up go contestants
    /// </summary>
    public class HandlePreGoGame: MonoBehaviour
    {
        public StoneType LocalStoneType;
        
        // reference
        private IMatchMaker _matchMaker;
        private IPlayer _localPlayer;
        private GamePacketSocket _gamePacketSocket;

        private void Awake()
        {
            _matchMaker = GetComponent<IMatchMaker>();
            _localPlayer = GetComponent<IPlayer>();
            _gamePacketSocket = GetComponent<GamePacketSocket>();
        }

        public IEnumerator DoCoroutine()
        {
            var lwTicekt = new ListenNWaitTicket();
            var lwHandshake = new ListenNWaitHandShake();
            lwTicekt.Listen(_matchMaker);
            lwHandshake.Listen(_gamePacketSocket);

            // Request match
            Debug.Log("request match");
            _matchMaker.RequestMatch();
            // get ticket
            Debug.Log("waiting for ticket...");
            yield return lwTicekt.Wait();
            Debug.Log("get ticket");

            // handshake with peer
            Debug.Log("send and wait for handshake...");
            _gamePacketSocket.Send(new HandShakePck() {SenderId = _localPlayer.Id});
            yield return lwHandshake.Wait();
            Debug.Log("get handshake");

            // decide who is white
            LocalStoneType =
                DecideLocalPlayerStoneType(_localPlayer.Id, lwHandshake.HandShakePck.SenderId);
            
        }

        private StoneType DecideLocalPlayerStoneType(string localId, string peerId)
        {
            return String.CompareOrdinal(localId, peerId) < 0 ? StoneType.White : StoneType.Black;
        }

        private class ListenNWaitTicket
        {
            private bool _haveGetTicket = false;

            public void Listen(IMatchMaker matchMaker)
            {
                void WaitTicket(Ticket t)
                {
                    _haveGetTicket = true;
                    matchMaker.GetTicketEvent -= WaitTicket;
                }

                matchMaker.GetTicketEvent += WaitTicket;
            }

            public IEnumerator Wait()
            {
                yield return new WaitUntil(() => _haveGetTicket);
            }
        }

        private class ListenNWaitHandShake
        {
            private bool _haveGetHandShake = false;
            public HandShakePck HandShakePck { get; private set; }

            public void Listen(GamePacketSocket socket)
            {
                void OnGetHandShake(HandShakePck pck)
                {
                    HandShakePck = pck;
                    _haveGetHandShake = true;
                    socket.GetHandShakePckEvent -= OnGetHandShake;
                }

                socket.GetHandShakePckEvent += OnGetHandShake;
            }

            public IEnumerator Wait()
            {
                yield return new WaitUntil(() => _haveGetHandShake);
            }
        }
    }
}