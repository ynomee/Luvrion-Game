using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator != null)
        {
            float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, animationLength);
        }
        else
        {
            Debug.LogWarning("Animator not found on " + gameObject.name);
        }
    }

}
