using TMPro;
using UnityEngine;

public class MissileUI : MonoBehaviour
{
    private GameObject missile;
    [SerializeField] private TextMeshProUGUI rangeText;

    public void SetMissile(GameObject missileOrigin)
    {
        missile = missileOrigin;
    }

    void Update()
    {
        if (missile != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(missile.transform.position);
            rangeText.text = missile.GetComponent<Missile>().RemainingDistance().ToString("0.00") + "km";
        }
    }
}
