#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class EditorSelectionCleaner
{
    static EditorSelectionCleaner()
    {
        // Clear selection when entering Play Mode so Inspector can't hold dead references
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingEditMode ||
                state == PlayModeStateChange.EnteredPlayMode)
            {
                Selection.activeObject = null;
            }
        };
    }
}
#endif