using UnityEngine;

public class Walls : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            if (child != null)
                child.enabled = false;
        }
    }
}
