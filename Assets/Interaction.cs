using UnityEngine;

public class Interaction : MonoBehaviour
{
    public string interactiontext;
    public string alreadyusedtext;
    public AudioClipXT sfxpreview;
    public AudioClipXT sfxinteract;
    public EInteractionFunctionType function = EInteractionFunctionType.eNone;
    public EItemType iteminteraction = EItemType.eCrystals;
    public bool oneshot = true;

    private bool hasused = false;
    public bool HasUsed => hasused;

    public void OnInteract(Interactor player)
    {
        bool success = false;
        player.interacttext.text = "";

        if(!hasused)
        {
            if (sfxinteract.IsValid())
                GameManager.Play2D(sfxinteract);
        }

        if (function == EInteractionFunctionType.eDepositItem)
        {
            // -- set success to true if we have enough to deposit or w/e
            //GameManager.Player.ItemDeposited(iteminteraction);
        }
        else if(function == EInteractionFunctionType.eGetItem)
        {
            GameManager.Player.ItemPickedUp(iteminteraction);
            success = true;
        }

        if(success)
        {
            hasused = true;
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