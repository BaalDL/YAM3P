using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour {
    public float moveTime = 0.5f;
    public float gridSize = 1f;
    public int matchType = 0;
    private float inverseMoveTime;
    // Use this for initialization
    void Start () {
        inverseMoveTime = 1f / moveTime;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool Move (Vector2 Dir) {

        //Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;
        Vector2 end = start + Dir;
        // Calculate end position based on the direction parameters passed in when calling Move.

        StartCoroutine(SmoothMove(end));
        return true;
    }

    private IEnumerator SmoothMove(Vector3 end)
    {
        GameManager.instance.PlayerTurn = false;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            transform.position = newPostion;

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        GameManager.instance.PlayerTurn = true;
    }
}
