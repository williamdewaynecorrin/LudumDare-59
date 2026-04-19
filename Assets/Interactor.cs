using TMPro;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform origin;
    public float castdistance = 5.0f;
    public LayerMask interactmask;
    public TMP_Text interacttext;

    private Interaction bestinteraction = null;

    void Start()
    {
        interacttext.text = "";
    }

    void Update()
    {
        if(Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, castdistance, interactmask, QueryTriggerInteraction.Collide))
        {
            Interaction interaction = hit.collider.gameObject.GetComponent<Interaction>();
            if(interaction != null)
            {
                if (!interaction.oneshot || !interaction.HasUsed)
                {
                    if (bestinteraction != null && bestinteraction != interaction)
                        bestinteraction.HideInteract(this);
                    else if (bestinteraction == null)
                    {
                        bestinteraction = interaction;
                        bestinteraction.PreviewInteract(this);
                    }
                }
            }
        }
        else
        {
            if(bestinteraction != null)
            {
                bestinteraction.HideInteract(this);
                bestinteraction = null;
            }
        }

        if(bestinteraction != null && Input.GetKeyDown(KeyCode.E))
        {
            bestinteraction.OnInteract(this);
            bestinteraction.HideInteract(this);
        }
    }
}
