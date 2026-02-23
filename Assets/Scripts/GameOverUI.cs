using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private Color winColor; 
    [SerializeField] private Color loseColor;
    [SerializeField] private Color drawColor;
    [SerializeField] private Button rematchButton;
    [SerializeField] private RectTransform mainPanel;
    private Sequence flagSequence;

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
        StartFlagWave();
    }

    private void Hide()
    {
        flagSequence?.Kill();
        gameObject.SetActive(false);
    }

    private void StartFlagWave()
    {
        // Ensure the pivot is handled on the mainPanel
        mainPanel.localRotation = Quaternion.identity;
        mainPanel.localScale = Vector3.one;
        flagSequence?.Kill();

        flagSequence = DOTween.Sequence();

        // 1. The Hinge Motion: Rotate the whole panel back and forth
        flagSequence.Append(mainPanel.DORotate(new Vector3(0, 0, 4f), 1.2f).SetEase(Ease.InOutQuad))
                    .Append(mainPanel.DORotate(new Vector3(0, 0, -4f), 1.2f).SetEase(Ease.InOutQuad));

        // 2. The Flutter: Join a scale tween to make the "fabric" stretch slightly
        // Scaling only X makes it look like it's catching wind
        flagSequence.Join(mainPanel.DOScaleX(1.1f, 0.6f).SetEase(Ease.InOutSine).SetLoops(4, LoopType.Yoyo))
                    .SetLoops(-1); // Infinite loop
                    
        // 3. Optional: Add a subtle vertical bob
        flagSequence.Join(mainPanel.DOAnchorPosY(mainPanel.anchoredPosition.y + 10f, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
    }
}