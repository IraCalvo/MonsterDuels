using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TeamBuilderManager : MonoBehaviour
{

    public Dictionary<string, int> maxAmountOfOneUnitDict = new Dictionary<string, int>() 
    {
        { "PieceKnight", 3 },
        { "PieceGolem", 3},
        {"PieceBird", 3}
    };

    public TeamSlot[] player1Team;
    public TeamSlot[] player2Team;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
