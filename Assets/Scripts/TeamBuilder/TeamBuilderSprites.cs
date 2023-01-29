using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamBuilderSprites : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public GameObject monsterHeld;
    private GamePiece monsterPicked;
    public _TeamBuilderManager _teamBuilderManager;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SpriteRenderer monsterHeldSR = monsterHeld.GetComponent<SpriteRenderer>();
        sr.sprite = monsterHeldSR.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.nearClipPlane;
        screenPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        GameObject monsterPickedObject = Instantiate(monsterHeld, screenPoint, Quaternion.identity);
        monsterPicked = monsterPickedObject.GetComponent<GamePiece>();
        monsterPicked.isTeamBuilding = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.nearClipPlane;
        monsterPicked.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        Debug.Log("TeamBuilderSprites:OnDrag:");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //case: if want to replace

        monsterPicked.transform.position = new Vector3(monsterPicked.transform.position.x, monsterPicked.transform.position.y, 0);
        float radius = 0.1f;
        LayerMask layerMask = LayerMask.GetMask("TeamSlot");
        Collider2D collider = Physics2D.OverlapCircle(monsterPicked.transform.position, radius, layerMask);
        if(collider == null)
        {
            Destroy(monsterPicked.gameObject);
            return;
        }
        // Check if team full of this unit
        else if (CheckIfTeamFullOfUnit(collider.gameObject.GetComponent<TeamSlot>())) 
        {
            Destroy(monsterPicked.gameObject);
            return;
        }
        else
        {
            if(collider.gameObject.tag == "TeamSlot")
            {
                TeamSlot teamSlot = collider.gameObject.GetComponent<TeamSlot>();
                monsterPicked.transform.position = teamSlot.transform.position;
                teamSlot.monsterPicked = monsterPicked;
            }
        }


    }

    bool CheckIfTeamFullOfUnit(TeamSlot currentTeamSlot) 
    {
        int monsterCounter = 0;
        if (currentTeamSlot.playerOwnership == TeamSlot.PlayerOwnership.PlayerOne) 
        {
            foreach (TeamSlot teamSlot in _teamBuilderManager.player1Team) 
            {
                if (teamSlot.monsterPicked != null) {
                    if (teamSlot.monsterPicked.gameObject.tag == monsterPicked.gameObject.tag)
                    {
                        monsterCounter++;
                    }
                }
            }
        }
        if (currentTeamSlot.playerOwnership == TeamSlot.PlayerOwnership.PlayerTwo)
        {
            foreach (TeamSlot teamSlot in _teamBuilderManager.player2Team)
            {
                if (teamSlot.monsterPicked != null)
                {
                    if (teamSlot.monsterPicked.gameObject.tag == monsterPicked.gameObject.tag)
                    {
                        monsterCounter++;
                    }
                }
            }
        }

        int monsterPickedLimit = _teamBuilderManager.maxAmountOfOneUnitDict[monsterPicked.gameObject.tag];
        if (monsterCounter < monsterPickedLimit) 
        {
            return false;
        }
        return true;
    }
}
