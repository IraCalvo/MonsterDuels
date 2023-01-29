using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : GamePoint
{
    // Start is called before the first frame update
    public enum PlayerOwnership
    {
        player1SpawnPoint,
        player2SpawnPoint
    }

    public PlayerOwnership playerOwnership;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
