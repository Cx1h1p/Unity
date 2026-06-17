using UnityEngine;

public class CleanupTransitionCanvas : MonoBehaviour
{
    private void Start()
    {
        GameObject transitionCanvas =
            GameObject.Find("Level2 Transition Canvas");

        if (transitionCanvas != null)
        {
            Destroy(transitionCanvas);
        }
    }
}