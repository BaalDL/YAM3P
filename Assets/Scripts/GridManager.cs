using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Match
{
    public HashSet<Coord2> MovingObjects = new HashSet<Coord2>();
    public HashSet<Coord2> Centers = new HashSet<Coord2>();
    public int Size()
    {
        return MovingObjects.Count;
    }
    public enum MatchType
    {
        I,
        T,
        L,
        X,
        Superb
    } // Maybe Later...
    //public Coord2 Center()
    //{
    //    switch(MatchType):
    //    return MovingObjects[0];
    //}
}

public class Coord2
{
    public int x;
    public int y;
    //why do I need this, really
    public Coord2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Coord2 asCoord2 = obj as Coord2;
        if ((object)asCoord2 == null) return false;
        return (this.x == asCoord2.x) && (this.y == asCoord2.y);
    }
    public bool Equals(Coord2 obj)
    {
        if (obj == null) return false;
        return (this.x == obj.x) && (this.y == obj.y);
    }
    public override int GetHashCode()
    {
        return x ^ y;
    }
}

public class GridManager : MonoBehaviour {

    public int GridWidth;
    public int GridHeight;
    public GameObject[,] Grid;
    private List<Match> matches;
    public List<Coord2> ClearedObjects;

    public static GridManager instance;
    public GameObject PlayerCharacter;
    public GameObject[] TilePrefabs;

