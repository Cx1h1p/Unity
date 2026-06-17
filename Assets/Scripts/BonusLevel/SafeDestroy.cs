using UnityEngine;

public static class SafeDestroy
{
    public static void DestroyObject(GameObject obj, float delay = 0f)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == obj)
        {
            UnityEditor.Selection.activeGameObject = null;
        }
#endif

        Object.Destroy(obj, delay);
    }
}