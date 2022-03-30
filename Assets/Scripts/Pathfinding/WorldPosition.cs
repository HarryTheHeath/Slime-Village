using UnityEngine;
public class WorldPosition : MonoBehaviour
{
    // Get Mouse Pos in World with Z value of 0f
    public static Vector3 GetMouseWorldPos()
    {
        Vector3 vec = GetMouseWorldPosZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    // Uses ScreenToWorldPoint to get the current mouse position on screen
    public static Vector3 GetMouseWorldPosZ(Vector3 screenPos, Camera worldCam)
    {
        Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
