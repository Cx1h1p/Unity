using UnityEngine;
using System.Collections;

public class PartRandomizer : MonoBehaviour
{
    public PartSlot[] slots; // Part1, Part2, Part3

    string[] ids = new string[] { "RED", "BLUE", "YELLOW" };

    void OnEnable()
    {
       
        StartCoroutine(RandomizeNextFrame());
    }

    IEnumerator RandomizeNextFrame()
    {
        yield return null;

        Shuffle(ids);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                slots[i].SetPart(ids[i]);
        }

        Debug.Log("Parts randomized on panel open");
    }

    void Shuffle(string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);

            string temp = array[i];
            array[i] = array[rand];
            array[rand] = temp;
        }
    }
}