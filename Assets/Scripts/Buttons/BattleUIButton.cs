using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIButton : MonoBehaviour
{
    public Image battleList;
    _GameManager _gameManager;

    void Awake()
    {
        _gameManager = FindObjectOfType<_GameManager>();
    }

    public void ButtonClicked()
    {
        if (_gameManager.didBattle == false)
        {
            _gameManager.BattleChecker();
            battleList.gameObject.SetActive(true);
        }
        
    }
}
