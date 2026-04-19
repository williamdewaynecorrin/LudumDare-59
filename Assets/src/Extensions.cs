using UnityEngine;

public static class Extensions
{
    private const string kBaseAnimLayer = "Base Layer";

    public static Vector3 NoY(this Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    public static void PlayAnimationState(this Animator anim, string statename, string animlayer = kBaseAnimLayer, bool resetifsame = false)
    {
        int layer = anim.GetLayerIndex(animlayer);

        if(resetifsame && anim.AnimatorIsInState(statename, animlayer))
        {
            anim.Play(statename, layer, 0.0f);
        }
        else
            anim.Play(statename, layer);
    }

    public static void PlayAnimationStateTransitioned(this Animator anim, string statename, float transitiontime, string animlayer = kBaseAnimLayer)
    {
        int layer = anim.GetLayerIndex(animlayer);
        anim.CrossFade(statename, transitiontime, layer);
    }

    public static bool AnimatorIsInState(this Animator anim, string statename, string animlayer = kBaseAnimLayer)
    {
        int layer = anim.GetLayerIndex(animlayer);
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(layer);

        return info.IsName(statename);
    }

    public static Color SetAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }
}
