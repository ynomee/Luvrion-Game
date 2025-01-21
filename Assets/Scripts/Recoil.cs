using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement _pm;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Recoil")]
    [SerializeField] private int _recoilXSteps = 5;
    [SerializeField] private int _recoilYSteps = 5;

    [SerializeField] private float _recoilXSpeed = 100;
    [SerializeField] private float _recoilYSpeed = 100;

    private bool _recoilingX, _recoilingY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleRecoil()
    {
        if (_recoilingX)
        {
            if (_pm.IsFacingRight)
            {
                _rb.velocity = new Vector2(-_recoilXSpeed, 0);
            }
            else
            {
                _rb.velocity = new Vector2(_recoilXSpeed, 0);
            }
        }

        if (_recoilingY) 
        {

        }
    }
}
