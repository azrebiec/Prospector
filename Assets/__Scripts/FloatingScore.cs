using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//an enum to track the possible states of a FloatingScore
public enum FSState{
	idle,
	pre,
	active,
	post
}

//FloatingScore can move itself on screen following a Bezier curve
public class FloatingScore : MonoBehaviour {
	public FSState state = FSState.idle;
	[SerializeField]
	private int		_score = 0; //the score field
	public string	scoreString;

	//the score property also sets scoreString when set
	public int score {
		get{
			return(_score);
		}
		set{
			_score = value;
			scoreString = Utils.AddCommasToNumber (_score);
			GetComponent<GUIText> ().text = scoreString;
		}
	}

	public List<Vector3>	bezierPts; //bezier points for movement
	public List<float>		fontSizes; //bezier points for font scaling
	public float			timeStart = -1f;
	public float			timeDuration = 1f;
	public string			easingCurve = Easing.InOut; //uses easing in utils.cs

	//the gameobject that will receive the sendmessage when this is done moving
	public GameObject		reportFinishTo = null;

	//set up the floatingscore and movement
	//note the use of parameter defaults for etimes and etimed
	public void Init(List<Vector3> ePts, float eTimeS = 0, float eTimeD = 1){
		bezierPts = new List<Vector3> (ePts);

		if (ePts.Count == 1) { //if theres only one point
			//...then just go there
			transform.position = ePts [0];
			return;
		}

		//if etimes is the default, just start at the current time
		if(eTimeS==0) eTimeS = Time.time;
		timeStart = eTimeS;
		timeDuration = eTimeD;

		state = FSState.pre; //set it to the pre state, ready to start moving
	}

	public void FSCallback (FloatingScore fs){
		//when this callback is called by sendmessage,
		//add the score from the calling floatingscore
		score += fs.score;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if this is not moving, just return
		if(state==FSState.idle) return;

		//get u from the current time and duration
		//u ranges from 0 to 1 (usually)
		float u = (Time.time - timeStart)/timeDuration;
		//use easing class from utils to curve the u value
		float uC = Easing.Ease (u, easingCurve);
		if (u < 0) { //if u<0, then we shouldn't move yet
			state = FSState.pre;
			//move to the initial point
			transform.position = bezierPts [0];
		} else {
			if (u >= 1) { //if u>=1, we're done moving
				uC = 1; //set uC=1 so we don't overshoot
				state = FSState.post;
				if (reportFinishTo != null) { //if theres a callback gameobject
					//use sendmessage to call the fscallback method
					//with this as the parameter
					reportFinishTo.SendMessage ("FSCallback", this);
					//now that the message has been sent,
					//destroy this gameobject
					Destroy (gameObject);
				} else { //if there is nothing to callback
					//...then don't destroy this. just let it stay still
					state = FSState.idle;
				}
			} else {
				//0<=u<1, which means that this is active and moving
				state = FSState.active;
			}
			//use bezier curve to move this to the right point
			Vector3 pos = Utils.Bezier(uC, bezierPts);
			transform.position = pos;
			if (fontSizes != null && fontSizes.Count > 0) {
				//if fontsizes has values in it
				//...then adjust the fontsize of this guitext
				int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
				GetComponent<GUIText> ().fontSize = size;
			}
		}
	}
}