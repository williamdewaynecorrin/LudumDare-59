using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AnimationSoundBank : MonoBehaviour
{
    public CAnimationSoundBankEntry[] bank;

    private Dictionary<string, CAnimationSoundBankEntry> bankdict;

    void Awake()
    {
        bankdict = new Dictionary<string, CAnimationSoundBankEntry>();
        foreach(CAnimationSoundBankEntry entry in bank)
        {
            Assert.IsTrue(!bankdict.ContainsKey(entry.name), string.Format("AnimationSoundBank on gameObject '{0} already contains an entry named '{1}'", gameObject.name, entry.name));
            bankdict.Add(entry.name, entry);
        }
    }

    public void PlaySFX3D(string name)
    {
        GameManager.Play3D(bankdict[name].audio, transform.position);
    }

    public void PlaySFX2D(string name)
    {
        GameManager.Play2D(bankdict[name].audio);
    }
}

[System.Serializable]
public class CAnimationSoundBankEntry
{
    public string name = "";
    public AudioClipXT audio;
}