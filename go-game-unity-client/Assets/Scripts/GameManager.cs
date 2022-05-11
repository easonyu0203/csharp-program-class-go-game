using System;
using System.Collections;
using GoGame;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private IHandleInGoGame _handleInGoGame;
    private IUIManager _uiManager;
    private IPlayer _localPlayer;
    private IMatchMaker _matchMaker;
    private GoGameHandler _goGameHandler;
    
    private void Awake()
    {
        _handleInGoGame = GetComponent<IHandleInGoGame>();
        _uiManager = GetComponent<IUIManager>();
        _localPlayer = GetComponent<IPlayer>();
        _matchMaker = GetComponent<IMatchMaker>();
        _goGameHandler = GetComponent<GoGameHandler>();
        _handleInGoGame.GoGameEndEvent += OnHandleInGoGameEnd;
    }

    private void Start()
    {
        // init UI
        _uiManager.ShowStartPage();
        // prepare match making
        _matchMaker.RegisterPlayer(_localPlayer);
    }

    public void FindMatchNStartGoGame()
    {
        // starting pre go game communication
        // 1. request match
        // 2. get ticket
        // 3. communicate with peer
        StartCoroutine(DoCoroutine());

        IEnumerator DoCoroutine()
        {
            yield return _goGameHandler.PreGoGameRoutine();
            StartGame();
        }
    }

    public void StartGame()
    {
        // clean up
        _handleInGoGame.EndGoGame();
        _uiManager.HideAllPage();
        // start game
        _goGameHandler.StartGoGame();
    }

    private void OnHandleInGoGameEnd(StoneType winPlayer)
    {
        _uiManager.ShowGoGameOverPage(winPlayer);
    }
}