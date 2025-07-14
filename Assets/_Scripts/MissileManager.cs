using System;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    public static MissileManager Instance { get; private set; }
    public event EventHandler OnMissileFiredRight;
    public event EventHandler OnMissileFiredLeft;
    [SerializeField] private float missileFireOpportunity = 5f;
    private float fireOppotunityTimer = 0f;
    [SerializeField] private float missileFireChance = 20f;
    [SerializeField] private GameObject missileLeftPrefab;
    [SerializeField] private GameObject missileRightPrefab;
    [SerializeField] private Transform missileSpawnPoint;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        fireOppotunityTimer += Time.deltaTime;
        if (fireOppotunityTimer > missileFireOpportunity)
        {
            FireMissile();
            fireOppotunityTimer = 0;
        }
    }

    private void FireMissile()
    {
        Debug.Log("fire chance!");
        int missileChance = UnityEngine.Random.Range(0, 100);
        if (missileChance < missileFireChance)
        {
            int missileDirection = UnityEngine.Random.Range(0, 100);
            if (missileDirection < 50)
            {
                GameObject firedMissile = Instantiate(missileRightPrefab, missileSpawnPoint.position, Quaternion.identity);
                firedMissile.GetComponent<Missile>().SetDirection(true);
                OnMissileFiredRight?.Invoke(firedMissile, EventArgs.Empty);
                Debug.Log("fire from right!");
            }
            else if (missileDirection >= 50)
            {
                GameObject firedMissile = Instantiate(missileLeftPrefab, missileSpawnPoint.position, Quaternion.identity);
                firedMissile.GetComponent<Missile>().SetDirection(false);
                OnMissileFiredLeft?.Invoke(firedMissile, EventArgs.Empty);
                Debug.Log("fire from left!");
            }
            else
            {
                Debug.Log("no direction?");
            }
        }
    }
}
