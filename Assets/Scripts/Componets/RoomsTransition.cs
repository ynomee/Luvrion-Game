using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsTransition : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [SerializeField] private float _exitTime;

    [SerializeField] private PlayerMovement _pm;
    [SerializeField] private PlayerStateList _pState;
    [SerializeField] private Rigidbody2D _rb;
    public PlayerData Data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _pState.cutScene = true;
            _pm.enabled = false;
            _rb.velocity = Vector2.zero;

            Transform targetPoint = Vector2.Distance(other.transform.position, _startPoint.position) <
                                    Vector2.Distance(other.transform.position, _endPoint.position) ?
                                    _endPoint : _startPoint;

            Vector2 moveDir = (targetPoint.position - other.transform.position).normalized;

            StartCoroutine(WalkIntoNewRoom(moveDir, _exitTime, targetPoint.position));
        }
    }

    public IEnumerator WalkIntoNewRoom(Vector2 direction, float delay, Vector3 newPos)
    {
        _rb.velocity = Vector2.zero;
        _rb.transform.position = newPos;

        if (direction.y != 0)
            _rb.velocity = Data.jumpForce * new Vector2(0, direction.y);

        if (direction.x != 0)
            _rb.velocity = new Vector2(Data.runMaxSpeed * Mathf.Sign(direction.x), _rb.velocity.y);

        yield return new WaitForSeconds(delay);

        _pState.cutScene = false;
        _pm.enabled = true;
    }
}
