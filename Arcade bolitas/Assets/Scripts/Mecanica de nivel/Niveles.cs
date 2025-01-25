using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Niveles : MonoBehaviour
{
    TextMeshProUGUI Movement;
    public GameObject GridRequirement;
    public GameObject PrefabRequirement;
    public Element[] Element;
    public Sprite[] Requirement;
    public int[] CountRequirement;
    public int MovementCount;
    public GameObject[] RequirementObject;
    private void Start()
    {
        Movement = GameObject.FindGameObjectWithTag("Move").GetComponent<TextMeshProUGUI>();
        Movement.text = MovementCount.ToString();
        RequirementObject = new GameObject[Requirement.Length];
        for(int i = 0; i < Requirement.Length;i++)
        {
            RequirementObject[i] = Instantiate(PrefabRequirement, Vector3.zero, Quaternion.identity, GridRequirement.transform);
            RequirementObject[i].GetComponent<Image>().sprite = Requirement[i];
            RequirementObject[i].GetComponentInChildren<TextMeshProUGUI>().text = "X" + CountRequirement[i].ToString();
        }
    }

    public void ReducirRequisitos(Element element,int cant)
    {
        for(int i = 0; i < Element.Length; i++)
        {
            if(element == Element[i] && CountRequirement[i] > 0)
            {
                if (CountRequirement[i] - cant <= 0)
                {
                    RequirementObject[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    RequirementObject[i].GetComponent<Image>().enabled = true;
                    CountRequirement[i] = 0;
                }
                else
                {
                    CountRequirement[i] = CountRequirement[i] - cant;
                    RequirementObject[i].GetComponentInChildren<TextMeshProUGUI>().text = "X" + (CountRequirement[i]).ToString();
                }
            }
        }
    }
    public void ReducirMovement()
    {
        MovementCount--;
        Movement.text = MovementCount.ToString();
    }

}
