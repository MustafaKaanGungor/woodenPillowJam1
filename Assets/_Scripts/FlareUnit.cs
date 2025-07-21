using UnityEngine;

public class FlareUnit : MonoBehaviour
{
    private bool isIntact = true;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 50f;

    private void Update()
    {
        if (isIntact && targetPosition != null)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector3 moveDirNormalized = (targetPosition - transform.position).normalized;
        transform.position += moveDirNormalized * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            isIntact = false;
            //TODO kill the flare
        }
    }

    public void SetTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }
}
