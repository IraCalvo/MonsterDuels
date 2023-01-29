using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndTurnButton : MonoBehaviour
{
    _GameManager _gameManager;
    public TextMeshProUGUI buttonText;
    void Awake()
    {
        _gameManager = FindObjectOfType<_GameManager>();
    }

    public void ButtonClicked()
    {
        if (_gameManager.gameState == _GameManager.GameState.player1Turn)
        {
            _gameManager.EndPlayerOneTurn();
            buttonText.text = ("Player 2 End Turn");
        }
        else
        {
            _gameManager.EndPlayerTwoTurn();
            buttonText.text = ("Player 1 End Turn");
        }
    }
}
