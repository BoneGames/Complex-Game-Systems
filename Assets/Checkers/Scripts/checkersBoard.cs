using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{

    public class checkersBoard : MonoBehaviour
    {
        [Tooltip("Prefabs for Checker Pieces")]
        public GameObject whitePiecePrefab, blackPiecePrefab;
        [Tooltip("Where to attach the spawned pieces in the Hierarchy")]
        public Transform checkersParent;
        public Vector3 boardOffset = new Vector3(-4, 0, -4);
        public Vector3 pieceOffset = new Vector3(.5f, 0, .5f);

        void Start()
        {
            GenerateBoard();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"> Location </param>
        /// <param name="y"> Location </param>
        public void GeneratePiece(int x, int y, bool isWhite)
        {
            // Ternary operator: condition ? true result : false result
            // select prefab
            GameObject prefab = isWhite ? whitePiecePrefab : blackPiecePrefab;
            // generate instance of prefab
            GameObject clone = Instantiate(prefab, checkersParent);
            // reposition clone
            clone.transform.position = new Vector3(x, 0, y) + boardOffset + pieceOffset;
        }

        /// <summary>
        /// Clears and re-generates entire board 
        /// </summary>
        public void GenerateBoard()
        {
            // Generate white team
            for (int y = 0; y < 3; y++)
            {
                bool oddRow = y % 2 == 0;
                // loop columns
                for (int x = 0; x < 8; x+=2)
                {
                    // Generate Piece
                    GeneratePiece(oddRow ? x : x + 1, y, true);
                    //if (x % 2 == 0 && y % 2 == 0)
                    //{
                    //    GeneratePiece(x, y, true);
                    //}
                    //if (x % 2 == 1 && y % 2 == 1)
                    //{
                    //    GeneratePiece(x, y, true);
                    //}
                }
            }
            // Generate black team
            for (int y = 5; y < 8; y++)
            {
                bool oddRow = y % 2 == 0;
                // loop columns
                for (int x = 0; x < 8; x+=2)
                {
                    // Generate Piece
                    GeneratePiece(oddRow ? x : x + 1, y, false);
                    //if (x % 2 == 0 && y % 2 == 0) 
                    //{
                    //    GeneratePiece(x, y, false);
                    //}
                    //if (x % 2 == 1 && y % 2 == 1)
                    //{
                    //    GeneratePiece(x, y, false);
                    //}
                }
            }
        }
    }
}
