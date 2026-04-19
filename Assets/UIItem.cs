using TMPro;
using UnityEngine;

public class UIItem : MonoBehaviour
{
    public EItemType type = EItemType.eCrystals;
    public TMP_Text counttext;

    private int count = 0;

    void Awake()
    {
        if (counttext != null)
            counttext.text = count.ToString();

        gameObject.SetActive(false);
    }

    public void Obtain()
    {
        ++count;
        if(counttext != null)
        {
            counttext.text = count.ToString();
        }

        gameObject.SetActive(true);
    }
}
