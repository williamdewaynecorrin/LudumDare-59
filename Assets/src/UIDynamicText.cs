using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIDynamicText : PooledObject
{
    [Header("Base Data")]
    public TMP_Text textobject;
    public AnimationCurve xovertime;
    public AnimationCurve yovertime;
    public AnimationCurve rotationovertime;
    public AnimationCurve scaleovertime;
    public AnimationCurve alphaovertime;

    private Vector3 startpos;
    private float startrot;
    private Vector3 startscale;

    public void CreateText(string text, Color c, Vector3 pos, float rot, Vector3 scale)
    {
        textobject.text = text;
        textobject.color = c;

        startpos = pos;
        startrot = rot;
        startscale = scale;

        SetTransformData();
    }

    public override void OnHandle()
    {
        base.OnHandle();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override void OnRecycled()
    {
        base.OnRecycled();
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        SetTransformData();
    }

    private void SetTransformData()
    {
        float ratio = Mathf.Clamp01(lifetime / owningpool.recycletime);

        float xpos = xovertime.Evaluate(ratio);
        float ypos = yovertime.Evaluate(ratio);
        float rot = rotationovertime.Evaluate(ratio);
        float scale = scaleovertime.Evaluate(ratio);
        float alpha = alphaovertime.Evaluate(ratio);

        transform.localPosition = startpos + new Vector3(xpos, ypos, 0f);
        transform.localRotation = Quaternion.AngleAxis(startrot + rot, Vector3.forward);
        transform.localScale = startscale * scale;
        textobject.color = textobject.color.SetAlpha(alpha);
    }
}