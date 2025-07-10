using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private Animator animator;
    [SerializeField] private float turnFactor = 5f;
    [SerializeField] private float turnTime = 5f;
    [SerializeField] private List<GameObject> missiledOnLeft = new List<GameObject>();
    [SerializeField] private List<GameObject> missiledOnRight = new List<GameObject>();


    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();

        MissileManager.Instance.OnMissileFiredLeft += missile_fired_left;
        MissileManager.Instance.OnMissileFiredRight += missile_fired_right;
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
        Look();
        //Maneuver();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (missiledOnLeft.Count != 0)
            {
                missiledOnLeft[0].GetComponent<Missile>().SwayFromTarget();
                missiledOnLeft.RemoveAt(0);
            }
            if (missiledOnRight.Count != 0)
            {
                missiledOnRight[0].GetComponent<Missile>().SwayFromTarget();
                missiledOnRight.RemoveAt(0);
            }
        }
    }

    private void Look()
    {
        float lookValue = GameInput.Instance.GetLookAxis();

        animator.SetFloat("LookDirection", lookValue);
    }

    private void Maneuver()
    {
        Vector2 maneuverInput = GameInput.Instance.GetManeuverVector();

        transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z - maneuverInput.x * turnFactor), turnTime, RotateMode.FastBeyond360);
    }

    public void GetHit()
    {
        GameManager.Instance.EndGame();
    }
}
