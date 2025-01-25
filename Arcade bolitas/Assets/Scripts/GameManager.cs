using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Declaracion de variables
    Tiles[,] Grid;

    [SerializeField]
    int sizeX;

    [SerializeField]
    int sizeY;

    [SerializeField]
    Tiles[] tilesPrefabs;

    [SerializeField]
    Tiles[] Potenciadores;

    [SerializeField]
    GameObject Fondo;

    int dragX = -1;
    int dragY = -1;
    int activeAnimations = 0;
    bool CanMove = false;
    bool fast = true;
    public float timeFall = 0.004f;
    public float moveDuration = 0.5f;
    public float fallDuration = 0.2f; 
    public AudioClip Sound;
    public AudioSource Audio;
    bool GameStart = false;
    Niveles Nivel;
    //Generacion inicial del Grid y los tiles
    void Start()
    {
        Nivel = GetComponent<Niveles>();
        //Audio = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        //Audio.clip = Sound;
        //Audio.Play();

        Grid = new Tiles[sizeX, sizeY * 2];

        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                GenerateTiles(i, j);
                Instantiate(Fondo, new Vector3(i, j), Quaternion.identity,transform);
            }
        }
        Check();
    }

   
   //Funcion que activa el movimiento
    void MoveTile(int x1, int y1, int x2, int y2, bool Retorno = false)
{
    StartCoroutine(MoveTileCoroutine(x1, y1, x2, y2,Retorno));
}

    //Animacion y ejecucion del movimiento
    IEnumerator MoveTileCoroutine(int x1, int y1, int x2, int y2, bool Retorno = false)
    {
        if (Nivel.MovementCount > 0)
        {
            Vector3 startPos1 = (Grid[x1, y1] != null) ? Grid[x1, y1].transform.position : new Vector3(x1, y1);
            Vector3 endPos1 = new Vector3(x2, y2, startPos1.z);
            Vector3 startPos2 = (Grid[x2, y2] != null) ? Grid[x2, y2].transform.position : new Vector3(x2, y2);
            Vector3 endPos2 = new Vector3(x1, y1, startPos2.z);
            float elapsedTime = 0;
            activeAnimations++;
            while (elapsedTime < moveDuration)
            {
                if (Grid[x1, y1] != null) Grid[x1, y1].transform.position = Vector3.Lerp(startPos1, endPos1, elapsedTime / moveDuration);
                if (Grid[x2, y2] != null) Grid[x2, y2].transform.position = Vector3.Lerp(startPos2, endPos2, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime; yield return null;
            }
            if (Grid[x1, y1] != null) Grid[x1, y1].transform.position = endPos1;
            if (Grid[x2, y2] != null) Grid[x2, y2].transform.position = endPos2;

            Tiles temp = Grid[x1, y1];
            Grid[x1, y1] = Grid[x2, y2];
            Grid[x2, y2] = temp;

            if (Grid[x1, y1] != null) Grid[x1, y1].ChangePosition(x1, y1);
            if (Grid[x2, y2] != null) Grid[x2, y2].ChangePosition(x2, y2);
            activeAnimations--;
            if (activeAnimations == 0)
            {
                List<Tiles> TilesToCheck = CheckHorizontalMatches();
                TilesToCheck.AddRange(CheckVerticalMatches());
                if (TilesToCheck.Count == 0 && !Retorno) MoveTile(x1, y1, x2, y2, true);
                else Nivel.ReducirMovement();
                Check();
            }
        }
    } 

    //Funcion de intercambio de tile
    public void SwapTiles(int x1, int y1, int x2, int y2) 
    {
        GameStart = true;
        fast = false;
        if (x1 == x2 && y1 == y2) return; 
        MoveTile(x1, y1, x2, y2);
    }

    public  void Drag(Tiles tile)
    {
        if (!CanMove) return;

        dragX = tile.x;
        dragY = tile.y;
    }

   public void Drop(Tiles tile)
    {
        if (!CanMove) return;
        if (dragX == -1 || dragY == -1) return;

        if (IsAdjacent(tile.x, tile.y,dragX,dragY))
        {
            SwapTiles(dragX, dragY, tile.x, tile.y);
        }
        dragX = -1;
        dragY = -1;
    }

    private bool IsAdjacent(int x1, int y1, int x2, int y2)
    {
        return (Mathf.Abs(x1 - x2) == 1 && y1 == y2) || (Mathf.Abs(y1 - y2) == 1 && x1 == x2);
    }

    //Revision de match vertical
    List<Tiles> CheckVerticalMatches()
    {
        List<Tiles> TilesToCheck = new List<Tiles>();
        List<Tiles> TilesToReturn = new List<Tiles>();
        Element element = Element.none;

        for (int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                if (Grid[i, j] == null) continue;
                if (Grid[i,j].element != element)
                {
                    if (TilesToCheck.Count >= 3)
                    {
                        TilesToReturn.AddRange(TilesToCheck);
                    }
                    TilesToCheck.Clear();
                    
                }
                TilesToCheck.Add(Grid[i, j]);
                element = Grid[i, j].element;
            }

            if (TilesToCheck.Count >= 3)
            {
                TilesToReturn.AddRange(TilesToCheck);
            }
            TilesToCheck.Clear();
        }
            return TilesToReturn;
    }

    //Revision de un match horizontal
    List<Tiles> CheckHorizontalMatches()
    {
        List<Tiles> TilesToCheck = new List<Tiles>();
        List<Tiles> TilesToReturn = new List<Tiles>();
        Element element = Element.none;

        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                if (Grid[i, j] == null) continue;
                if (Grid[i, j].element != element)
                {
                    if (TilesToCheck.Count >= 3)
                    {
                        TilesToReturn.AddRange(TilesToCheck);
                    }
                    TilesToCheck.Clear();

                }
                TilesToCheck.Add(Grid[i, j]);
                element = Grid[i, j].element;

            }
            if (TilesToCheck.Count >= 3)
            {
                TilesToReturn.AddRange(TilesToCheck);
            }
            TilesToCheck.Clear();
        }
        return TilesToReturn;
    }

    //Eliminacion y generacion de tiles en un match
    void Check()
    {
        List<Tiles> TilesToDestroy = CheckHorizontalMatches();
        TilesToDestroy.AddRange(CheckVerticalMatches());

        TilesToDestroy = TilesToDestroy.Distinct().ToList();

        bool sw = TilesToDestroy.Count == 0;

        for (int i = 0; i < TilesToDestroy.Count; i++)
        {
            if (TilesToDestroy[i] != null)
            {
                Grid[TilesToDestroy[i].x, TilesToDestroy[i].y] = null;
                if(GameStart)Nivel.ReducirRequisitos(TilesToDestroy[i].element, 1);
                Destroy(TilesToDestroy[i].gameObject);
                GenerateTiles(TilesToDestroy[i].x, TilesToDestroy[i].y + sizeY);
            }
        }

        if (!sw) StartCoroutine(Gravity());
    }

    //Funcion que genera los tiles
    private void GenerateTiles(int i, int j)
    {
        Tiles go = Instantiate(tilesPrefabs[Random.Range(0, tilesPrefabs.Length)], new Vector3(i, j), Quaternion.identity,transform) as Tiles;
        go.Tile(this, i, j);
        Grid[i, j] = go;
    }

    //Gravedad para mover tiles a una casilla nula debajo
    IEnumerator Gravity()
    {
        bool hasFallen;

        do
        {
            hasFallen = false;
            CanMove = false;

            for (int j = 1; j < sizeY * 2; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (Fall(i, j))
                    {
                        hasFallen = true;
                        yield return new WaitForSeconds(timeFall);
                    }
                }
            }

            if (!fast) yield return null;

        } while (hasFallen);

        CanMove = true;
        Check();
    }

    //Revision de tile cayendo
    bool Fall(int x, int y)
    {
        if (x < 0 || y <= 0 || x >= sizeX || y >= sizeY * 2) return false;
        if (Grid[x, y] == null) return false;
        if (y == 0 || Grid[x, y - 1] != null) return false;

        StartCoroutine(FallAnimation(Grid[x, y], x, y - 1));

        Grid[x, y - 1] = Grid[x, y];
        Grid[x, y] = null;

        if (Grid[x, y - 1] != null)
        {
            Grid[x, y - 1].ChangePosition(x, y - 1);
        }

        return true;
    }

    //Animacion de caida
    IEnumerator FallAnimation(Tiles tile, int x, int y)
    {
        Vector3 startPos = tile.transform.position;
        Vector3 endPos = new Vector3(x, y, startPos.z);
        float elapsedTime = 0;
        
        while (elapsedTime < fallDuration)
        {
            if (tile != null)
            {
                tile.transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / fallDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            else break;
        }

        if (tile != null) tile.transform.position = endPos;
    }


}
