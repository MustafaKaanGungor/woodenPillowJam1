using UnityEngine;

public class PilotHUD : MonoBehaviour
{
    private float turnAmountX;
    private float parentTurnAmountX;

    private float turnAmountY;
    [SerializeField] private GameObject verticleGuides;
    [SerializeField] private GameObject verticleParent;
    [SerializeField] private float verticalSpeed = 50f;
    private float inputSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 maneuverInput = GameInput.Instance.GetManeuverVector();
        turnAmountX -= maneuverInput.x;
        turnAmountX = Mathf.Clamp(turnAmountX, -40, 40);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, turnAmountX), Time.deltaTime);

        parentTurnAmountX -= maneuverInput.x;
        parentTurnAmountX = Mathf.Clamp(parentTurnAmountX, -30, 30);
        verticleParent.transform.rotation = Quaternion.Lerp(verticleParent.transform.rotation, Quaternion.Euler(0, 0, -parentTurnAmountX), Time.deltaTime * 2);


        turnAmountY -= maneuverInput.y * inputSpeed;
        turnAmountY = Mathf.Clamp(turnAmountY, -250, 250);
        verticleGuides.transform.localPosition = Vector3.Lerp(verticleGuides.transform.localPosition, new Vector3(verticleGuides.transform.localPosition.x,
        turnAmountY , verticleGuides.transform.localPosition.z), Time.deltaTime * verticalSpeed);




        if (maneuverInput.x < 0.1f || maneuverInput.x > -0.1f)
        {
            turnAmountX = Mathf.Lerp(turnAmountX, 0f, Time.deltaTime);
            parentTurnAmountX = Mathf.Lerp(turnAmountX, 0f, Time.deltaTime);
        }

        
        if (maneuverInput.y < 0.1f || maneuverInput.y > -0.1f)
        {
            turnAmountY = Mathf.Lerp(turnAmountY, 0f, Time.deltaTime);
        }
        
    }
}
