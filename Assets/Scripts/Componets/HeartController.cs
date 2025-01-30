using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    [SerializeField] private HealthComponent _healthComponent;
    private GameObject[] _heartContainers;
    private Image[] _heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _heartContainers = new GameObject[_healthComponent.maxHealth];
        _heartFills = new Image[_healthComponent.maxHealth];

        _healthComponent.onHealthChangeCallback += UpdateHeartsHUD;
        
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    private void SetHeartContainers()
    {
        for (int i = 0; i < _heartContainers.Length; i++ )
        {
            if (i < _healthComponent.maxHealth)
            {
                _heartContainers[i].SetActive(true);
            }
            else
            {
                _heartContainers[i].SetActive(false);
            }
        }
    }

    private void SetFilledHearts()
    {
        for (int i = 0; i < _heartFills.Length; i++)
        {
            if (i < _healthComponent.Health)
                 _heartFills[i].fillAmount = 1;
            else
                _heartFills[i].fillAmount = 0;
        }
    }

    private void InstantiateHeartContainers()
    {
        for (int i = 0; i < _healthComponent.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            _heartContainers[i] = temp;

            //finding gameobject by its name
            _heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    private void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
