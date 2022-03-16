using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHand : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("hand is placed at start position.");
        }
    }

}
