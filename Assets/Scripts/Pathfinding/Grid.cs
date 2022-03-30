using System;
using UnityEngine;

public class Grid<TGridObject>
{
    // variables for Grid sizing
    private int width;
    private int height;
    private float cellSize;
    private int counter = 0;
    private bool showDebug;
    
    // multidimensional array with two dimensions
    private TGridObject[,] gridArray;
    [SerializeField] private Vector3 originPos;

    // grid constructor
    public Grid(int width, int height, float cellSize, Vector3 originPos, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        // allocate cell values
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;

        // creates array
        gridArray = new TGridObject[width, height];
        
        // create gameObject holders
        var textHolder = new GameObject("Grid Text Holder", typeof(TextController)); // initiates with TextController script that disables all text objects
        var wallsHolder = new GameObject("Wall Holder", typeof(Walls)); // initiates with Walls Script that disables sprite renderer of walls
        var grassHolder = new GameObject("Grass Holder", typeof(Grass));
        
        
        // visual creation using array cycling
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                counter++;
                gridArray[x, y] = createGridObject(this, x, y);
                // add text mesh object to each grid cell
                AddTextMesh(gridArray[x, y]?.ToString(), textHolder, x, y);
                
                // add wall blocks
                AddBlock(wallsHolder, x, y);
                
                // add grass blocks
                AddGrass(grassHolder, x, y);
            }
        }
        
        // debug for drawing lines of the grid
        showDebug = GameObject.FindObjectOfType<CharacterManager>().GetComponent<CharacterManager>().showGrid;
        if (!showDebug) return;
        {
            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    // draw vertical line
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, 100f);
                    // draw horizontal line
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.white, 100f);
                }
            }
            
            // add final missing lines around grid edges
            Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);
        }
    }
    
    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
    
    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPos;
    }
    
    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPos).y / cellSize);
    }

    
    public void AddTextMesh(string gridArray, GameObject parent, int x, int y)
    {
        var gridTextObject = new GameObject($"Grid_Text{counter}", typeof(TextMesh));
        var textMesh = gridTextObject.GetComponent<TextMesh>();
        textMesh.text = gridArray;

        // text adjustables
        textMesh.color = Color.black;
        textMesh.fontSize = 40;
        textMesh.anchor = TextAnchor.MiddleCenter;
                
        // sorts each text mesh object as child of Text Holder gameObject
        gridTextObject.transform.SetParent(parent.transform);

        // assign world position of text and centre it (using half of the cell's size)
        gridTextObject.transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * .5f;
        
        // adds secondary text shows grid cell number (from count)
        AddTextValue(gridTextObject, x, y);
    }

    public void AddTextValue(GameObject parent, int x, int y)
    {
        var gridTextValueObject = new GameObject($"Value Text", typeof(TextMesh));
        var textMesh = gridTextValueObject.GetComponent<TextMesh>();
        // assigns each cell a number
        textMesh.text = counter.ToString();
        
        // text adjustables
        textMesh.color = Color.red;
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.UpperRight;
        
        // set parent and transform
        gridTextValueObject.transform.SetParent(parent.transform);
        gridTextValueObject.transform.position = parent.transform.position + new Vector3(cellSize, cellSize) * .5f;
    }
    
    private void AddBlock(GameObject parent, int x, int y)
    {
        // create empty object with SpriteRenderer
        var block = new GameObject($"Wall{counter}", typeof(SpriteRenderer));
        var sprite = block.GetComponent<SpriteRenderer>();
        
        // Assign Sprite Render the Wall Sprite
        sprite.sprite = GameObject.Find("Wall").GetComponent<SpriteRenderer>().sprite;
        sprite.color = Color.white;
        
        // Scale to cell size
        block.transform.localScale = new Vector3(cellSize, cellSize, 1);
        
        // set parent and transform
        block.transform.SetParent(parent.transform);
        block.transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * .5f;

        // set tag and add box collider for ray-casting targeting
        block.AddComponent<BoxCollider2D>();
        block.tag = "Wall";
        
        // add higher sorting layer
        sprite.sortingOrder = 2;
        //AddShadow(block, x, y);
    }

    private void AddShadow(GameObject parent, int x, int y)
    {
        // create empty object with SpriteRenderer
        var shadow = new GameObject($"Shadow", typeof(SpriteRenderer));
        var sprite = shadow.GetComponent<SpriteRenderer>();
        
        // Assign Sprite Render the Wall Sprite
        sprite.sprite = GameObject.Find("Boulder Shadow").GetComponent<SpriteRenderer>().sprite;
        sprite.color = Color.black;
        
        // Scale to cell size
        shadow.transform.localScale = GameObject.Find("Boulder Shadow").transform.localScale;
        
        // set parent and transform
        shadow.transform.SetParent(parent.transform);
        
        shadow.transform.position = GetWorldPos(x, (y)) + new Vector3(cellSize, cellSize) * .5f;
        
        shadow.GetComponent<SpriteRenderer>().enabled = false;

        // add higher sorting layer
        sprite.sortingOrder = 1;
    }

    private void AddGrass(GameObject parent, int x, int y)
    {
        // create empty object with SpriteRenderer
        var grassBlock = new GameObject($"Grass{counter}", typeof(SpriteRenderer));
        var sprite = grassBlock.GetComponent<SpriteRenderer>();
        
        // Assign Sprite Render the Grass Sprite
        sprite.sprite = GameObject.Find("Summer Grass").GetComponent<SpriteRenderer>().sprite;
        sprite.color = Color.white;
        
        // Scale to cell size + a bit extra to fill out the sprite
        grassBlock.transform.localScale = new Vector3(cellSize +2.5f, cellSize +2.5f, 1);
        
        // set parent and transform
        grassBlock.transform.SetParent(parent.transform);
        grassBlock.transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * .5f;
        
        // set tag
        grassBlock.tag = "Grass";
    }
    
    // logic for grid array constructor
    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }
    
    // set world positions for grid arrays
    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
}
