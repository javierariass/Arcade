using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{

    public Element element;

    public GameManager gameManager;

    public int x, y;


    public void Tile(GameManager Manager,int x,int y)
    {
        this.x = x;
        this.y = y;
        gameManager = Manager;
    }

    public void ChangePosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void OnMouseDown()
    {
        gameManager.Drag(this);
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonUp(0))
        {
            gameManager.Drop(this);
        }
    }


}

public enum Element
{
    Orange,
    Apple,
    Banana,
    Coconut,
    Rocket,
    Bomb,
    Boomerang,
    none

}