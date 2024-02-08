using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildColliderScript : MonoBehaviour
{
    public DoorUIScript DoorRefUI;

    private void Start()
    {
        // Get the ParentScript component from the parent GameObject
        // DoorRef = GetComponentInParent<door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Call a method in the ParentScript when the trigger event occurs
        DoorRefUI.ActivateUI("Press E to open.");
    }

    private void OnTriggerExit(Collider other)
    {
        // Call a method in the ParentScript when the trigger event occurs
        DoorRefUI.DeactivateUI();
    }
}