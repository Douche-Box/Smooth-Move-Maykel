using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    [SerializeField] float _extraSpeed;

    // Speed pad that increases the speed with ExtraForce for a higher speed ceiling
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CharStateMachine>())
        {
            other.GetComponentInParent<CharStateMachine>().IsForced = true;
            other.GetComponentInParent<CharStateMachine>().ExtraForce = _extraSpeed;
        }
    }
}
