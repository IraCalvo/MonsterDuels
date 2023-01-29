using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBattleList : MonoBehaviour
{
    public Image battleList;

    void Awake()
    {
    }
    public void ButtonClicked()
    {
        battleList.gameObject.SetActive(false);
        BattleButton[] battleButtonsList = battleList.gameObject.GetComponentsInChildren<BattleButton>();
        foreach(BattleButton battleButton in battleButtonsList)
        {
            Destroy(battleButton.gameObject);
        }

        CloseBattleList closeBattleListButton = battleList.gameObject.GetComponentInChildren<CloseBattleList>();
        Destroy(closeBattleListButton.gameObject);
    }
}
