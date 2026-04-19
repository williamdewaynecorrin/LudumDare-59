using UnityEngine;
using static UnityEditor.Progress;

public class UIItems : MonoBehaviour
{
    public UIItem[] itementries;
    public UIObjectives uiobjectives;

    public void ObtainItem(Item item)
    {
        ObtainItem(item.type);
    }

    public void ObtainItem(EItemType itemtype)
    {
        foreach (UIItem uiitem in itementries)
        {
            if (itemtype == uiitem.type)
            {
                bool wascomplete = uiitem.ItemComplete;
                uiitem.Obtain();
                
                if(uiitem.ItemComplete && !wascomplete)
                {
                    uiobjectives.ItemCompleted(uiitem);
                }

                UIDynamicText texth = GameManager.TextPooler.Handle() as UIDynamicText;
                texth.CreateText("ACQUIRED " + uiitem.itemname, uiitem.itemcolor, new Vector3(Random.Range(-80f, -30f), Random.Range(70f, 10f), 0f), Random.Range(-10f, 10f), Vector3.one);
                texth.DisableRotation();
                break;
            }
        }
    }

    public bool FullyComplete()
    {
        foreach (UIItem uiitem in itementries)
        {
            if (!uiitem.ItemComplete)
                return false;
        }

        return true;
    }
}

public enum EItemType
{
    eCrystals = 0,
    eRocketWindow = 10,
    eMeteoriteScrap = 20,
}