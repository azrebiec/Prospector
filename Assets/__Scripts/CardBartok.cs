using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//CBState includes both states for the game and to___ states for movement
public enum CBState{
	drawpile,
	toHand,
	hand,
	toTarget,
	target,
	discard,
	to,
	idle
}

//CardBartok extends Card just as CardProspector did.
public class CardBartok : Card {
	//these static fields are used to set values that will be the same
	//for all instances of CardBartok
	static public float			MOVE_DURATION = 0.5f;
	static public string		MOVE_EASING = Easing.InOut;
	static public float 		CARD_HEIGHT = 3.5f;
	static public float			CARD_WIDTH = 2f;

	public CBState 				state = CBState.drawpile;

	//fields to store info the card will use to move and rotate
	public List<Vector3>		bezierPts;
	public List<Quaternion>		bezierRots;
	public float				timeStart, timeDuration; //declares 2 fields

	public int eventualSortOrder;
	public string eventualSortLayer;

	//when the card is done moving, it will call reportFinishTo.SendMessage()
	public GameObject			reportFinishTo = null;
	public Player 				callbackPlayer = null;

	void Awake() {
		callbackPlayer = null; // Just to be sure.
	}

	//MoveTo tells the card to interpolate to a new postiion and rotation
	public void MoveTo(Vector3 ePos, Quaternion eRot){
		//make new interpolation lists for the card
		//position and rotation will each have only two points
		bezierPts = new List<Vector3>();
		bezierPts.Add (transform.localPosition); //current position
		bezierPts.Add(ePos); 					//new position
		bezierRots = new List<Quaternion>();
		bezierRots.Add (transform.rotation);	//current rotation
		bezierRots.Add(eRot);					//new rotation

		//if timestart is 0, then its set to start immediately.
		//otherwise, it starts at timestart. this way, if timestart is
		//already set, it won't be overwritten
		if (timeStart == 0) {
			timeStart = Time.time;
		}
		//timeduration always starts the same but can be altered later
		timeDuration = MOVE_DURATION;

		//setting state to either tohand or totarget will be handled by the
		//calling method
		state = CBState.to;
	}
	//this overload of MoveTo doesnt require a rotation argument
	public void MoveTo(Vector3 ePos){
		MoveTo (ePos, Quaternion.identity);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		//all the to___ states are ones where the card is interpolating
		case CBState.toHand:
		case CBState.toTarget:
		case CBState.to:
			//get u from the current time and duration
			//u ranges from 0 to 1 (usually)
			float u = (Time.time - timeStart) / timeDuration;

			//use easing class from utils to curve the u value
			float uC = Easing.Ease (u, MOVE_EASING);

			if (u < 0) { //if u<0, then we shouldn't move yet
				//stay at the initial position
				transform.localPosition = bezierPts [0];
				transform.rotation = bezierRots [0];
				return;
			} else if (u >= 1) { //if u>=1, we're finished moving
				uC = 1; //set uC=1 so we don't overshoot
				//move from the to___ state to the following state
				if (state == CBState.toHand)
					state = CBState.hand;
				if (state == CBState.toTarget)
					state = CBState.target;
				if (state == CBState.to)
					state = CBState.idle;
				//move to the final position
				transform.localPosition = bezierPts [bezierPts.Count - 1];
				transform.rotation = bezierRots [bezierPts.Count - 1];
				//reset timestart to 0 so it gets overwritten next time
				timeStart = 0;

				if (reportFinishTo != null) { //if there's a callback GameObject
					//...then use SendMessage to call the CBCallback method
					//with this as the parameter.
					reportFinishTo.SendMessage ("CBCallback", this);
					//after calling sendmessage(), reportfinishto must be set
					//to null so that the card doesn't continue to report
					//to the same GameObject every subsequent time it moves
					reportFinishTo = null;
				} else if (callbackPlayer != null) {
					// If there's a callback Player
					// then call CBCallback directly on the Player
					callbackPlayer.CBCallback(this);
					callbackPlayer = null;
				} else { //if there is nothing to callback
					//do nothing
				}
			} else { //0<=u<1 which means that this is interpolating now
				//use bezier curve to move this to the right point
				Vector3 pos = Utils.Bezier (uC, bezierPts);
				transform.localPosition = pos;
				Quaternion rotQ = Utils.Bezier(uC, bezierRots);
				transform.rotation = rotQ;

				if (u>0.5f && spriteRenderers[0].sortingOrder != eventualSortOrder) {
					// Jump to the proper sort order
					SetSortOrder(eventualSortOrder);
				}
				if (u>0.75f && spriteRenderers[0].sortingLayerName !=
					eventualSortLayer) {
					// Jump to the proper sort layer
					SetSortingLayerName(eventualSortLayer);
				}

			}
			break;
		}
	}

	// This allows the card to react to being clicked
	override public void OnMouseUpAsButton() {
		// Call the CardClicked method on the Bartok singleton
		Bartok.S.CardClicked(this);
		// Also call the base class (Card.cs) version of this method
		base.OnMouseUpAsButton();
	}
}