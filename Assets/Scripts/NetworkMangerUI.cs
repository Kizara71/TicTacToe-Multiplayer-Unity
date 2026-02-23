using UnityEngine;
using System;

public class NetworkMangerUI : MonoBehaviour 
{
    private void Start() 
    {

        GameManger.Instance.OnGameStarted += GameManger_OnGameStarted;
    }

    private void GameManger_OnGameStarted(object sender, EventArgs e) 
    {
        Hide();
    }

    private void Hide() 
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        if (GameManger.Instance != null) 
        {
            GameManger.Instance.OnGameStarted -= GameManger_OnGameStarted;
        }
    }
}
