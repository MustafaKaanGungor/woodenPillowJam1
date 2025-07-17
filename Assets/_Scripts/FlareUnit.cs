using UnityEngine;

public class FlareUnit : MonoBehaviour
{
    private bool isIntact = true;
    private Vector3 startPoint;
    [SerializeField] private Vector3 targetPosition;
    private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve projectileCurve;
    [SerializeField] private float projectileMaxHeight = 10f;
    [SerializeField] private float flareRouteXFactor = 1f;
    [SerializeField] private float flareRouteYFactor = 1f;

    private void Start()
    {
        startPoint = transform.position;
    }
    private void Update()
    {
        if (isIntact && targetPosition != null)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector3 projectileRange = targetPosition - startPoint;
        float nextPositionZ = transform.position.z + moveSpeed * Time.deltaTime;
        float nextPositionZNormalized = (nextPositionZ - startPoint.z) / projectileRange.z;
        float nextPositionX = transform.position.x - moveSpeed * Time.deltaTime;
        float nextPositionY = transform.position.y - moveSpeed * Time.deltaTime;

        float nextPositionXYNormalized = projectileCurve.Evaluate(nextPositionZNormalized);

        float nextPositionXY = startPoint.y - nextPositionXYNormalized * projectileMaxHeight;

        Vector3 newPosition = new Vector3(-nextPositionX, -nextPositionY, nextPositionZ);
        transform.position = newPosition;
    }

    public void SetTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }
}
