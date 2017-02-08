using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public bool PlayerTurn;
    public int score;
    public GameObject scoreText;
	// Use this for initialization
	void Start () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //scoreText = GameObject.Find("/PlayScreen Canvas/Score Text");
        DontDestroyOnLoad(gameObject);
        PlayerTurn = true;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void UpdateScore()
    {
        scoreText.GetComponent<Text>().text = "Score " + score;
    }
}