	// Use this for initialization
	void Start () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        this.ClearedObjects = new List<Coord2>();
        CreateGrid();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void CreateGrid()
    {
        Grid = new GameObject[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (x == 0 && y == 0)
                {
                    Grid[0, 0] = Instantiate(PlayerCharacter, new Vector2(0, 0), Quaternion.identity) as GameObject;
                }
                else
                {
                    //randomTile = Random.Range(0, TilePrefabs.Length);
                    int randomTile = PickRandomNonadjacentIndex(x, y);
                    Grid[x, y] = Instantiate(TilePrefabs[randomTile], new Vector2(x, y), Quaternion.identity) as GameObject;
                }
            }
        }
        //CheckMatchWholeBoard();
        //RemoveInitialMatches();
        //ProcessMatches(false);
    }

    public void RemoveInitialMatches()
    {

        foreach (Match m in this.matches)
        {
            foreach (Coord2 o in m.MovingObjects)
            {

            }
        }
    }

    public int PickRandomNonadjacentIndex(int x, int y)
    {
        Debug.Log(x + ", " + y);
        List<int> Indexes = Enumerable.Range(0, TilePrefabs.Length).ToList<int>();
        if (x > 0 && Grid[x - 1, y] != null)
        {
            Indexes.Remove(Grid[x - 1, y].GetComponent<MovingObject>().matchType);
        }
        if (x < GridWidth - 1 && Grid[x + 1, y] != null)
        {
            Indexes.Remove(Grid[x + 1, y].GetComponent<MovingObject>().matchType);
        }
        if (y > 0 && Grid[x, y - 1] != null)
        {
            Indexes.Remove(Grid[x, y - 1].GetComponent<MovingObject>().matchType);
        }
        if (y < GridHeight - 1 && Grid[x, y + 1] != null)
        {
            Indexes.Remove(Grid[x, y + 1].GetComponent<MovingObject>().matchType);
        }
        return Indexes[Random.Range(0, Indexes.Count)];
    }

    public void ExchangeTwo(int x1, int y1, int x2, int y2, bool check)
    {
        Grid[x1, y1].GetComponent<MovingObject>().Move(new Vector2(x2 - x1, y2 - y1));
        Grid[x2, y2].GetComponent<MovingObject>().Move(new Vector2(x1 - x2, y1 - y2));
        GameObject temp = Grid[x2, y2];
        Grid[x2, y2] = Grid[x1, y1];
        Grid[x1, y1] = temp;
        if (check)
        {
            CheckMatchWholeBoard();
            if (this.matches.Count > 0)
            {
                ProcessMatches(true);
                RefillGrids();
            }
        }
    }

    private void CheckMatchWholeBoard()
    {
        List<Match> matches = new List<Match>();
        Match singleMatch = new Match();
        bool[,] checkedAlready = new bool[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (Grid[x, y].GetComponent<MovingObject>().matchType > -1 && !checkedAlready[x, y])
                {
                    singleMatch = CheckMatchChainFromACell(x, y);
                    foreach (Coord2 cell in singleMatch.MovingObjects)
                    {
                        checkedAlready[cell.x, cell.y] = true;
                    }
                    if (singleMatch.Size() >= 3)
                    {
                        Debug.Log("There is a match of size " + singleMatch.Size() + " including (" + x + ", " + y + ")");
                        matches.Add(singleMatch);
                    }
                }
            }
        }

        this.matches = matches;
    }

    private Match CheckMatchChainFromACell(int x, int y)
    {
        Match Chain = new Match();
        Chain.MovingObjects.Add(new Coord2(x, y));
        RecursivelyCheckAdditionalMatch(ref Chain, x, y, true);
        RecursivelyCheckAdditionalMatch(ref Chain, x, y, false);
        //int length = CheckLengthOfSameCells(x, y, true, ref f, ref b);
        //if (length >= 2)
        //{
        //    for (int i = -b; i <= f; i++)
        //    {
        //        if (i != 0)
        //        {
        //            Chain.MovingObjects.Add(new Coord2(x + i, y));
        //            RecursivelyCheckAdditionalMatch(ref Chain, x + i, y, true);
        //        }
                
        //    }
        //} // Let us check just this now
        return Chain;
    }

    private void RecursivelyCheckAdditionalMatch(ref Match Chain, int x, int y, bool Horizontal)
    {
        int f = 0, b = 0;
        int length = CheckLengthOfSameCells(x, y, !Horizontal, ref f, ref b);
        if (length >= 2)
        {
            for (int i = -b; i <= f; i++)
            {
                if (i != 0)
                {
                    Chain.MovingObjects.Add(new Coord2(x + (Horizontal ? 0 : i), y + (Horizontal ? i : 0)));
                    RecursivelyCheckAdditionalMatch(ref Chain, x + (Horizontal ? 0 : i), y + (Horizontal ? i : 0), !Horizontal);
                }
            }
        }
    }

    private int CheckLengthOfSameCells(int x, int y, bool Horizontal, ref int front, ref int backward)
    {
        int length;
        for (length = 1; length < (Horizontal ? GridWidth - x : GridHeight - y); length++)
        {
            if (!CheckTwoCellsAreSame(x, y, x + (Horizontal ? length : 0), y + (Horizontal ? 0 : length)))
            {
                break;
            }
        }
        int backlength;
        for (backlength = 1; backlength < (Horizontal ? x : y); backlength++)
        {
            if (!CheckTwoCellsAreSame(x, y, x - (Horizontal ? backlength : 0), y - (Horizontal ? 0 : backlength)))
            {
                break;
            }
        }
        front = length - 1;
        backward = backlength - 1;
        return front + backward;
    }

    private bool CheckTwoCellsAreSame(int x1, int y1, int x2, int y2)
    {
        if (x1 >= GridWidth || x2 >= GridWidth || y1 >= GridHeight || y2 >= GridHeight || x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0)
        {
            return false;
        }
        if (Grid[x1, y1].GetComponent<MovingObject>().matchType == Grid[x2, y2].GetComponent<MovingObject>().matchType)
        {
            return true;
        }
        return false;
    }

    private void ProcessMatches(bool checkscore)
    {
        foreach (Match match in this.matches)
        {
            if (checkscore)
            {
                GameManager.instance.score += match.Size();
                GameManager.instance.UpdateScore();
            }
            foreach (Coord2 cell in match.MovingObjects)
            {
                Grid[cell.x, cell.y].GetComponent<SpriteRenderer>().enabled = false;
                this.ClearedObjects.Add(new Coord2(cell.x, cell.y));
            }
        }
        ApplyGravity();
    }

    private void RefillGrids()
    {
        foreach (Coord2 cell in this.ClearedObjects)
        {
            Grid[cell.x, cell.y].GetComponent<MovingObject>().matchType = PickRandomNonadjacentIndex(cell.x, cell.y);
            Grid[cell.x, cell.y].GetComponent<SpriteRenderer>().enabled = true;
        }
        ClearedObjects.Clear();
    }
    
    private void ApplyGravity()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 1; y < GridHeight; y++)
            {
                if (!Grid[x, y - 1].GetComponent<SpriteRenderer>().enabled)
                {
                    ExchangeTwo(x, y, x, y - 1, false);   
                }
            }
        }
        CheckMatchWholeBoard();
        if (this.matches.Count > 0)
        {
            ProcessMatches(true);
            RefillGrids();
        }
    }

    

}
