using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private Color winColor; 
    [SerializeField] private Color loseColor;
    [SerializeField] private Color drawColor;
    [SerializeField] private Button rematchButton;

    void Awake()
    {
        rematchButton.onClick.AddListener(() => {
            GameManger.Instance.RematchRpc();
            Hide();
        });
    }

    void Start()
    {
        Hide();   
        GameManger.Instance.OnGameWin += GameManger_OnGameWin;
        GameManger.Instance.OnRematch += GameManger_OnRematch;
        GameManger.Instance.OnGameDraw += GameManger_OnGameDraw;
    }

    private void GameManger_OnGameDraw(object sender, System.EventArgs e)
    {
        youWinText.text = "Draw!";
        youWinText.color = drawColor;
        Show();
    }   

    private void GameManger_OnRematch(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManger_OnGameWin(object sender, GameManger.OnGameWinEventArgs e)
    {
        if(e.winnerPlayerType == GameManger.Instance.GetLocalPlayerType())
        {
            youWinText.text = "You Win!";
            youWinText.color = winColor;
        }
        else
        {
            youWinText.text = "You Lose!";
            youWinText.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
