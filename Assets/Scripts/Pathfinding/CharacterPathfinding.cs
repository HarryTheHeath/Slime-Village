using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterPathfinding : MonoBehaviour
{
    private float speed ;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private Vector3 transformPos;
    private Vector3 MousePos;

    // variables for correcting position/ snapping to grid
    public bool moving = false;
    private float CharPosX;
    private float CharPosY;

    private CharacterManager Characters;

    private void Start()
    {
        Characters = FindObjectOfType<CharacterManager>().GetComponent<CharacterManager>();
    }

    private void Update()
    {
        HandleMovement();
        
        if (moving) return;
        
        // speed can be adjusted in play
        speed = Characters.CharacterSpeed;
        
        // get current pos for reference
        var lastPos = GetPosition();
        
        // Round target coordinates to fit grid (end with 5) and create a new coordinate
        CharPosX = Mathf.Abs(Mathf.Ceil(GetPosition().x + 5) -5);
        CharPosY = Mathf.Abs(Mathf.Ceil(GetPosition().y + 5) -5);
        
        // Enforces the end in 5 rule for situations in which positions should have been floored
        TestIfEven();

        var newPos = new Vector3(CharPosX, CharPosY, 0);

        // don't change position if everything looks okay/ keeps it from constantly updating
        if (newPos == lastPos) return;
        
        // snap character to this strict rule if they're not following it already
        moving = false;
        this.GameObject().transform.position = newPos;
    }
    

    private void HandleMovement()
    {
        if (pathVectorList == null) return;
        
        // grabs next target position
        var targetPos = pathVectorList[currentPathIndex];
        
        // checks if far away enough
        if (Vector3.Distance(transformPos, targetPos) > 1f)
        {
            // keeps initial position from being (0,0,0) by default
            if (transformPos == Vector3.zero)
                transformPos = GetPosition();
            
            moving = true;
            // if so, get movement direction ready
            var moveDir = (targetPos - transformPos).normalized;
            transformPos = (transformPos + moveDir * speed * Time.deltaTime);

            // set speed and move towards target position
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transformPos, targetPos, step);
        }
        
        else
        {
            currentPathIndex++;
            if (currentPathIndex >= pathVectorList.Count)
                pathVectorList = null;

            moving = false;
        }
    }

    
    public Vector3 GetPosition() 
    {
        return transform.position;
    }


    // use pathfinding logic to find a path and set a target position to travel to
    public void SetTargetPos(Vector3 targetPos)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPos);

        // clear list upon completion
        if (pathVectorList != null && pathVectorList.Count > 1)
            pathVectorList.RemoveAt(0);
    }
    
    
    // Uses modulo operator % to check if Character's vectors are odd or even
    private void TestIfEven()
    {
        // if X ends in 6
        if (CharPosX % 2 == 0)
            CharPosX = (CharPosX -1);

        // if X ends in 6
        if (CharPosY % 2 == 0)
            CharPosY = (CharPosY -1);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Slime"))
            return;
            //Debug.Log($"{name}: bumped into {other.gameObject.name}");
    }
}
