using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exposure : MonoBehaviour
{
    //Renderer mLED_object ;

    //IEnumerator LEDFlash()
    //{
    //    Color c = mLED_object.material.color;
    //    for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
    //    {
    //        c.a = alpha;
    //        renderer.material.color = c;
    //        yield return null;
    //    }
    //}
    public Material m_Material;


    // Start is called before the first frame update
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
        //m_Material.EnableKeyword("_EMISSION");
        m_Material.DisableKeyword("_EMISSION");
        //mLED_object = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        }
    }
}
