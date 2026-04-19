using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class UIObjectives : MonoBehaviour
{
    public CUIObjectiveEntry[] entries;

    void Awake()
    {
        foreach (CUIObjectiveEntry entry in entries)
        {
            entry.checkmark.SetActive(false);
        }
    }

    public void ItemCompleted(UIItem item)
    {
        foreach(CUIObjectiveEntry entry in entries)
        {
            if(entry.item.type == item.type)
            {
                entry.checkmark.SetActive(true);
                entry.panel.transform.localPosition += Vector3.right * 20f;
                entry.text.color = entry.text.color * 0.75f;
                break;
            }
        }
    }
}

[System.Serializable]
public class CUIObjectiveEntry
{
    public UIItem item;
    public GameObject checkmark;
    public TMP_Text text;
    public GameObject panel;
}