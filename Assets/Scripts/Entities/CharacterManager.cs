using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Pathfinding pathfinding;
    
    // Gameplay variables
    [HideInInspector] public int width = 20;
    [HideInInspector] public int height = 10;
    public bool showGrid;
    public bool showGridText;
    public bool showPathLines;
    public float CharacterSpeed = 25f;
    public float DaysBetweenSeasons;
    public float CharacterVanishTime = 2.0f;
    public float PathfindingWaitTime;

    // Slime GameObjects
    public GameObject Character1;
    public GameObject Character2;
    public GameObject Character3;
    public GameObject Character4;
    public GameObject Character5;
    public GameObject Character6;

    // Target Location parent objects
    public GameObject Target1;
    public GameObject Target2;
    public GameObject Target3;
    public GameObject Target4;
    public GameObject Target5;
    public GameObject Target6;

    // Houses
    public GameObject House1;
    public GameObject House2;
    public GameObject House3;
    public GameObject House4;
    public GameObject House5;
    public GameObject House6;

    // Null location indicator and DayTracker
    public GameObject NullLocationIndicator;
    private Vector3 NullLocationDefaultPos;
    public float NullLocationResetTime;
    private GameObject DayTracker;
    public int DayCount;

    // Lists of targets
    private readonly List<GameObject> targetLocations = new List<GameObject>();
    private readonly List<GameObject> targetChildrenLocations = new List<GameObject>();
    private readonly List<GameObject> houseLocations = new List<GameObject>();

    // Other variables
    [HideInInspector] public SpriteRenderer otherSR;
    [HideInInspector] public Vector3 mouseWorldPos;
    public bool AllCharactersNotMoving;
    public bool AllCharactersAreHome;
    

    private void Start()
    {
        // create new pathfinding and grid
        pathfinding = new Pathfinding(width, height);
        NullLocationDefaultPos = NullLocationIndicator.transform.position;

        // Add targets to the targetLocations list
        targetLocations.Add(Target1);
        targetLocations.Add(Target2);
        targetLocations.Add(Target3);
        targetLocations.Add(Target4);
        targetLocations.Add(Target5);
        targetLocations.Add(Target6);
        
        // Add houses to the houseLocations list
        houseLocations.Add(House1);
        houseLocations.Add(House2);
        houseLocations.Add(House3);
        houseLocations.Add(House4);
        houseLocations.Add(House5);
        houseLocations.Add(House6);

        // Populate targetChildrenLocations list using children of targetLocations list
        foreach (var target in targetLocations)
        {
            var child0 = target.transform.GetChild(0).gameObject;
            var child1 = target.transform.GetChild(1).gameObject;
            targetChildrenLocations.Add(child0);
            targetChildrenLocations.Add(child1);
        }
        
        // Day tracking UI logic
        DayTracker = GameObject.Find("Day Tracker");
        DayTracker.GetComponent<TMP_Text>().text = $"Day {DayCount+1}";


        // Toggle Grid Text (shows cell coordinates of each block)
        if (showGridText)
            FindObjectOfType<TextController>().GetComponent<TextController>().ShowText = true;
        
        CheckIfAtHome();
        SetLandmarkLocationsUnwalkable();
    }

    
    private void Update()
    {
        // need this check a lot so constantly updates
        CheckIfAllMoving();
        
        // characters start new paths on left-click if all are not moving (i.e. at home or at target)
        if (Input.GetMouseButtonDown(0))
        {
            if (!AllCharactersNotMoving) return;

            RandomizeTarget();
            CalculateDistances();
            ToggleTargetLocation();

            // if characters are home we can update the day when a new path is triggered
            CheckIfAtHome();
            if (AllCharactersAreHome)
            {
                // Increment and visualise days
                    DayCount++;
                    DayTracker.GetComponent<TMP_Text>().text = $"Day {DayCount}";
                    
                    // change seasons based on current daysBetweenSeasons Inspector variable
                    if (DayCount % DaysBetweenSeasons == 0)
                        FindObjectOfType<Grass>().GetComponent<Grass>().ChangeSeason();
            }
            
            // okay to start moving towards target
            AssignPathfinding();
        }

        // toggle boulders on right-click to block slimes
        else if (Input.GetMouseButtonDown(1))
        {
            CheckIfAtHome();
            ToggleWalls();
        }
    }


    // method for uniquely shuffling and distributing landmark locations without overlaps
    private void RandomizeTarget()
    {
       // shuffle targetLocations list using a new GUID (globally unique identifier) which can be generated very quickly
        var shuffled = targetLocations.OrderBy(x => Guid.NewGuid()).ToList();

        // feed shuffled list into method
        SetRandomTarget(shuffled[0], shuffled[1], shuffled[2], shuffled[3], shuffled[4], shuffled[5]);
    }

    
    private void SetRandomTarget(GameObject one, GameObject two, GameObject three, GameObject four, GameObject five, GameObject six)
    {
        Character1.GetComponent<CharacterRoutine>().targetPos = one;
        Character2.GetComponent<CharacterRoutine>().targetPos = two;
        Character3.GetComponent<CharacterRoutine>().targetPos = three;
        Character4.GetComponent<CharacterRoutine>().targetPos = four;
        Character5.GetComponent<CharacterRoutine>().targetPos = five;
        Character6.GetComponent<CharacterRoutine>().targetPos = six;
    }

    
    private void CalculateDistances()
    {
        Character1.GetComponent<CharacterRoutine>().CalculateDistance();
        Character2.GetComponent<CharacterRoutine>().CalculateDistance();
        Character3.GetComponent<CharacterRoutine>().CalculateDistance();
        Character4.GetComponent<CharacterRoutine>().CalculateDistance();
        Character5.GetComponent<CharacterRoutine>().CalculateDistance();
        Character6.GetComponent<CharacterRoutine>().CalculateDistance();
    }
    
    
    private void ToggleTargetLocation()
    {
        Character1.GetComponent<CharacterPathways>().ToggleTarget();
        Character2.GetComponent<CharacterPathways>().ToggleTarget();
        Character3.GetComponent<CharacterPathways>().ToggleTarget();
        Character4.GetComponent<CharacterPathways>().ToggleTarget();
        Character5.GetComponent<CharacterPathways>().ToggleTarget();
        Character6.GetComponent<CharacterPathways>().ToggleTarget();
    }
    
    
    private void AssignPathfinding()
    {
        Character1.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character2.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character3.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character4.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character5.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character6.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
    }


    private void ToggleWalls()
    {
        mouseWorldPos = WorldPosition.GetMouseWorldPos();
        // swap out isWalkable to false
        // Convert mouse click to vector 2 for ray-casting
        var worldPos2D = mouseWorldPos.ConvertTo<Vector2>();

        // Send out a raycast from the mouse click
        var hit = Physics2D.Raycast(worldPos2D, Vector2.down);
        var other = hit.collider;
        // check if an object was hit
        if (other == null) return;
        
        // exceptions for invalid wall placement locations
        if (other.CompareTag("Landmark"))
        {
            Debug.Log("Can't put boulder on Landmark");
            return; 
        }
        else if (other.CompareTag("House"))
        {
            Debug.Log("Can't put boulder on House");
            return; 
        }
        else if (other.CompareTag("Day Tracker"))
        {
            Debug.Log("Can't put boulder on Day Tracker");
            return; 
        }
        else if (other.CompareTag("Slime"))
        {
            Debug.Log("You nearly squished a slime! :O");
            return; 
        }
        // can't be out of bounds, basically
        else if (!other.CompareTag("Wall")) return;
        
        // toggle sprite renderer of wall if not already an un-walkable area
        var otherGO = other.gameObject;
        pathfinding.GetGrid().GetXY(otherGO.transform.position, out int x, out int y);
        otherSR = otherGO.GetComponent<SpriteRenderer>();

        CheckIfWallWillBlockPathfinding(x, y, otherSR, mouseWorldPos);

        //if (!otherSR.enabled && !pathfinding.GetNode(x, y).isWalkable) return;
        
        // var ShadowSr = other.transform.GetChild(1).GetComponent<SpriteRenderer>();
        // ShadowSr.enabled = otherSR.enabled;
    }

    
    private void CheckIfAllMoving()
    {
        var char1moving = Character1.GetComponent<CharacterPathfinding>().moving;
        var char2moving = Character2.GetComponent<CharacterPathfinding>().moving;
        var char3moving = Character3.GetComponent<CharacterPathfinding>().moving;
        var char4moving = Character4.GetComponent<CharacterPathfinding>().moving;
        var char5moving = Character5.GetComponent<CharacterPathfinding>().moving;
        var char6moving = Character6.GetComponent<CharacterPathfinding>().moving;

        if (!char1moving && !char2moving && !char3moving && !char4moving && !char5moving && !char6moving)
            AllCharactersNotMoving = true;
        else
            AllCharactersNotMoving = false;
    }

    
    private void CheckIfAtHome()
    {
        var home1 = Character1.GetComponent<CharacterRoutine>().atHome;
        var home2 = Character2.GetComponent<CharacterRoutine>().atHome;
        var home3 = Character3.GetComponent<CharacterRoutine>().atHome;
        var home4 = Character4.GetComponent<CharacterRoutine>().atHome;
        var home5 = Character5.GetComponent<CharacterRoutine>().atHome;
        var home6 = Character6.GetComponent<CharacterRoutine>().atHome;

        if (home1 && home2 && home3 && home4 && home5 && home6)
            AllCharactersAreHome = true;
        //StartCoroutine(Wait());
        else
            AllCharactersAreHome = false;
    }
    
    
    private IEnumerator Wait()
    {
        // wait before marking all as not moving so can't move them again straightaway after
        yield return new WaitForSeconds(PathfindingWaitTime);
        AllCharactersAreHome = true;
    }

    
    // TODO: needs to be updated if landmarks moved or grid size increased
    private void SetLandmarkLocationsUnwalkable()
    {
        // Cherry Blossom
        SetUnwalkable(4, 1, false, true);
        // Torii Gate
        SetUnwalkable(13, 1, false, true);
        // Holy Shrine
        SetUnwalkable(0, 4, false, true);
        // Slime Statue
        SetUnwalkable(18, 4, false, true);
        // Water Well
        SetUnwalkable(5, 8, false, true);
        // Tree Swings
        SetUnwalkable(14, 8, false, true);
    }

    
    private void SetUnwalkable(int startNo, int endNo, bool walkable, bool landmark)
    {
        // uses first x and y to find other 3 landmark grid cells
        pathfinding.GetNode(startNo, endNo).SetIsWalkable(walkable);
        pathfinding.GetNode(startNo, endNo +1).SetIsWalkable(walkable);
        pathfinding.GetNode(startNo+1, endNo).SetIsWalkable(walkable);
        pathfinding.GetNode(startNo+1, endNo +1).SetIsWalkable(walkable);
        
        if (landmark == false) return;
        // setTargetPoints of landmarks
        pathfinding.GetNode(startNo, endNo-1).SetIsTargetPoint(true);
        pathfinding.GetNode(startNo+1, endNo-1).SetIsTargetPoint(true);
    }
    
    
    public void UpdateWalls()
    {
        // recalculate pathfinding for each character if wall placed and all aren't currently moving
        pathfinding.GetGrid().GetXY(mouseWorldPos, out var x, out int y);
        pathfinding.GetNode(x, y).SetIsWalkable(!otherSR.enabled);
        if (!AllCharactersNotMoving) return;
        
        Character1.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character2.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character3.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character4.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character5.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        Character6.GetComponent<CharacterPathways>().CalculatePathfinding(pathfinding);
        
        // var ShadowSr = other.transform.GetChild(1).GetComponent<SpriteRenderer>();
        // ShadowSr.enabled = otherSR.enabled;
    }


    private void CheckIfWallWillBlockPathfinding(int x, int y, Renderer otherRenderer, Vector3 mousePos)
    {
        var currentNode = pathfinding.GetNode(x, y);
        otherRenderer.enabled = !otherRenderer.enabled;

        // extra check to make sure walkable logic follows sprite renderer active state logic
        pathfinding.GetNode(x, y).SetIsWalkable(!otherRenderer.enabled);
        
        // calculate first a test path from the bottom left to the top-right
        var longPath = pathfinding.FindPath(0, 0, width-1, height-1);

        // if this test path is null perform more checks
        if (longPath == null) 
        {
            // can't place boulder here
            StartCoroutine(ResetNullLocationIndicator(mouseWorldPos));
            Debug.Log($"Placing a boulder here will block movement");
            otherRenderer.enabled = false;
            pathfinding.GetNode(x, y).SetIsWalkable(true);
        }

        // perform multiple path checks from each target location to each house before deciding if boulder can be placed
        else
            CheckAllPaths(x, y, otherRenderer);

            // once checks are done assign pathfinding
        if (!AllCharactersAreHome && DayCount!= 0)
            AssignPathfinding();
    }

    
    private void CheckAllPaths(int x, int y, Renderer otherRenderer)
    {
        // more target children locations than houses so start from them
        foreach (var targets in targetChildrenLocations)
        {
            foreach (var houses in houseLocations)
            {
                // get transform postions
                var startPos = targets.transform.position;
                var endPos = houses.transform.position;

                    
                // get grid positions for pathfinding
                pathfinding.GetGrid().GetXY(startPos, out var startX, out var startY);
                pathfinding.GetGrid().GetXY(endPos, out var endX, out var endY);

                // test all paths
                var newPath = pathfinding.FindPath(startX, startY, endX, endY);
                if (newPath != null) continue;

                // if any path is null warn player they can't place boulder here and reset walkable logic
                StartCoroutine(ResetNullLocationIndicator(mouseWorldPos));
                Debug.Log($"Placing a boulder here will block movement between {houses.name} & {targets.name}");
                otherRenderer.enabled = false;
                pathfinding.GetNode(x, y).SetIsWalkable(true);
                return;
            }
        }
    }

    
    private IEnumerator ResetNullLocationIndicator(Vector3 mousePos)
    {
        // calculations to place indicator nicely on grid (x and y values will end in closest 5)
        var mouseX = (int) Mathf.Ceil(mousePos.x/10);
        var mouseY = (int) Mathf.Ceil(mousePos.y/10);
        NullLocationIndicator.transform.position = new Vector3(((mouseX*10) -5), ((mouseY*10) -5), 0);
        
        // reset location (makes it disappear again)
        yield return new WaitForSeconds(NullLocationResetTime);
        NullLocationIndicator.transform.position = NullLocationDefaultPos;
    }
    
    
    // way of forbidding placement of boulders on the left and right side borders of the gird
    public void SideCheck(Pathnode currentNode, Vector3 mousePos)
    {
        var leftRightNeighbours = pathfinding.GetLeftRightNeighbours(currentNode);
        var left = leftRightNeighbours[0];
        var right = leftRightNeighbours[1];

        // show warnings
        if (left.x != 0 || left.y != 0) return;
        Debug.Log("Can't place walls on side borders");
        StartCoroutine(ResetNullLocationIndicator(mousePos));
    }
}
