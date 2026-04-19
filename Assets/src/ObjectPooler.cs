using UnityEngine;
using System;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public Transform root;
    public PooledObject prefab;
    public int maxobjects = 20;
    public float recycletime = 10.0f;

    private List<PooledObject> activeobjects;
    private List<PooledObject> readyobjects;

    void Awake()
    {
        activeobjects = new List<PooledObject>();
        readyobjects = new List<PooledObject>();

        for (int i = 0; i < maxobjects; ++i)
        {
            PooledObject newobject = PooledObject.Instantiate(prefab, root);
            newobject.OnInitialize(this);
            newobject.gameObject.SetActive(false);
            readyobjects.Add(newobject);
        }
    }

    public PooledObject Handle()
    {
        PooledObject handle = null;

        if (readyobjects.Count > 0)
        {
            handle = readyobjects[0];
        }
        else
        {
            PooledObject longestalive = activeobjects[0];
            longestalive.RecycleSelf();

            handle = readyobjects[0];
        }

        readyobjects.RemoveAt(0);
        activeobjects.Add(handle);

        handle.OnHandle();
        return handle;
    }

    void Update()
    {
        for (int i = 0; i < activeobjects.Count; ++i)
        {
            PooledObject active = activeobjects[i];

            active.OnUpdate(Time.deltaTime);

            // -- check for recycle
            if (active.LifetimeReached())
            {
                active.RecycleSelf(false);
                readyobjects.Add(active);

                activeobjects.RemoveAt(i);
                --i;
                continue;
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < activeobjects.Count; ++i)
        {
            PooledObject active = activeobjects[i];
            active.OnFixedUpdate();
        }
    }

    public void RecycleObject(PooledObject instance)
    {
        activeobjects.Remove(instance);
        readyobjects.Add(instance);
    }
}

[System.Serializable]
public class PooledObject : MonoBehaviour
{
    protected float lifetime;
    protected ObjectPooler owningpool;

    public float CurrentLifetime => lifetime;

    public void OnInitialize(ObjectPooler parent)
    {
        owningpool = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        lifetime = 0.0f;

        OnInitialized();
    }

    public bool LifetimeReached()
    {
        return lifetime >= owningpool.recycletime;
    }

    public void RecycleSelf(bool parentrecycle = true)
    {
        lifetime = 0.0f;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        OnRecycled();

        gameObject.SetActive(false);

        if (parentrecycle)
            owningpool.RecycleObject(this);
    }

    protected virtual void OnInitialized()
    {

    }

    public virtual void OnHandle()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnUpdate(float dt)
    {
        lifetime += dt;
    }

    public virtual void OnFixedUpdate()
    {

    }

    protected virtual void OnRecycled()
    {

    }
}