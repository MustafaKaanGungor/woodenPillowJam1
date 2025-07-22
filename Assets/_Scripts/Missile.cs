using DG.Tweening;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float timeToReach = 10f;
    [SerializeField] private float reachTimer = 0f;
    [SerializeField] private float timeToDodge = 5f;
    [SerializeField] private bool isTracking = true;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private AnimationCurve projectileCurve;
    private Vector3 startPoint;
    public bool isDodgeable = false;
    private bool isTargetingFlare = false;
    private GameObject targetFlare;
    [SerializeField] private GameObject missileUIprefab;
    private GameObject missileUI = null;
    private Vector3 target;
    [SerializeField] private float projectileMaxHeight = 100;
    private bool isRight;
    private float missileRouteXFactor = 1f;
    private float missileRouteYFactor = 1f;


    void Start()
    {
        startPoint = transform.position;
        target = Player.Instance.transform.position;
        missileUI = Instantiate(missileUIprefab, HUDManager.Instance.transform);
        missileUI.GetComponent<MissileUI>().SetMissile(gameObject);
        missileRouteXFactor = Random.Range(0.5f, 1.5f);
        missileRouteYFactor = Random.Range(0.5f, 1.5f);

    }

    void Update()
    {
        if (isTracking)
        {
            UpdatePosition();
            reachTimer += Time.deltaTime;
            if (reachTimer > timeToReach && !isTargetingFlare)
            {
                Player.Instance.GetHit();
            }
            if (reachTimer > timeToDodge)
            {
                isDodgeable = true;
            }
        }
    }

    private void UpdatePosition()
    {

        if (isTargetingFlare)
        {
            target = targetFlare.transform.position;

            Vector3 moveDirNormalized = (target - transform.position).normalized;
            transform.position += moveDirNormalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            Vector3 projectileRange = target - startPoint;
            float nextPositionZ = transform.position.z - moveSpeed * Time.deltaTime;
            float nextPositionZNormalized = (nextPositionZ - startPoint.z) / projectileRange.z;

            float nextPositionXYNormalized = projectileCurve.Evaluate(nextPositionZNormalized);
            float nextPositionXY = startPoint.y - nextPositionXYNormalized * projectileMaxHeight;


            if (isRight)
            {
                Vector3 newPosition = new Vector3(-nextPositionXY * missileRouteXFactor, -nextPositionXY * 0.7f * missileRouteYFactor, nextPositionZ);
                transform.position = newPosition;
            }
            else
            {
                Vector3 newPosition = new Vector3(nextPositionXY * missileRouteXFactor, -nextPositionXY * 0.7f * missileRouteYFactor, nextPositionZ);
                transform.position = newPosition;
            }
        }

        if (Vector3.Distance(transform.position, target) < 1f)
        {
            SwayFromTarget();
        }


    }

    public float RemainingDistance()
    {
        return timeToReach - reachTimer;
    }

    public void SetDirection(bool isRight)
    {
        this.isRight = isRight;
    }

    public void SwayFromTarget()
    {
        isTracking = false;
        missileUI.GetComponent<MissileUI>().Hide();
    }

    public void SwayToFlare(GameObject newTarget)
    {
        targetFlare = newTarget;
        isTargetingFlare = true;
    }
}
