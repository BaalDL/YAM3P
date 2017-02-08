using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterControl : MonoBehaviour {
    private MovingObject movingObject = null;
    private GridManager gridManager = null;
    private Vector2 move = new Vector2(0f, 0f);
    private float threshold = 0.1f;
    private float gridSize = 1f;

    // Use this for initialization
    void Awake() {
        movingObject = gameObject.GetComponent<MovingObject>();
        gridManager = GridManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.PlayerTurn)
        {
            move = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                move.y = 0f;
            }
            else
            {
                move.x = 0f;
            }
            if (Mathf.Abs(move.x) > 0f || Mathf.Abs(move.y) > 0f)
            {
                move = new Vector2(System.Math.Sign(move.x) * gridSize, System.Math.Sign(move.y) * gridSize);
                Vector2 start = transform.position;
                Vector2 end = start + move;
                if (end.x < gridManager.GridWidth && end.x > -1 && end.y < gridManager.GridHeight && end.y > -1)
                {
                    gridManager.ExchangeTwo((int)start.x, (int)start.y, (int)end.x, (int)end.y, true);
                    //movingObject.Move(move);
                    //gridManager.Grid[(int)end.x, (int)end.y].GetComponent<MovingObject>().Move(new Vector2(0f, 0f) - move);
                    //GameObject temp = gridManager.Grid[(int)end.x, (int)end.y];
                    //gridManager.Grid[(int)end.x, (int)end.y] = gridManager.Grid[(int)start.x, (int)start.y];
                    //gridManager.Grid[(int)start.x, (int)start.y] = temp;
                }
            }
            
        }
    }
}
