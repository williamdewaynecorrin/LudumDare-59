using UnityEngine;

public class PlayerWeaponSM : StateMachineBehaviour
{
    public EPlayerWeaponSMState smstate = EPlayerWeaponSMState.eReloadFinished;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WeaponHandler handler = animator.transform.GetComponent<WeaponHandler>();
        if (handler != null)
        {
            switch (smstate)
            {
                case EPlayerWeaponSMState.eReloadFinished:
                    handler.OnReloadFinished();
                    break;
            }
        }
    }
}

public enum EPlayerWeaponSMState
{
    eReloadFinished = 0
}