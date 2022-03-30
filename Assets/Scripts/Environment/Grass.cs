using UnityEngine;

public class Grass : MonoBehaviour
{
    public Sprite SpringGrass;
    public Sprite SummerGrass;
    public Sprite AutumnGrass;
    public Sprite WinterGrass;

    private int count;

    private void Start()
    {
        SpringGrass = GameObject.Find("Spring Grass").GetComponent<SpriteRenderer>().sprite;
        SummerGrass = GameObject.Find("Summer Grass").GetComponent<SpriteRenderer>().sprite;
        AutumnGrass = GameObject.Find("Autumn Grass").GetComponent<SpriteRenderer>().sprite;
        WinterGrass = GameObject.Find("Winter Grass").GetComponent<SpriteRenderer>().sprite;
        count = 1;
    }

    public void ChangeSeason()
    {
        count++;

        if (count > 4)
            count = 1;

        switch (count)
        {
            case 1:
                ChangeGrass(SummerGrass);
                Debug.Log($"Season: Summer");
                break;
            case 2:
                ChangeGrass(AutumnGrass);
                Debug.Log($"Season: Autumn");
                break;
            case 3:
                ChangeGrass(WinterGrass);
                Debug.Log($"Season: Winter");
                break;
            case 4:
                ChangeGrass(SpringGrass);
                Debug.Log($"Season: Spring");
                break;
        }
    }

    private void ChangeGrass(Sprite GrassSprite)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            child.sprite = GrassSprite;
        }
    }
}
