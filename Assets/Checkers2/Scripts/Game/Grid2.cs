using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2 : MonoBehaviour {

    public GameObject redPiecePrefab, whitePiecePrefab;
    public Vector3 boardOffset = new Vector3(-4f, 0f, -4f);
    public Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
    public Piece2[,] pieces = new Piece2[8, 8];
    // For Drag and Drop
    private Vector2Int mouseOver; // Grid cordinates the mouse is over
    private Piece2 selectedPiece; // Piece that has been clicked and ragged
    void Start () {
        GenerateBoard();

    }

    Piece2 GetPiece(Vector2Int cell)
    {
        return pieces[cell.x, cell.y];
    }

    bool IsOutOfBounds(Vector2Int cell)
    {
        return cell.x < 0 || cell.x >= 8 ||
               cell.y < 0 || cell.y >= 8;
    }

    bool TryMove(Piece2 selected, Vector2Int desiredCell)
    {
        // Get the selected piece's cell
        Vector2Int startCell = selected.cell;

        // is it not a valid move?
        if(!ValidMove(selected, desiredCell))
        {
            // move back to start Pos
            MovePiece(selected, startCell);
            return false;
        }

        // Replace end coordinates with selectedPiece
        MovePiece(selected, desiredCell);
        // Valid move detected!
        return true;
    }

    // checks if start and end drag positions are valid game moves
    bool ValidMove(Piece2 selected, Vector2Int desiredCell)
    {
        // get direction of movement
        Vector2Int direction = selected.cell - desiredCell;
        #region Rule 1 - out of bounds?
        // out of bounds?
        if (IsOutOfBounds(desiredCell))
        {
            Debug.Log("<color=red>Invalid - You cannot move outside the board</color>");
            return false;
        }
        #endregion

        #region Rule 2 - selected cell same as desired cell (drop piece where it started)

        #endregion

        #region Rule 3 - is there a piece at the desired cell?

        #endregion

        #region Rule 4 - forced moves?

        #endregion

        #region Rule 5 - is the drag a 2 square drag?

        #endregion

        #region Rule 6 - is the piece moving diagonally?

        #endregion

        #region Rule 17 - is the piece moving the right directrion (forward/back)

        #endregion
        // If all the above rules dont return false, move is valid!
        Debug.Log("<color=green>Success - Valid move detected!</color>");
        return true;
    }

    Piece2 SelectPiece(Vector2Int cell)
    {
        // Check is X and Y is out of bounds
        if(IsOutOfBounds(cell))
        {
            // return result early
            return null;
        }

        // get the piece at X and Y location
        Piece2 piece = GetPiece(cell);

        // Check that it isn't null
        if(piece)
        {
            return piece;
        }

        return null;
    }
    // Updating when the pieces have been selected
    void MouseOver()
    {
        // Raycast from mouse Pos
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // If the ray hit the board
        if(Physics.Raycast(camRay, out hit))
        {
            // Convert mouse coordinates to 2D array coordinates
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver = new Vector2Int(-1, -1);
        }
    }

    void DragPiece(Piece2 selected)
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Detects mouse ray hit point
        if(Physics.Raycast(camRay, out hit))
        {
            // Updates position of selected piece to hit point + offset
            selected.transform.position = hit.point + Vector3.up;
        }
    }
    Vector3 GetWorldPosition(Vector2Int cell)
    {
        return new Vector3(cell.x, 0, cell.y) + boardOffset + pieceOffset;
    }

    void MovePiece(Piece2 piece, Vector2Int newCell)
    {
        Vector2Int oldCell = piece.cell;
        // Update array
        pieces[oldCell.x, oldCell.y] = null;
        pieces[newCell.x, newCell.y] = piece;
        // Update data on piece
        piece.oldCell = oldCell;
        piece.cell = newCell;
        // Translate the piece to another location
        piece.transform.localPosition = GetWorldPosition(newCell);
    }

    void GeneratePiece(GameObject prefab, Vector2Int desiredCell)
    {
        // Generate Instance of prefab
        GameObject clone = Instantiate(prefab, transform);
        // Get the Piece component
        Piece2 piece = clone.GetComponent<Piece2>();
        // Set the cell data for the first time
        piece.oldCell = desiredCell;
        piece.cell = desiredCell;
        // reposition clone
        MovePiece(piece, desiredCell);
    }

    void Update()
    {
        // Update mouse over info
        MouseOver();
        if(Input.GetMouseButtonDown(0))
        {
            // Try selecteidng piece
            selectedPiece = SelectPiece(mouseOver);
        }
        // If there is a selected piece
        if(selectedPiece)
        {
            // Move the piece with mouse
            DragPiece(selectedPiece);
            // If button is released
            if(Input.GetMouseButtonUp(0))
            {
                // Move piece to end position
                TryMove(selectedPiece, mouseOver);
                // release piece
                selectedPiece = null;
            }
        }
    }

    void GenerateBoard()
    {
        Vector2Int desiredCell = Vector2Int.zero;
        // Generate White team
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = y % 2 == 0;
            // Loop through columns
            for (int x = 0; x < 8;  x += 2)
            {
                desiredCell.x = oddRow ? x : x + 1;
                desiredCell.y = y;
                // Generate Piece
                GeneratePiece(whitePiecePrefab, desiredCell);
            }
        }
        // Generate Red Team
        for (int y = 5; y < 8; y++)
        {
            bool oddRow = y % 2 == 0;
            // Loop through columns
            for (int x = 0; x < 8; x += 2)
            {
                desiredCell.x = oddRow ? x : x + 1;
                desiredCell.y = y;
                // Generate Piece
                GeneratePiece(redPiecePrefab, desiredCell);
            }
        }
    }
}
