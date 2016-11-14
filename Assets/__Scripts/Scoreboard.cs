using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//the scoreboard class manages showing the score to the player
public class Scoreboard : MonoBehaviour {
	public static Scoreboard S; //the singleton for scoreboar 

	public GameObject		prefabFloatingScore;

	public bool __________________;
	[SerializeField]
	private int				_score = 0;
	public string			_scoreString;

	//the score property also sets the scoreString
	public int score {
		get{
			return(_score);
		}
		set{
			_score = value;
			scoreString = Utils.AddCommasToNumber (_score);
		}
	}

	//the scorestring property also sets the GUIText.text
	public string scoreString{
		get{
			return(_scoreString);
		}
		set{
			_scoreString = value;
			GetComponent<GUIText> ().text = _scoreString;
		}
	}

	void Awake(){
		S = this;
	}

	//when called by sendmessage, this adds the fs.score to the this.score
	public void FSCallback(FloatingScore fs){
		score += fs.score;
	}

	//this will instantiate a new floatingscore gameobject and initialize it
	//it also returns a pointer to the floatingscore created so that the
	//calling function can do more with it (like set fontsizes, etc.)
	public FloatingScore CreateFloatingScore(int amt, List<Vector3> pts){
		GameObject go = Instantiate (prefabFloatingScore) as GameObject;
		FloatingScore fs = go.GetComponent<FloatingScore> ();
		fs.score = amt;
		fs.reportFinishTo = this.gameObject; //set fs to call back to this
		fs.Init(pts);
		return fs;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
