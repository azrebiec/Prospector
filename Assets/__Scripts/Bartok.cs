using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bartok : MonoBehaviour {
	static public Bartok S;


	public TextAsset			deckXML;
	public TextAsset			layoutXML;
	public Vector3				layoutCenter = Vector3.zero;

	public bool _________________;

	public Deck					deck;
	public List<CardBartok>		drawPile;
	public List<CardBartok>		discardPile;

	void Awake(){
		S = this;
	}

	// Use this for initialization
	void Start () {
		deck = GetComponent<Deck> ();		//get the deck
		deck.InitDeck(deckXML.text);		//pass DeckXML to it
		Deck.Shuffle(ref deck.cards);		//this shuffles the deck
		//the ref keyword passes a reference to deck.cards, which allows
		//deck.cards to be modified by Deck.Shuffle()
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
