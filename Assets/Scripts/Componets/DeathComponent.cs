using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBloodInstantiaction
{
    void BloodInstantiate();
}


public class DeathComponent : MonoBehaviour, IBloodInstantiaction
{
    [SerializeField] private PlayerStateList _pstate;
    [SerializeField] private GameObject _bloodSpurt;

    public IEnumerator Death()
    {
        _pstate.alive = false;
        Time.timeScale = 1f;

        BloodInstantiate();

        yield return new WaitForSeconds(0.9f);
    }

    public void BloodInstantiate()
    {
        GameObject bloodSpurtParticles = Instantiate(_bloodSpurt, transform.position, Quaternion.identity);
        Destroy(bloodSpurtParticles, 1.5f);
    }
}
