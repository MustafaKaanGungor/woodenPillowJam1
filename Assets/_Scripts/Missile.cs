using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float timeToReach = 10f;
    [SerializeField] private float reachTimer = 0f;
    [SerializeField] private float timeToDodge = 5f;
    [SerializeField] private bool isTracking = true;
    public bool isDodgeable = false;

    void Update()
    {
        if (isTracking)
        {
            reachTimer += Time.deltaTime;
            if (reachTimer > timeToReach)
            {
                Player.Instance.GetHit();
            }
            if (reachTimer > timeToDodge)
            {
                isDodgeable = true;
            }
        }
    }

    public void SwayFromTarget()
    {
        isTracking = false;  
    }
}
