using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroy : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(1280,720,false);
    }
   void Update()
    {
        DontDestroyOnLoad(this);
    }
}
