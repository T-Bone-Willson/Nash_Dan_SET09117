﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    // Sets data for the array. Which tells us the amount of black and white pieces for the board
    public Piece[,] pieces = new Piece[8, 8];
    // Creates game objects for board. This allows me to drag and drop the relevant art to the relevant GameObject
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    // Vector3 data realigns objects within "boardOffset" method
    private Vector3 boardOffset = new Vector3(-4.0f, 0.0f, -4.0f);
    // Same as previous but for "pieceOffset" method
    private Vector3 pieceOffset = new Vector3(0.5f, 0.0f, 0.5f);
    //Look at tutorial 2, 13.05 mins in. Could give indication on how to do movement log
    private Piece selectedPiece;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector3 endDrag;

    // Use this for initialization
    private void Start()
    {
        GenerateBoard();
    }
    // updates the game every frame.
    private void Update()
    {
        UpdateMouseOver();

        //Debug.Log(mouseOver);

        // if it is my turn
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;
            // If pressed mouse button, then we select piece
            if (Input.GetMouseButtonDown(0))
                SelectPiece(x, y);
            // when release mouse button, attemps to confirm/initiate the move.
            if (Input.GetMouseButtonUp(0))
                //x and y values refer to "mouseOver" integer
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
        }
    }
    // Mouse controls
    private void UpdateMouseOver()
    {
        // My Turn
        // Check to see if main camera is aparent.
        if (!Camera.main)
        {
            Debug.Log("Unable to main Camera");
            return;
        }
        // Aligns camera to mouse click input/Point Of View
        RaycastHit pov;
        // Store value of the "mousePosition" into variable "pov"
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pov, 25.0f, LayerMask.GetMask("Board")))
        {
            //x, y and z are axis's in Vector2
            mouseOver.x = (int)(pov.point.x - boardOffset.x);
            mouseOver.y = (int)(pov.point.z - boardOffset.z);
        }
        else
        {
            //if raycast doesnt his, then equals -1. A non hit essentially.
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void SelectPiece(int x, int y)
    {
        // Determines if player is out of bounds, in relation to the range of the array
        if (x < 0 || x >= pieces.Length || y < 0 || y >= pieces.Length)
            return;

        Piece p = pieces[x, y];
        // if null, you don't recieve debug log. Means things are working.
        if (p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }
    // x1 & y1 mean start position, x2 & y2 mean end position
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        //THIS IS ONLY FOR MULTIPLAYER SUPPORT!!!! LOOK AT 17.20 MINS IN OF TUTORIAL 2!!!!
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        MovePiece(selectedPiece, x2, y2);
    }

    // Method to generate pieces on board
    private void GenerateBoard()
    {
        // Generate White team, sets at bottom two rows of the board
        for(int up2Down = 0; up2Down < 3; up2Down++)
        {
            // Determines if it's an odd row or not. This is by using a Modulu operator
            bool oddRow = (up2Down % 2 == 0);
            // Makes pieces generate from left to right, going across the bottom two rows of the board
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                // Generate our Piece by using a Ternary operator
                // if our pieve is on odd row; place. If it's not; move over by 1 and place.
                GeneratePiece((oddRow)? left2Right : left2Right +1, up2Down);
            }
        }

        // REVIEW CODE COMMENTS ABOVE ^^^ TO UNDERSTAND PROCESS. IT'S THE SAME APART FROM SOME VARIABLE VALUES

        // Generate Black team, sets at top two rows of the board
        // int "up2Down" is set at 7 because our array is 8 by 8, and our code reads from 0 to 7
        // which means it's the top row on the board.
        for (int up2Down = 7; up2Down > 4; up2Down--)
        {
            bool oddRow = (up2Down % 2 == 0);
            for (int left2Right = 0; left2Right < 8; left2Right += 2)
            {
                GeneratePiece((oddRow) ? left2Right : left2Right + 1, up2Down);
            }
        }
    }

    // Associates int "left2Right" and "up2Down" to GameObject's
    private void GeneratePiece(int left2Right, int up2Down)
    {
        // States that if int up2Down is > 3; Then it's black team. If less: White team. Use Ternary operator
        bool isPieceWhite = (up2Down > 3) ? false : true;
        // Ternary operator, if "isPieceWhite is white, then spawn "whitePiecePreFab". Else; Spawn "blackPiecePrefab"
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        // calls back to Array "pieces" that was made on line 8 and associates it to "p"
        pieces[left2Right, up2Down] = p;
        MovePiece(p, left2Right, up2Down);
    }

    // Method to place pieces in the correct quadrant of the board
    private void MovePiece(Piece p, int left2Right, int up2Down)
    {
        // Board in centre of the world, so there is offset. There's -4 on one side and 4 on the other
        // This fixes that
        p.transform.position = (Vector3.right * left2Right) + (Vector3.forward * up2Down) + boardOffset + pieceOffset;
    }
}