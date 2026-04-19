using System.Collections.Generic;
using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    public Enemy prefab;
    public Item rewarditem;
    public AudioClipXT sfxrewardspawn;
    public Transform[] spawns;
    public GameObject encounterbounds;
    public AudioClipXT sfxstart;
    public AudioClip battlemusic;
    public Transform boundsfull;
    public CTimer boundsexpandtimer;

    private List<Enemy> activeenemies;
    private bool hasspawned = false;
    private Vector3 targetscale;

    void Awake()
    {
        activeenemies = new List<Enemy>();
        boundsexpandtimer.Init();
        encounterbounds.SetActive(false);
        rewarditem.gameObject.SetActive(false);
        targetscale = encounterbounds.transform.localScale;
        encounterbounds.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (boundsexpandtimer.TimerReached())
            return;

        if (hasspawned)
        {
            boundsexpandtimer.Tick(Time.deltaTime);
            encounterbounds.transform.localScale = Vector3.Lerp(Vector3.zero, targetscale, boundsexpandtimer.GetLerpValue());
        }
    }

    public void EnemyDeath(Enemy enemy)
    {
        if (activeenemies.Contains(enemy))
            activeenemies.Remove(enemy);

        if(activeenemies.Count == 0)
        {
            OnEncounterFinished(enemy.transform.position);
        }
    }

    private void OnEncounterFinished(Vector3 lastenemypos)
    {
        GameManager.Play2D(sfxrewardspawn);
        rewarditem.transform.position = lastenemypos;
        rewarditem.gameObject.SetActive(true);
        encounterbounds.SetActive(false);

        GameManager.ResumeNormalTrack();
    }

    public void SpawnEnemies()
    {
        if (hasspawned)
            return;

        foreach (Transform spawn in spawns)
        {
            Enemy enemyinst = GameObject.Instantiate(prefab, spawn.position, Quaternion.identity);
            enemyinst.SetSpawner(this);
            activeenemies.Add(enemyinst);
        }

        GameManager.Play2D(sfxstart);
        hasspawned = true;
        encounterbounds.SetActive(true);
        GameManager.PlayBattleTrack(battlemusic);
    }
}
