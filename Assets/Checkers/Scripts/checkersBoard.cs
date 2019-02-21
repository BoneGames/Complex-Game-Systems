using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class CheckersBoard : MonoBehaviour
    {
        [Tooltip("Prefabs for Checker Pieces")]
        public GameObject whitePiecePrefab, blackPiecePrefab;
        [Tooltip("Where to attach the spawned pieces in the Hierarchy")]
        public Transform checkersParent;
        public Vector3 boardOffset = new Vector3(-4.0f, 0.0f, -4.0f);
        public Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
        public float rayDistance = 1000f;
        public LayerMask hitLayers;

        public Piece[,] pieces = new Piece[8, 8];

        /*
         * isHost = Is the player currently the host? (for networking)
         * isWhiteTurn = Is it current player's turn or opponent?
         * hasKilled = Did the player's piece get killed?
         */
        private bool isWhiteTurn = true, hasKilled;
        private Vector2 mouseOver, startDrag, endDrag;

        private Piece selectedPiece = null;

        private void Start()
        {
            GenerateBoard();
        }
        private void Update()
        {
            // Update the mouse over information
            MouseOver();
            // Is it currently white's turn?
            if (isWhiteTurn)
            {
                // Get x and y coordinate of selected mouse over
                int x = (int)mouseOver.x;
                int y = (int)mouseOver.y;
                // If the mouse is pressed
                if (Input.GetMouseButtonDown(0))
                {
                    // Try selecting piece
                    selectedPiece = SelectPiece(x, y);
                    startDrag = new Vector2(x, y);
                }
                // If there is a selected piece
                if (selectedPiece)
                {
                    // Move the piece with Mouse
                    DragPiece(selectedPiece);
                }
                // If button is released
                if (Input.GetMouseButtonUp(0))
                {
                    endDrag = new Vector2(x, y); // Record end drag
                    TryMove(startDrag, endDrag); // Try moving the piece
                    selectedPiece = null; // Let go of the piece
                }
            }
        }

        public void GeneratePiece(int x, int y, bool isWhite) // Generates a Checker Piece in specified coordinates
        {
            // What prefab are we using (white or black) ?
            GameObject prefab = isWhite ? whitePiecePrefab : blackPiecePrefab;
            // Generate Instance of prefab
            GameObject clone = Instantiate(prefab, checkersParent);
            // Get the Piece component
            Piece p = clone.GetComponent<Piece>();
            // Update Piece X & Y with Current Location
            p.x = x;
            p.y = y;
            // Reposition clone
            MovePiece(p, x, y);
        }

        public void GenerateBoard() // Clears and re-generates entire board 
        {
            // Generate White Team
            for (int y = 0; y < 3; y++)
            {
                bool oddRow = y % 2 == 0;
                // Loop through columns
                for (int x = 0; x < 8; x += 2)
                {
                    // Generate Piece
                    GeneratePiece(oddRow ? x : x + 1, y, true);
                }
            }
            // Generate Black Team
            for (int y = 5; y < 8; y++)
            {
                bool oddRow = y % 2 == 0;
                // Loop through columns
                for (int x = 0; x < 8; x += 2)
                {
                    // Generate Piece
                    GeneratePiece(oddRow ? x : x + 1, y, false);
                }
            }
        }

        private Piece SelectPiece(int x, int y) // Selects a piece on the 2D grid and returns it
        {
            // Check if X and Y is out of bounds
            if (OutOfBounds(x, y))
                // Return result early
                return null;

            // Get the piece at X and Y location
            Piece piece = pieces[x, y];

            // Check that it is't null
            if (piece)
                return piece;

            return null;
        }

        private void MovePiece(Piece p, int x, int y) // Moves a Piece to another coordinate on a 2D grid
        {
            // Update array
            pieces[p.x, p.y] = null;
            pieces[x, y] = p;
            p.x = x;
            p.y = y;
            // Translate the piece to another location
            p.transform.localPosition = new Vector3(x, 0, y) + boardOffset + pieceOffset;
        }

        private void MouseOver() // Updates when the pieces have been selected
        {
            // Perform Raycast from mouse position
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // If the ray hit the board
            if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
            {
                // Convert mouse coordinates to 2D array coordinates
                mouseOver.x = (int)(hit.point.x - boardOffset.x);
                mouseOver.y = (int)(hit.point.z - boardOffset.z);
            }
            else // Otherwise
            {
                // Default to error (-1)
                mouseOver.x = -1;
                mouseOver.y = -1;
            }
        }

        private void DragPiece(Piece selected) // Drags the selected piece using Raycast location
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Detects mouse ray hit point
            if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
            {
                // Updates position of selected piece to hit point + offset
                selected.transform.position = hit.point + Vector3.up;
            }
        }

        // Tries moving a piece from Current (x1 + y1) to Desired (x2 + y2) coordinates
        private void TryMove(Vector2 start, Vector2 end)
        {
            int x1 = (int)start.x;
            int y1 = (int)start.y;
            int x2 = (int)end.x;
            int y2 = (int)end.y;

            // Record start Drag & end Drag
            startDrag = new Vector2(x1, y1);
            endDrag = new Vector2(x2, y2);

            // If there is a selected piece
            if (selectedPiece)
            {
                // Check if desired location is Out of Bounds
                if (OutOfBounds(x2, y2))
                {
                    // Move it back to original (start)
                    MovePiece(selectedPiece, x1, y1);
                    return; // Exit function!
                }

                // Check if it is a Valid Move
                if (ValidMove(start, end))
                {
                    //  Replace end coordinates with our selected piece
                    MovePiece(selectedPiece, x2, y2);
                }
                else
                {
                    // Move it back to original (start)
                    MovePiece(selectedPiece, x1, y1);
                }

                EndTurn();
            }
        }


        // checks if given co-ords are outside of board
        private bool OutOfBounds(int x, int y) 
        {
            return x < 0 || x >= 8 || y < 0 || y >= 8;
        }

        private bool ValidMove(Vector2 start, Vector2 end)
        {
            int x1 = (int)start.x;
            int y1 = (int)start.y;
            int x2 = (int)end.x;
            int y2 = (int)end.y;

            // RULE #1: Is the start the same as the end?
            if (start == end)
            {
                // You can move back where you were
                return true;
            }

            // RULE #2: If you are moving on top of another piece
            if (pieces[x2, y2])
            {
                // YA CAN'T DO DAT!
                return false;
            }

            // RULE #3: MOVING DIAGONAL FORWARDS OR BACKWARDS?
            int startCombined = int.Parse((x1.ToString() + y1.ToString()));
            int endCombined = int.Parse((x2.ToString() + y2.ToString()));

            int diagLeft = 9; // end - start = this when black moves diagonal left
            int diagRight = -11; // end - start = this when black moves diagonal right

            if (selectedPiece.isWhite) // reverse diagonal values for opposite direction
            {
                diagLeft = -9;
                diagRight = 11;
            } 


            //bool diagLef = sele
           // do the bool set condition that is in CheckForKing() for the diag left and right


            if(selectedPiece.isKing) // reverse diagonal values for opposite direction
            {
                diagLeft *= -1;
                diagRight *= -1;
            }

            if ((endCombined - startCombined) == (diagRight*2) || (endCombined - startCombined) == (diagLeft*2)) // if not moving forward
            {
                // Get the piece in between move
                Piece pieceBetween = GetPieceBetween(start, end);
                // If there is a piece between AND the piece isn't the same color
                if (pieceBetween == null || pieceBetween.isWhite == selectedPiece.isWhite)
                {
                    return false;
                }
                RemovePiece(pieceBetween);
            }

            else if ((endCombined - startCombined) != diagRight && (endCombined - startCombined) != diagLeft) // if not moving forward
            {
                return false;
            }          

            // Yeah... Alright, you can do dat.
            return true;
        }

        //calculates piece coordinates of in between piece
        private Piece GetPieceBetween(Vector2 start, Vector2 end)
        {
            int xIndex = (int)(start.x + end.x) / 2;
            int yindex = (int)(start.y + end.y) / 2;
            return pieces[xIndex, yindex];
        }

        private void RemovePiece(Piece pieceToRemove)
        {
            // Remove it from the array
            pieces[pieceToRemove.x, pieceToRemove.y] = null;
            // Destroy the gameobject of the piece immediately
            DestroyImmediate(pieceToRemove.gameObject);
        }

        private void EndTurn()
        {
            // Check if a piece needs to be kinged
            CheckForKing();
        }

        void CheckForKing()
        {
            // Get the end drag locations
            int x = (int)endDrag.x;
            int y = (int)endDrag.y;
            // Check if the selected piece is not kinged
            if (!selectedPiece.isKing)
            {
                bool whiteNeedsKing = selectedPiece.isWhite && y == 7;
                bool blackNeedsKing = !selectedPiece.isWhite && y == 0;
                // If the selected piece is white and reached the end of the board
                if (whiteNeedsKing || blackNeedsKing)
                {
                    // The selected piece is kinged!
                    selectedPiece.isKing = true;
                    // Run animations
                }
            }
        }
    }
}