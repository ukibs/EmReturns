using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthMarkerController : MonoBehaviour
{
    public Image healthFront;

    [HideInInspector] public Boss1SegmentController healthController;

    private RectTransform rectTransform;    

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nearestUsableScreenPosition = Camera.main.WorldToScreenPoint(healthController.transform.position);
        //rectTransform.anchoredPosition = new Vector2((nearestUsableScreenPosition.x - 0.5f) * (Screen.width / 2), (nearestUsableScreenPosition.y - 0.5f) * (Screen.height / 2));
        rectTransform.anchoredPosition = nearestUsableScreenPosition;
    }

    public void SetHealthController(Boss1SegmentController healtController)
    {
        this.healthController = healtController;
    }

    public void UpdateValue(int currentHealht, int maxHealth)
    {
        healthFront.fillAmount = (float)currentHealht / (float)maxHealth;
    }
}
