using Unity.VisualScripting;
using UnityEngine;

public class CharacterRoutine : MonoBehaviour
{
    private CharacterManager characterManager;
    private Pathfinding path;
    
    [HideInInspector] public Vector3 housePos;
    [HideInInspector] public GameObject targetPos;
    [HideInInspector] public GameObject setPosObject;
    [HideInInspector] public Vector3 setPos;
    public Color pathLineColour;
    public bool atHome;

    private SpriteRenderer sr;
    private float characterVanishTime;
    private bool firstDay;
    private float timer = 0.0f;
    
    
    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>().GetComponent<CharacterManager>();
        housePos = this.gameObject.transform.position;
        sr = gameObject.GetComponent<SpriteRenderer>();
        characterVanishTime = characterManager.CharacterVanishTime;
        firstDay = true;
    }

    
    private void Update()
    {
        // timer for having slimes disappear into houses only active when atHome and not on Game Start
        atHome = this.gameObject.transform.position == housePos;
        if (atHome && !firstDay)
            startTimer();
        else
            sr.enabled = true;
        
        // set firstDay permanently false only once
        if (!firstDay) return;
        
        if (characterManager.DayCount > 0)
            firstDay = false;
    }

    
    public void CalculateDistance()
    {
        path = characterManager.pathfinding;
        
        // get Character's X and Y coordinates
        var charPos = this.GameObject().transform.position;
        path.GetGrid().GetXY(charPos, out var charX, out var charY);
        
        // get the transform positions of the two possible target spots for the specified landmark
        var target1 = targetPos.transform.GetChild(0).gameObject;
        var target2 = targetPos.transform.GetChild(1).gameObject;
        
        // get grid cells of targets
        path.GetGrid().GetXY(target1.transform.position, out var target1X, out var target1Y);
        path.GetGrid().GetXY(target2.transform.position, out var target2X, out var target2Y);

        // draw paths to each target spot
        var path1 = path.FindPath(charX, charY, target1X, target1Y);
        var path2 = path.FindPath(charX, charY, target2X, target2Y);

        // compare size of each path
        setPos = path1.Count < path2.Count ? target1.transform.position : target2.transform.position;
        setPosObject = setPos == target2.transform.position ? target2 : target1;
    }

    
    private void startTimer()
    {
        // timer for disabling character sprites when atHome
        timer += Time.deltaTime;

        if (!(timer > characterVanishTime)) return;
        if (atHome)
            sr.enabled = false;
            
        // light way of resetting timer to 0
        timer = timer - characterVanishTime;
    }
}
