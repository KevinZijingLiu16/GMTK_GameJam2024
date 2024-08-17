using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player; 

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; 
            transform.position = newPosition;
        }
    }
}
