//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PauseTst : MonoBehaviour
//{
//    //key board control
//    bool gameIsPaused = false;

//    IEnumerator Example()
//    {
//        if (!Input.GetKey("right") && !Input.GetKey("left"))
//        {
//            Debug.Log(" press right or left keys");
//            gameIsPaused = true;
//            PauseGame();
//        }

//        Time.timeScale = 0f;
//        Debug.Log("Waiting for princess to be rescued...");
//        yield return new WaitUntil(() => Input.GetKey("right"));
//        Debug.Log("Princess was rescued!");
//    }

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        StartCoroutine(Example());
//        //Debug.Log("After coroutine ");
//    }
//}
