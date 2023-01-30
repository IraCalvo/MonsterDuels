using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GamePiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public GamePoint currentPoint;
    // private HomePoint homePoint;
    public int spacesCanMove;
    private Vector3 startingPosition;

    public enum PlayerOwnership { PlayerOne, PlayerTwo };
    public PlayerOwnership playerOwnership;

    public bool isTeamBuilding;

    _GameManager _gameManager;

    public TeamSlot teamSlot;
    public AttackMove[] learnedMoves;
    public List<AttackMove> rngTable;

    void Awake()
    {
        _gameManager = FindObjectOfType<_GameManager>();
        CreateRNGTable();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isTeamBuilding) 
        {
            eventData.pointerDrag = null;
        }
        if(_gameManager.didMove)
        {
            eventData.pointerDrag = null;
        }
        //it's player 2's turn
        if(playerOwnership == PlayerOwnership.PlayerOne && _gameManager.gameState == _GameManager.GameState.player2Turn)
        {
            eventData.pointerDrag = null;
        }
        if(playerOwnership == PlayerOwnership.PlayerTwo && _gameManager.gameState == _GameManager.GameState.player1Turn)
        {
            eventData.pointerDrag = null;
        }
        // We need the starting position so that when there is an invalid move it can go to the starting position
        startingPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Drag the object.transform
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.nearClipPlane;
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        // Find if there is a space under us
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if this move can be made
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        float radius = 0.1f;
        LayerMask layerMask = LayerMask.GetMask("Default");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        
        // Only the knight is detected so move back to the starting position
        if (colliders.Length == 1) {
            transform.position = startingPosition;
            return;
        }
        else 
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.tag == "MapPoint")
                {

                    GamePoint endPoint = collider.gameObject.GetComponent<GamePoint>();

                    // Object hasn't spawned yet
                    if(currentPoint == null && CanMoveToPointFromSpawn(endPoint, spacesCanMove)) {
                        MovePiece(endPoint);
                        CheckForWin();
                        _gameManager.CheckIfPlayerOnePieceIsCaptured();
                        _gameManager.CheckIfPlayerTwoPieceIsCaptured();
                        _gameManager.didMove = true;
                        return;
                    }

                    // Object already spawned, check can move
                    else if (CanMoveToPoint(currentPoint, endPoint, spacesCanMove)) {
                        // Dragged back on same point
                        if (collider.gameObject == currentPoint.gameObject)
                        {
                            transform.position = startingPosition;
                            return;
                        }

                        MovePiece(endPoint);
                        CheckForWin();

                        // Check for captures
                        _gameManager.CheckIfPlayerOnePieceIsCaptured();
                        _gameManager.CheckIfPlayerTwoPieceIsCaptured();
                        _gameManager.didMove = true;
                        return;
                    }
                    else 
                    {
                        transform.position = startingPosition;
                        return;
                    }
                    
                }
            }
        }
    }

    public void MovePiece(GamePoint endPoint) 
    {
        transform.position = endPoint.transform.position;

        // Swapping point ownership
        if (currentPoint != null)
        {
            currentPoint.gamePiece = null;
        }
        endPoint.gamePiece = this;
        currentPoint = endPoint;
    }

    public void CheckForWin() 
    {
        if (currentPoint.name.Contains("HomePoint"))
        {
            HomePoint homePoint = currentPoint.GetComponent<HomePoint>();
            if (homePoint.playerOwnership == HomePoint.PlayerOwnership.player1HomePoint)
            {
                if (playerOwnership == PlayerOwnership.PlayerTwo)
                {
                    PlayerPrefs.DeleteAll();
                    SceneManager.LoadScene("TeamBuilder");
                    Debug.Log("player 2 wins");
                }
            }
            if (homePoint.playerOwnership == HomePoint.PlayerOwnership.player2HomePoint)
            {
                if (playerOwnership == PlayerOwnership.PlayerOne)
                {
                    PlayerPrefs.DeleteAll();
                    SceneManager.LoadScene("TeamBuilder");
                    Debug.Log("player 1 wins");
                }
            }

        }
    }

    public bool CanMoveToPointFromSpawn(GamePoint endPoint, int movesLeft)
    {
        bool canReachEndPoint = false;
        if (_gameManager.gameState == _GameManager.GameState.player1Turn) 
        {
            foreach (SpawnPoint spawnPoint in _gameManager.playerOneSpawnPoints) 
            {
                if (spawnPoint.GetComponent<MapPoint>().gamePiece != null)
                {
                    continue;
                }
                canReachEndPoint = canReachEndPoint || CanMoveToPoint(spawnPoint.GetComponent<MapPoint>(), endPoint, movesLeft-1);
            }
        }
        else 
        {
            foreach (SpawnPoint spawnPoint in _gameManager.playerTwoSpawnPoints)
            {
                if (spawnPoint.GetComponent<MapPoint>().gamePiece != null)
                {
                    continue;
                }
                canReachEndPoint = canReachEndPoint || CanMoveToPoint(spawnPoint.GetComponent<MapPoint>(), endPoint, movesLeft-1);
            }
        }
        return canReachEndPoint;
    }

    public bool CanMoveToPoint(GamePoint currentPoint, GamePoint endPoint, int movesLeft) 
    {
        if (movesLeft == -1) 
        {
            return false;
        }

        if (currentPoint == null)
        {
            return false;
        }

        if (currentPoint.name == endPoint.name) 
        {
            return true;
        }

        bool canReachEndPoint = false;
        foreach (GamePoint point in currentPoint.connectedMapPoints) 
        {
            if (point.gamePiece != null) {
                continue;
            }
            canReachEndPoint = canReachEndPoint || CanMoveToPoint(point, endPoint, movesLeft - 1);
        }

        return canReachEndPoint;
    }

    public void CreateRNGTable()
    {
        int currentIndex = 0;
        foreach(AttackMove attackMove in learnedMoves)
        {
            if (attackMove.attackName == "Miss")
            {
                attackMove.attackChance = 100 - currentIndex;
            }

            int currentMoveWeightIndex = 0;
            while (currentMoveWeightIndex < attackMove.attackChance)
            {
                rngTable.Add(attackMove);
                currentMoveWeightIndex++;
                currentIndex++;
            }
        }
    }
}
