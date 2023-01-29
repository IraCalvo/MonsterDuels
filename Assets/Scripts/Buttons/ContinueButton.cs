using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{

    public _TeamBuilderManager _teamBuilderManager;
    public void ButtonPressed()
    {
        PlayerPrefs.DeleteAll();
        SaveTeamOne();
        SaveTeamTwo();
        SceneManager.LoadScene("localPlayNormalBoard");
    }

    void SaveTeamOne()
    {
        foreach(TeamSlot teamSlot in _teamBuilderManager.player1Team)
        {
            if(teamSlot.monsterPicked != null)
            {
                PlayerPrefs.SetString("Player1" + teamSlot.name, teamSlot.monsterPicked.gameObject.tag);
            }
        }
    }

    void SaveTeamTwo()
    {
        foreach(TeamSlot teamSlot in _teamBuilderManager.player2Team)
        {
            if(teamSlot.monsterPicked != null)
            {
                PlayerPrefs.SetString("Player2" + teamSlot.name, teamSlot.monsterPicked.gameObject.tag);
            }
        }
    }
}
