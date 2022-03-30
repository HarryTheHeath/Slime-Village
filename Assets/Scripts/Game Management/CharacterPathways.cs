using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class CharacterPathways : MonoBehaviour
{
    public Pathfinding pathfinding;
    private CharacterPathfinding characterPathfinding;
    private CharacterRoutine characterRoutine;
    private float OriginX;
    private float OriginY;
    private Grid grid;
    public Vector3 targetPos;
    public CharacterManager Characters;


    private void Start()
    {
        Characters = FindObjectOfType<CharacterManager>().GetComponent<CharacterManager>();
        characterPathfinding = this.gameObject.GetComponent<CharacterPathfinding>();
        characterRoutine = this.gameObject.GetComponent<CharacterRoutine>();
        pathfinding = Characters.pathfinding;
    }

    
    public void ToggleTarget()
    {
        // head to a landmark target position if at home
        if (this.GameObject().transform.position == characterRoutine.housePos)
            targetPos = characterRoutine.setPos;

        // head home if not moving and not at home i.e. at target position
        else if (!characterPathfinding.moving && this.GameObject().transform.position != characterRoutine.housePos)
            targetPos = characterRoutine.housePos;
    }


    public void CalculatePathfinding(Pathfinding sharedPathfinding)
    {
        // converts setPos into a x,y coordinate
        sharedPathfinding.GetGrid().GetXY(targetPos, out var x, out var y);
        // get character's current position
        var charPos = characterPathfinding.GetPosition();
        // floor and round down to meet node value structures
        var charPosX = Mathf.FloorToInt(charPos.x) / 10;
        var charPosY = Mathf.FloorToInt(charPos.y) / 10;
        var path = sharedPathfinding.FindPath(charPosX, charPosY, x, y);
        
        AssignPathfinding(path);
    }

    
    private void AssignPathfinding(IReadOnlyList<Pathnode> path)
    {
        // calculates a path starting from the player's transform position going and ending at the mouse position coordinates
        if (path == null || !Characters.showPathLines) return;
        
        for (var i = 0; i < path.Count - 1; i++)
        {
            // draws a path line from 0,0
            Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f,
                new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, characterRoutine.pathLineColour,
                duration: 5f);
        }
        characterPathfinding.SetTargetPos(targetPos);
    }
}