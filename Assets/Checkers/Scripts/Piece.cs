using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class Piece : MonoBehaviour
    {
        // check if piece is White and/or king
        public bool isWhite, isKing;
        public int x, y;
        private Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void King()
        {
            isKing = true;
            anim.SetTrigger("King");
        }
    } 
}