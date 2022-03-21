using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockHandPosition : MonoBehaviour
{
    [SerializeField] private GameObject stylusHand;
    [SerializeField] private GameObject staticHand;
    private Transform stylusHandTransform;
    private Transform staticHandTransform;

    void Start()
    {
        staticHandTransform = staticHand.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey("space")))
        {
            stylusHandTransform = stylusHand.GetComponent<Transform>();

            staticHand.transform.position = stylusHandTransform.position;
            staticHand.transform.rotation = stylusHandTransform.rotation;
            staticHand.transform.localScale = stylusHandTransform.localScale;

            stylusHand.SetActive(false);
            staticHand.SetActive(true);
        }

        
    }


}
