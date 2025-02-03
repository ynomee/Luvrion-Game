using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{

    private List<GameObject> _teleporter = new List<GameObject>();

    [SerializeField] private Transform _destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_teleporter.Contains(collision.gameObject)) 
            return;

        if (_destination.TryGetComponent(out PlayerTeleport destinationTeleport))
        {
            destinationTeleport._teleporter.Add(collision.gameObject);
        }

        collision.transform.position = _destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _teleporter.Remove(collision.gameObject);
    }
}
