using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StarterDown/Item", fileName = "NewItem")]
public class Item : ScriptableObject
{
    public string id; // unique id, za: "wire", "connector", "tool", "complete_cable"
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;
}
