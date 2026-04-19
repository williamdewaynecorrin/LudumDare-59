using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UIItem : MonoBehaviour
{
    public EItemType type = EItemType.eCrystals;
    public string itemname = "NAME";
    public Color itemcolor = Color.blue;
    public bool hascount;
    public int totalcount;
    public TMP_Text counttext;
    public GameObject item;

    private int count = 0;

    public bool ItemComplete => hascount ? count == totalcount : item.activeInHierarchy;

    void Awake()
    {
        if (counttext != null)
            counttext.text = count.ToString();

        if(hascount)
        {
            item.SetActive(true);
            counttext.text = string.Format("0/{0}", totalcount);
        }
        else
            item.SetActive(false);
    }

    public void Obtain()
    {
        ++count;
        if(hascount)
        {
            counttext.text = string.Format("{0}/{1}",count, totalcount);
        }
        else
            item.SetActive(true);
    }
}
