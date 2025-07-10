using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerActions inputActions;

    public event EventHandler OnFlarePerformed;

    private void Awake()
    {
        Instance = this;

        inputActions = new PlayerActions();

        //TODO sinematik ve ana menüde action mapi değiştiren kod yaz
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Flare.performed += flare_performed;
    }

    void OnDestroy()
    {
        inputActions.Gameplay.Flare.performed -= flare_performed;
    }

    private void flare_performed(InputAction.CallbackContext context)
    {
        OnFlarePerformed?.Invoke(this, EventArgs.Empty);
    }

    public float GetLookAxis()
    {
        return inputActions.Gameplay.Look.ReadValue<float>();
    }

    public Vector2 GetManeuverVector()
    {
        Vector2 maneuverVector = inputActions.Gameplay.Maneuver.ReadValue<Vector2>();

        maneuverVector = maneuverVector.normalized;

        return maneuverVector;
    }
}
