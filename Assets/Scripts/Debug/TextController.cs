using UnityEngine;
public class TextController : MonoBehaviour
{
    public bool ShowText = false;
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
                // only show Text if enabled in inspector
                if (ShowText == false)
                    child.SetActive(false);
        }
    }
}
