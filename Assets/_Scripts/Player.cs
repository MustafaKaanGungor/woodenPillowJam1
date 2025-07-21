using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public enum PilotStatus
    {
        CALM = 0,
        UNCONSCIOUS = 1,
        PANIC = 2,
        SHOCK = 3
    }
    [SerializeField] private PilotStatus currentStatus;
    [SerializeField] private Animator animator;
    [SerializeField] private float edgePanAreaSize = 200;
    [SerializeField] private float turnFactor = 50f;
    [SerializeField] private float turnTime = 5f;
    private float turnAmountX = 0;
    private float turnAmountY = 0;

    private float turnAmountRight = 0f;
    private float turnAmountLeft = 0f;
    [SerializeField] private float turnAmountUp = 0f;
    [SerializeField] private float turnAmountDown = 0f;
    [SerializeField] private float turnLimit = 20f;
    [SerializeField] private List<GameObject> missiledOnLeft = new List<GameObject>();
    [SerializeField] private List<GameObject> missiledOnRight = new List<GameObject>();
    [SerializeField] private int flareAmount = 60;
    private bool startFlaring = false;
    private int flareFireAmount = 0;
    private float flareTimer = 1.5f;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private Transform flareSpawnPoint;
    [SerializeField] private Transform flareTargetRight;
    [SerializeField] private Transform flareTargetLeft;
    [SerializeField] private float flareCooldown = 0.5f;
    private int dodgeChanceSecurity = 0;
    [SerializeField] private float blackoutLimit = 100;
    [SerializeField] private float blackoutMeter = 0f;
    [SerializeField] private float blackoutCooldown = 2f;
    private float blackoutTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnFlarePerformed += flare_performed;
        MissileManager.Instance.OnMissileFiredLeft += missile_fired_left;
        MissileManager.Instance.OnMissileFiredRight += missile_fired_right;
    }

    private void flare_performed(object sender, EventArgs e)
    {
        if (flareAmount <= 0)
        {
            HUDManager.Instance.NoFlaresRemain();
        }
        else
        {
            startFlaring = true;
        }
    }

    private void missile_fired_left(object sender, EventArgs e)
    {
        missiledOnLeft.Add((GameObject)sender);
    }

    private void missile_fired_right(object sender, EventArgs e)
    {
        missiledOnRight.Add((GameObject)sender);
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case PilotStatus.CALM:
                Look();
                Maneuver(100);
                Flare();
                break;
            case PilotStatus.UNCONSCIOUS:
                //TODO blackout etkileri
                Maneuver(100 - blackoutMeter);
                if (blackoutMeter >= 70)
                {
                    currentStatus = PilotStatus.UNCONSCIOUS;
                }
                else if (blackoutMeter <= 40)
                {
                    currentStatus = PilotStatus.CALM;
                }
                break;
            case PilotStatus.PANIC:
                Look();
                Maneuver(70);
                Flare();
                break;
            default:
                break;
        }
    }

    private void Look()
    {
        float lookValue = GameInput.Instance.GetLookAxis();
        if (lookValue < edgePanAreaSize)
        {
            animator.SetBool("LookLeft", true);
            animator.SetBool("LookRight", false);
        }
        else if (lookValue > Screen.width - edgePanAreaSize)
        {
            animator.SetBool("LookRight", true);
            animator.SetBool("LookLeft", false);

        }
        else
        {
            animator.SetBool("LookRight", false);
            animator.SetBool("LookLeft", false);
        }
    }

    private void Maneuver(float effectiveness)
    {
        Vector2 maneuverInput = GameInput.Instance.GetManeuverVector();

        blackoutTimer -= Time.deltaTime;
        if (blackoutMeter > 0 && blackoutTimer <= 0)
        {
            blackoutMeter -= 0.2f;
        }
        if (maneuverInput.magnitude > 0.5 && blackoutMeter <= 100)
        {
            blackoutTimer = blackoutCooldown;
            blackoutMeter += 0.1f;
        }

        if (maneuverInput.x >= 0.5 && turnAmountRight <= turnLimit)
        {
            turnAmountRight += 0.1f;
        }
        else if (maneuverInput.x <= -0.5 && turnAmountLeft <= turnLimit)
        {
            turnAmountLeft += 0.1f;
        }
        else if (maneuverInput.x < 0.1f || maneuverInput.x > -0.1f)
        {
            turnAmountLeft = 0f;
            turnAmountRight = 0f;
        }

        //yukarı aşağı kodu önemli değil
        /*if (maneuverInput.y >= 0.5 && turnAmountUp <= turnLimit)
        {
            turnAmountUp += 0.1f;
        }
        else if (maneuverInput.y <= -0.5 && turnAmountDown <= turnLimit)
        {
            turnAmountDown += 0.1f;
        }
        else if (maneuverInput.y < 0.1f || maneuverInput.y > -0.1f)
        {
            turnAmountUp = 0f;
            turnAmountDown = 0f;
        }*/

        if (turnAmountRight >= 9 && missiledOnRight.Count > 0)
        {
            if (missiledOnRight[0].GetComponent<Missile>().isDodgeable)
            {
                missiledOnRight[0].GetComponent<Missile>().SwayFromTarget();
                missiledOnRight.RemoveAt(0);
            }

        }
        if (turnAmountLeft >= 9 && missiledOnLeft.Count > 0)
        {
            if (missiledOnLeft[0].GetComponent<Missile>().isDodgeable)
            {
                missiledOnLeft[0].GetComponent<Missile>().SwayFromTarget();
                missiledOnLeft.RemoveAt(0);
            }
        }

        turnAmountX -= maneuverInput.x;
        turnAmountX = Mathf.Clamp(turnAmountX, -60, 60);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, turnAmountX), Time.deltaTime);

        turnAmountY -= maneuverInput.y;
        turnAmountY = Mathf.Clamp(turnAmountY, -30, 30);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(turnAmountY, 0, 0), Time.deltaTime);

        if (maneuverInput.x < 0.1f || maneuverInput.x > -0.1f)
        {
            turnAmountX = Mathf.Lerp(turnAmountX, 0f, Time.deltaTime);
        }

        if (maneuverInput.y < 0.1f || maneuverInput.y > -0.1f)
        {
            turnAmountY = Mathf.Lerp(turnAmountY, 0f, Time.deltaTime);
        }
    }

    private void Flare()
    {
        if (startFlaring)
        {
            if (flareFireAmount < 3)
            {
                flareTimer += Time.deltaTime;
                if (flareTimer >= flareCooldown)
                {
                    flareAmount -= 2;
                    flareTimer = 0;
                    flareFireAmount++;
                    GameObject firedFlare = Instantiate(flarePrefab, flareSpawnPoint.position, Quaternion.identity);
                    firedFlare.GetComponent<FlareUnit>().SetTarget(flareTargetLeft.position);

                    GameObject firedFlare2 = Instantiate(flarePrefab, flareSpawnPoint.position, Quaternion.identity);
                    firedFlare2.GetComponent<FlareUnit>().SetTarget(flareTargetRight.position);
                    if (missiledOnLeft.Count != 0)
                    {
                        if (missiledOnLeft[0].GetComponent<Missile>().isDodgeable)
                        {
                            int dodgeChance = UnityEngine.Random.Range(0, 6);
                            if (dodgeChance >= 4 || dodgeChanceSecurity >= 6)
                            {
                                missiledOnLeft[0].GetComponent<Missile>().SwayToFlare(firedFlare);
                                missiledOnLeft.RemoveAt(0);
                                dodgeChanceSecurity = 0;
                            }
                            else
                            {
                                dodgeChanceSecurity++;
                            }
                            
                        }
                    }
                    if (missiledOnRight.Count != 0)
                    {
                        
                        if (missiledOnRight[0].GetComponent<Missile>().isDodgeable)
                        {
                            int dodgeChance = UnityEngine.Random.Range(0, 6);
                            if (dodgeChance >= 5 || dodgeChanceSecurity >= 6)
                            {
                                missiledOnRight[0].GetComponent<Missile>().SwayToFlare(firedFlare2);
                                missiledOnRight.RemoveAt(0);
                                dodgeChanceSecurity = 0;
                            }
                            else
                            {
                                dodgeChanceSecurity++;
                            }
                        }
                    }
                }
            }
            else
            {
                flareFireAmount = 0;
                startFlaring = false;
            }
        }
            
    }

    public void GetHit()
    {
        GameManager.Instance.EndGame();
    }

    public int GetFlareAmount()
    {
        return flareAmount;
    }
}
