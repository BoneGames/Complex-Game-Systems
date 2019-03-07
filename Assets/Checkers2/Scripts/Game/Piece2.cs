using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece2 : MonoBehaviour {

    public bool isWhite, isKing;
    public Vector2Int cell, oldCell;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void King()
    {
        isKing = true;
        anim.SetTrigger("King");
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
