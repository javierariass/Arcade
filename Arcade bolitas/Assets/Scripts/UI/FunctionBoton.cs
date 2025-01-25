using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FunctionBoton : MonoBehaviour
{
    
    public void Change_Scene(int Level)
    {
        SceneManager.LoadScene(Level);
    }
}
