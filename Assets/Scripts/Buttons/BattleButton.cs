using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleButton : MonoBehaviour
{
    public TextMeshProUGUI battleText;

    public GamePiece currentTurnPiece;
    public GamePiece enemyPiece;

    public _GameManager _gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        _gameManager = FindObjectOfType<_GameManager>();
    }
    public void ButtonClicked()
    {
        if(_gameManager.didBattle == false)
        {
            _gameManager.didBattle = true;
            int currentTurnPieceRoll = Random.Range(0, 100);
            int enemyPieceRoll = Random.Range(0, 100);

            AttackMove currentTurnPieceAttackMove = currentTurnPiece.rngTable[currentTurnPieceRoll];
            Debug.Log("You rolled" + currentTurnPieceAttackMove.attackName);
            AttackMove enemyPieceRollAttackMove = enemyPiece.rngTable[enemyPieceRoll];
            Debug.Log("Enemy rolled" + enemyPieceRollAttackMove.attackName);

            if (currentTurnPieceAttackMove.attackPower > enemyPieceRollAttackMove.attackPower)
            {
                // Destroy(enemyPiece.gameObject);
                enemyPiece.transform.position = enemyPiece.teamSlot.transform.position;
                enemyPiece.currentPoint.gamePiece = null;
                enemyPiece.currentPoint = null;
            }
            else if (enemyPieceRollAttackMove.attackPower > currentTurnPieceAttackMove.attackPower)
            {
                // Destroy(currentTurnPiece.gameObject);
                currentTurnPiece.transform.position = currentTurnPiece.teamSlot.transform.position;
                currentTurnPiece.currentPoint.gamePiece = null;
                currentTurnPiece.currentPoint = null;
            }
        }
    }
}
