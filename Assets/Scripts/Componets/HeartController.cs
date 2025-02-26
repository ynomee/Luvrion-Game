using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartController : MonoBehaviour
{
    // [SerializeField] private HealthComponent _healthComponent;
    // [SerializeField] private GameObject playerObj;
    // private GameObject[] _heartContainers;
    // private Image[] _heartFills;
    // public Transform heartsParent;
    // public GameObject heartContainerPrefab;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     _healthComponent = playerObj.GetComponent<HealthComponent>();
    //     _heartContainers = new GameObject[_healthComponent.maxHealth];
    //     _heartFills = new Image[_healthComponent.maxHealth];

    //     _healthComponent.onHealthChangeCallback += UpdateHeartsHUD;
        
    //     InstantiateHeartContainers();
    //     UpdateHeartsHUD();
    // }

    // private void SetHeartContainers()
    // {
    //     for (int i = 0; i < _heartContainers.Length; i++ )
    //     {
    //         if (i < _healthComponent.maxHealth)
    //         {
    //             _heartContainers[i].SetActive(true);
    //         }
    //         else
    //         {
    //             _heartContainers[i].SetActive(false);
    //         }
    //     }
    // }

    // private void SetFilledHearts()
    // {
    //     for (int i = 0; i < _heartFills.Length; i++)
    //     {
    //         if (i < _healthComponent.Health)
    //              _heartFills[i].fillAmount = 1;
    //         else
    //             _heartFills[i].fillAmount = 0;
    //     }
    // }

    // private void InstantiateHeartContainers()
    // {
    //     for (int i = 0; i < _healthComponent.maxHealth; i++)
    //     {
    //         GameObject temp = Instantiate(heartContainerPrefab);
    //         temp.transform.SetParent(heartsParent, false);
    //         _heartContainers[i] = temp;

    //         //finding gameobject by its name
    //         _heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
    //     }
    // }

    // private void UpdateHeartsHUD()
    // {
    //     SetHeartContainers();
    //     SetFilledHearts();
    // }
    private HealthComponent _healthComponent;
    private GameObject[] _heartContainers;
    private Image[] _heartFills;
    
    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    private void Start()
    {
        FindPlayerHealthComponent();
    }

    private void FindPlayerHealthComponent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Поиск игрока по тегу
        if (player != null)
        {
            _healthComponent = player.GetComponent<HealthComponent>();
            if (_healthComponent != null)
            {
                _healthComponent.onHealthChangeCallback += UpdateHeartsHUD;
                InitializeHearts();
                UpdateHeartsHUD();
            }
            else
            {
                Debug.LogError("HealthComponent не найден у объекта игрока!");
            }
        }
        else
        {
            Debug.LogError("Игрок не найден! Убедись, что он имеет тег 'Player'.");
        }
    }

    private void InitializeHearts()
    {
        _heartContainers = new GameObject[_healthComponent.maxHealth];
        _heartFills = new Image[_healthComponent.maxHealth];

        for (int i = 0; i < _healthComponent.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab, heartsParent, false);
            _heartContainers[i] = temp;
            _heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    private void UpdateHeartsHUD()
    {
        if (_healthComponent == null) return;

        for (int i = 0; i < _heartFills.Length; i++)
        {
            _heartFills[i].fillAmount = (i < _healthComponent.Health) ? 1 : 0;
        }
    }

}

