using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField]
    private Animator fpsarmsanim;
    [SerializeField]
    private string reloadanim = "Reload";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!fpsarmsanim.AnimatorIsInState(reloadanim))
            {
                fpsarmsanim.PlayAnimationState(reloadanim);
            }
        }
    }
}
