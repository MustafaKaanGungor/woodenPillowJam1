using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float timeToReach = 10f;
    [SerializeField] private float reachTimer = 0f;
    [SerializeField] private bool isTracking = true;

    void Update()
    {
        if (isTracking)
        {
            reachTimer += Time.deltaTime;
            if (reachTimer > timeToReach)
            {
                Player.Instance.GetHit();
            }
        }
    }

    public void SwayFromTarget()
    {
        isTracking = false;
    }
}
