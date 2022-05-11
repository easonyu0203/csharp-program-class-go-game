using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoGameProtocol;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoGame
{
    public interface IHandleInGoGame
    {
        event Action<StoneType> GoGameEndEvent;
        void StartGoGame(StoneType localPlayer);
        void EndGoGame();
    }

    /// <summary>
    /// Integrate behavior of go logic, board, input
    /// </summary>
    [RequireComponent(typeof(IMouseInGameDataProvider), typeof(PlayerInput))]
    public class HandleInGoGame : MonoBehaviour, IHandleInGoGame
    {
        // reference
        private IMouseInGameDataProvider _mouseInGameDataProvider;
        private IGoLogic _goLogic;
        private IBoard _board;

        private GamePacketSocket _gamePacketSocket;
        //state
        private StoneType _currentPlayer;

        private StoneType _localPlayer;
        //event
        private event Action<StoneType, int2> PlaceStoneEvent;
        private event Action SelectBoardEvent;

        public event Action<StoneType> GoGameEndEvent;

        private void Awake()
        {
            _mouseInGameDataProvider = GetComponent<IMouseInGameDataProvider>();
            _goLogic = GetComponent<IGoLogic>();
            _board = GameObject.Find("GoBoard").GetComponent<IBoard>();
            _gamePacketSocket = GetComponent<GamePacketSocket>();
        }

        public void StartGoGame(StoneType localPlayer)
        {
            // set local player stone type
            _localPlayer = localPlayer;
            // listen for win event
            void OnWin(StoneType winPlayer)
            {
                print($"{winPlayer} win!!");
                GoGameEndEvent?.Invoke(winPlayer);
                _goLogic.WinEvent -= OnWin;
                PlaceStoneEvent -= OnPlaceStone;
                SelectBoardEvent -= OnSelectBoard;
                _gamePacketSocket.GetPlaceStonePckEvent -= OnGetPlaceStonePck;
            }

            void OnGetPlaceStonePck(PlaceStonePck placeStonePck)
            {
                Debug.Assert(_currentPlayer != _localPlayer);
                PlaceStone((StoneType)placeStonePck.StoneType, new int2(placeStonePck.X, placeStonePck.Y));
            }

            void OnPlaceStone(StoneType stoneType, int2 xy)
            {
                // send our place stone event to peer
                if (_currentPlayer == _localPlayer)
                {
                    _gamePacketSocket.Send(new PlaceStonePck(){StoneType = (int)stoneType, X = xy.x, Y = xy.y});
                }
                
                // visual place stone 
                _board.PlaceStone(stoneType, xy);
                // compute go game logic
                _goLogic.PlaceStone(stoneType, xy);
                
                // switch current player
                _currentPlayer = (_currentPlayer == StoneType.Black) ? StoneType.White : StoneType.Black;
            }

            void OnSelectBoard()
            {
                // is local player round or not
                if(_currentPlayer != _localPlayer) return;
                
                int2 placeItersectionIndex = _board.NearestIntersectionIndex(_mouseInGameDataProvider.MouseWOrldPosition);
                if (_goLogic.IsIntersectionOccupied(placeItersectionIndex))
                {
                    print("this intersection is occupied");
                }
                else
                {
                    // place a stone
                    PlaceStone(_currentPlayer, placeItersectionIndex);
                }
            }

            _goLogic.WinEvent += OnWin;
            PlaceStoneEvent += OnPlaceStone;
            SelectBoardEvent += OnSelectBoard;
            _gamePacketSocket.GetPlaceStonePckEvent += OnGetPlaceStonePck;

            // white go first
            _currentPlayer = StoneType.White;
        }

        private void PlaceStone(StoneType stoneType, int2 xy)
        {

            
            PlaceStoneEvent?.Invoke(stoneType, xy);
        }

        public void EndGoGame()
        {
            _board.Clear();
            _goLogic.ClearBoard();
        }

        private void OnSelect()
        {
            // check hit anything
            if (_mouseInGameDataProvider.MouseHit == null) return;

            // if mouse hit board
            IBoard board = _mouseInGameDataProvider.MouseHit.transform.GetComponent<IBoard>();
            if (board != null)
            {
                SelectBoardEvent?.Invoke();
            }
        }
    }
}