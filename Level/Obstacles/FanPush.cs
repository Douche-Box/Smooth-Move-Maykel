using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanPush : MonoBehaviour
{
    [SerializeField] Transform _fanTransform;

    [SerializeField] float _pushForce;

    // Push the player in the fan direction using ExtraForce for more speed
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<CharStateMachine>().IsForced = true;
            other.GetComponentInParent<CharStateMachine>().ExtraForce = _pushForce;

            Vector3 pushdirection = _fanTransform.up;

            other.GetComponentInParent<CharStateMachine>().Rb.velocity = new Vector3(other.GetComponentInParent<CharStateMachine>().Rb.velocity.x, 0, other.GetComponentInParent<CharStateMachine>().Rb.velocity.z);
            other.GetComponentInParent<CharStateMachine>().Rb.AddForce(pushdirection *= _pushForce, ForceMode.Impulse);
        }
    }
}
