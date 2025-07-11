using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public enum PilotStatus
    {
        CALM = 0,
        UNCONSCIOUS = 1,
        TERRIFIED = 2,
    }
    [SerializeField] private PilotStatus currentStatus;
    private Animator animator;
    //[SerializeField] private float turnFactor = 5f;
    //[SerializeField] private float turnTime = 5f;
    private float turnAmountRight = 0f;
    private float turnAmountLeft = 0f;
    //[SerializeField] private float turnAmountUp = 0f;
    //[SerializeField] private float turnAmountDown = 0f;
    [SerializeField] private float turnLimit = 20f;
    [SerializeField] private List<GameObject> missiledOnLeft = new List<GameObject>();
    [SerializeField] private List<GameObject> missiledOnRight = new List<GameObject>();
    [SerializeField] private int flareAmount = 60;
    private bool startFlaring = false;
    private int flareFireAmount = 0;
    private float flareTimer = 1.5f;
    [SerializeField] private ParticleSystem leftFlareParticles;
    [SerializeField] private ParticleSystem rightFlareParticles;
    [SerializeField] private float flareCooldown = 0.5f;
    [SerializeField] private float blackoutLimit = 100;
    [SerializeField] private float blackoutMeter = 0f;
    [SerializeField] private float blackoutCooldown = 2f;
    private float blackoutTimer = 0f;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
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
            //TODO no missiles remain 
            //TODO UI managere hbaer ver, flares yazısı kırmızı parlasın ve boş sesi oynasın   
        }
        else
        {
            startFlaring = true;
            leftFlareParticles.Play();
            rightFlareParticles.Play();
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
            case PilotStatus.TERRIFIED:
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

        animator.SetFloat("LookDirection", lookValue);
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

        //transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z - maneuverInput.x * turnFactor), turnTime, RotateMode.FastBeyond360);
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
                    Debug.Log("flares fired!");
                    flareAmount -= 2;
                    flareTimer = 0;
                    flareFireAmount++;
                    if (missiledOnLeft.Count != 0)
                    {
                        if (missiledOnLeft[0].GetComponent<Missile>().isDodgeable)
                        {
                            missiledOnLeft[0].GetComponent<Missile>().SwayFromTarget();
                            missiledOnLeft.RemoveAt(0);
                        }
                    }
                    if (missiledOnRight.Count != 0)
                    {
                        if (missiledOnRight[0].GetComponent<Missile>().isDodgeable)
                        {
                            missiledOnRight[0].GetComponent<Missile>().SwayFromTarget();
                            missiledOnRight.RemoveAt(0);
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
