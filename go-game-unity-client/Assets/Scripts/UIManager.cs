using TMPro;
using UnityEngine;


public interface IUIManager
{
    void HideAllPage();
    void ShowStartPage();
    void ShowGoGameOverPage(StoneType winner);
}

public class UIManager : MonoBehaviour, IUIManager
{
    [SerializeField] private GameObject _diffusePanel;
    [SerializeField] private GameObject _startPage;
    [SerializeField] private GameObject _gameOverPage;
    [SerializeField] private GameObject _waitingmatchPage;
    [SerializeField] private TextMeshProUGUI _winnerText;

    public void HideAllPage()
    {
        _diffusePanel.SetActive(false);
        _startPage.SetActive(false);
        _gameOverPage.SetActive(false);
        _waitingmatchPage.SetActive(false);
    }

    public void ShowStartPage()
    {
        _diffusePanel.SetActive(true);
        _gameOverPage.SetActive(false);
        _waitingmatchPage.SetActive(false);
        _startPage.SetActive(true);
    }
    
    public void ShowGoGameOverPage(StoneType winner)
    {
        _diffusePanel.SetActive(true);
        _gameOverPage.SetActive(true);
        _winnerText.text = $"{winner} WIN!!!";
    }

    public void ShowWaitMatchPage()
    {
        _diffusePanel.SetActive(true);
        _gameOverPage.SetActive(false);
        _waitingmatchPage.SetActive(true);
        _startPage.SetActive(false);
    }

}

