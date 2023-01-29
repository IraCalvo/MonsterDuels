using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class _GameManager : MonoBehaviour
{
    public enum GameState
    {
        player1Turn,
        player2Turn
    }

    public GameState gameState;

    public TeamSlot[] player1Team;
    public TeamSlot[] player2Team;

    public float player1Timer;
    public float player2Timer;

    public GameObject player1LoseScreen;
    public GameObject player2LoseScreen;

    public GameObject[] monsterPieces;
    public MapPoint[] mapPoints;
    public SpawnPoint[] playerOneSpawnPoints;
    public SpawnPoint[] playerTwoSpawnPoints;

    public BattleButton battleButtonPrefab;
    public Image battleList;
    public CloseBattleList closeBattleListPrefab;

    public bool didBattle = false;
    public bool didMove = false;

    void Start()
    {
        LoadPlayer1Team();
        LoadPlayer2Team();
    }

    void update()
    {
        playerTimer();
    }

    void playerTimer()
    {
        if (player1Timer <= 0)
        {
            player1LoseScreen.SetActive(true);
        }
        if (player2Timer <= 0)
        {
            player2LoseScreen.SetActive(true);
        }
    }

    void LoadPlayer1Team()
    {
        foreach(TeamSlot teamSlot in player1Team)
        {
            string monsterTag = PlayerPrefs.GetString("Player1" + teamSlot.name);
            GameObject monsterObjectToSpawn = GetMonsterFromTag(monsterTag, teamSlot.transform.position);
            if (monsterObjectToSpawn != null) {
                GamePiece monsterToSpawn = monsterObjectToSpawn.GetComponent<GamePiece>();
                monsterToSpawn.playerOwnership = GamePiece.PlayerOwnership.PlayerOne;
                monsterToSpawn.teamSlot = teamSlot;
            }
        }
    }

    void LoadPlayer2Team()
    {
        foreach (TeamSlot teamSlot in player2Team)
        {
            string monsterTag = PlayerPrefs.GetString("Player2" + teamSlot.name);
            GameObject monsterObjectToSpawn = GetMonsterFromTag(monsterTag, teamSlot.transform.position);
            if (monsterObjectToSpawn != null)
            {
                GamePiece monsterToSpawn = monsterObjectToSpawn.GetComponent<GamePiece>();
                monsterToSpawn.playerOwnership = GamePiece.PlayerOwnership.PlayerTwo;
                monsterToSpawn.teamSlot = teamSlot;
            }
        }
    }

    GameObject GetMonsterFromTag(string monsterTag, Vector3 teamSlotPosition) {
        if (monsterTag == "PieceKnight") 
        {
            return Instantiate(monsterPieces[0], teamSlotPosition, Quaternion.identity);
        }
        if(monsterTag == "PieceGolem")
        {
            return Instantiate(monsterPieces[1], teamSlotPosition, Quaternion.identity);
        }
        if (monsterTag == "PieceBird")
        {
            return Instantiate(monsterPieces[2], teamSlotPosition, Quaternion.identity);
        }
        return null;
    }

    public void BattleChecker()
    {
        if(gameState == GameState.player1Turn)
        {
            foreach(MapPoint mapPoint in mapPoints)
            {
                if(mapPoint.gamePiece == null)
                {
                    continue;
                }
                if(mapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerOne)
                {
                    foreach (MapPoint connectedMapPoint in mapPoint.connectedMapPoints)
                    {
                        if (connectedMapPoint.gamePiece != null)
                        {
                            if (connectedMapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerTwo)
                            {
                                GameObject battleButtonObject = Instantiate(battleButtonPrefab.gameObject, Vector3.zero, Quaternion.identity);
                                battleButtonObject.transform.SetParent(battleList.gameObject.transform);
                                BattleButton battleButton = battleButtonObject.GetComponent<BattleButton>();
                                battleButtonObject.transform.localScale = new Vector3(1,1,1);
                                battleButton.battleText.text = (mapPoint.gamePiece.name + " (" + mapPoint.name + ") VS. " 
                                + connectedMapPoint.gamePiece.name + " (" + connectedMapPoint.name + ")"   );
                                battleButton.currentTurnPiece = mapPoint.gamePiece;
                                battleButton.enemyPiece = connectedMapPoint.gamePiece;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (MapPoint mapPoint in mapPoints)
            {
                if (mapPoint.gamePiece == null)
                {
                    continue;
                }
                if (mapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerTwo)
                {
                    foreach (MapPoint connectedMapPoint in mapPoint.connectedMapPoints)
                    {
                        if (connectedMapPoint.gamePiece != null)
                        {
                            if (connectedMapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerOne)
                            {
                                GameObject battleButtonObject = Instantiate(battleButtonPrefab.gameObject, Vector3.zero, Quaternion.identity);
                                battleButtonObject.transform.SetParent(battleList.gameObject.transform);
                                BattleButton battleButton = battleButtonObject.GetComponent<BattleButton>();
                                battleButtonObject.transform.localScale = new Vector3(1, 1, 1);
                                battleButton.battleText.text = (mapPoint.gamePiece.name + " (" + mapPoint.name + ") VS. "
                                + connectedMapPoint.gamePiece.name + " (" + connectedMapPoint.name + ")");
                                battleButton.currentTurnPiece = mapPoint.gamePiece;
                                battleButton.enemyPiece = connectedMapPoint.gamePiece;
                            }
                        }
                    }
                }
            }
        }
        // Add Close Button
        GameObject closeButtonObject = Instantiate(closeBattleListPrefab.gameObject, Vector3.zero, Quaternion.identity);
        closeButtonObject.transform.SetParent(battleList.gameObject.transform);
        closeButtonObject.transform.localScale = new Vector3(1, 1, 1);
        CloseBattleList closeButton = closeButtonObject.GetComponent<CloseBattleList>();
        closeButton.battleList = battleList;

    }
    public void CheckIfPlayerOnePieceIsCaptured()
    {
        //if the a piece's current connected points are all owned by the other player
        foreach(MapPoint mapPoint in mapPoints)
        {
            if(mapPoint.gamePiece == null)
            {
                continue;
            }
            int connectedMapPointCounter = 0;
            //get the ownership of the connected points
            foreach(MapPoint connectedMapPoint in mapPoint.connectedMapPoints)
            {
                if(connectedMapPoint.gamePiece != null)
                {
                    if(connectedMapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerTwo)
                    {
                        connectedMapPointCounter++;
                    }
                }
            }
            if(connectedMapPointCounter == mapPoint.connectedMapPoints.Length)
            {
                //TODO: move to jail later
                // Destroy(mapPoint.gamePiece.gameObject);
                mapPoint.gamePiece.gameObject.transform.position = mapPoint.gamePiece.teamSlot.transform.position;
                mapPoint.gamePiece.currentPoint = null;
                mapPoint.gamePiece = null;
            }
        }
    }

    public void CheckIfPlayerTwoPieceIsCaptured()
    {
        foreach (MapPoint mapPoint in mapPoints)
        {
            if (mapPoint.gamePiece == null)
            {
                continue;
            }
            int connectedMapPointCounter = 0;
            //get the ownership of the connected points
            foreach (MapPoint connectedMapPoint in mapPoint.connectedMapPoints)
            {
                if (connectedMapPoint.gamePiece != null)
                {
                    if (connectedMapPoint.gamePiece.playerOwnership == GamePiece.PlayerOwnership.PlayerOne)
                    {
                        connectedMapPointCounter++;
                    }
                }
            }
            if (connectedMapPointCounter == mapPoint.connectedMapPoints.Length)
            {
                //TODO: move to jail later
                // Destroy(mapPoint.gamePiece.gameObject);
                mapPoint.gamePiece.gameObject.transform.position = mapPoint.gamePiece.teamSlot.transform.position;
                mapPoint.gamePiece.currentPoint = null;
                mapPoint.gamePiece = null;
            }
        }
    }

    public void EndPlayerOneTurn()
    {
        gameState = GameState.player2Turn;
        didBattle = false;
        didMove = false;
    }

    public void EndPlayerTwoTurn()
    {
        gameState = GameState.player1Turn;
        didBattle = false;
        didMove = false;
    }
}
