using UnityEngine;

public class UIItems : MonoBehaviour
{
    public UIItem[] itementries;

    public void ObtainItem(Item item)
    {
        foreach(UIItem uiitem in itementries)
        {
            if(item.type == uiitem.type)
            {
                uiitem.Obtain();
                break;
            }
        }
    }
}

public enum EItemType
{
    eCrystals = 0,
    eRocketWindow = 10
}