using UnityEngine;

public class Interaction : MonoBehaviour
{
    public string interactiontext;
    public string alreadyusedtext;
    public AudioClipXT sfxpreview;
    public AudioClipXT sfxinteract;
    public AudioClipXT sfxfail;
    public EInteractionFunctionType function = EInteractionFunctionType.eNone;
    public EItemType iteminteraction = EItemType.eCrystals;
    public bool oneshot = true;

    private bool hasused = false;
    public bool HasUsed => hasused;

    public void OnInteract(Interactor player)
    {
        bool success = false;
        player.interacttext.text = "";

        if (function == EInteractionFunctionType.eDepositItem)
        {
            if(GameManager.Player.uiitems.FullyComplete())
            {
                GameManager.WinGame();
                success = true;
            }
            else
            {
                UIDynamicText texth = GameManager.TextPooler.Handle() as UIDynamicText;
                texth.CreateText("MISSING ITEMS", Color.red, new Vector3(Random.Range(-80f, -30f), Random.Range(70f, 10f), 0f), Random.Range(-10f, 10f), Vector3.one);
                texth.DisableRotation();
            }
        }
        else if(function == EInteractionFunctionType.eGetItem)
        {
            GameManager.Player.ItemPickedUp(iteminteraction);
            success = true;
        }

        if(success)
        {
            if (!hasused)
            {
                if (sfxinteract.IsValid())
                    GameManager.Play2D(sfxinteract);
            }

            hasused = true;
        }
        else
        {
            if (sfxfail.IsValid())
                GameManager.Play2D(sfxfail);
        }
    }

    public void PreviewInteract(Interactor player)
    {
        if(hasused)
            player.interacttext.text = alreadyusedtext;
        else
            player.interacttext.text = interactiontext;

        if (sfxpreview.IsValid())
            GameManager.Play2D(sfxpreview);
    }

    public void HideInteract(Interactor player)
    {
        player.interacttext.text = "";
    }
}

public enum EInteractionFunctionType
{
    eNone = 0,
    eDepositItem = 1,
    eGetItem = 2
}