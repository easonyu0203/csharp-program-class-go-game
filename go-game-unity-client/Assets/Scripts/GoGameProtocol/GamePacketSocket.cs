using System;
using System.Text.Json;
using UnityEngine;

namespace GoGameProtocol
{
    [RequireComponent(typeof(IP2PSocket))]
    public class GamePacketSocket : MonoBehaviour
    {
        public event Action<HandShakePck> GetHandShakePckEvent;
        public event Action<PlaceStonePck> GetPlaceStonePckEvent;
        

        private IP2PSocket _p2PSocket;

        public void Send<T>(T gamePck) where T: GamePck
        {   
            string js = JsonSerializer.Serialize(gamePck);
            _p2PSocket.Send(js);
        }

        private void Awake()
        {
            _p2PSocket = GetComponent<IP2PSocket>();
            _p2PSocket.RecvEvent += OnP2PSocketRecv;
        }

        private void OnP2PSocketRecv(string payload)
        {
            try
            {
                GamePck gamePck = JsonSerializer.Deserialize<GamePck>(payload);
                if (gamePck == null)
                {
                    Console.WriteLine("Get game pck but can't deserialize it");
                    Console.WriteLine($"Payload: {payload}");
                    return;
                }

                switch (gamePck.PckName)
                {
                    case GamePckNames.HandShake:
                        HandShakePck handShakePck = JsonSerializer.Deserialize<HandShakePck>(payload);
                        GetHandShakePckEvent?.Invoke(handShakePck);
                        break;
                    case GamePckNames.PlaceStone:
                        PlaceStonePck placeStonePck = JsonSerializer.Deserialize<PlaceStonePck>(payload);
                        GetPlaceStonePckEvent?.Invoke(placeStonePck);
                        break;
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine("Get game pck but can't deserialize it");
                Console.WriteLine($"Payload: {payload}");
                Console.WriteLine(e);
            }
        }

        private void OnDestroy()
        {
            _p2PSocket.RecvEvent -= OnP2PSocketRecv;
        }
    }
}