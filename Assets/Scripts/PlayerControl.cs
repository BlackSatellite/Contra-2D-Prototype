using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;
    public float gravity;

    public float pixelSize;

    public LayerMask solid;

    private float horisontalSpeed;
    private float verticalSpeed;

    private bool keyLeft;
    private bool keyRight;
    private bool keyUp;
    private bool keyDown;
    private bool keyJump;
    private bool keyAction;

    private bool onGround;

    private Vector2 botLeft; //Coordinates of an angles
    private Vector2 botRight;
    private Vector2 topLeft;
    private Vector2 topRight;

    private Rigidbody2D myRigidBody2D;

	// Use this for initialization
	void Start () {

	}
    
	// Update is called once per frame
	void Update () {
        CalculateBounds();
        onGround = CheckCollision(botLeft, Vector2.down, pixelSize, solid) || CheckCollision(botRight, Vector2.down, pixelSize, solid);

        GetInput();
        Move();
	}

    //Key is pressed or not
    void GetInput()
    {
        keyLeft = Input.GetKey(KeyCode.LeftArrow);
        keyRight = Input.GetKey(KeyCode.RightArrow);
        keyUp = Input.GetKey(KeyCode.UpArrow);
        keyDown = Input.GetKey(KeyCode.DownArrow);
        keyJump = Input.GetKeyDown(KeyCode.Z);
        keyAction = Input.GetKey(KeyCode.X);
    }

    void Move()
    {
        if (keyLeft) horisontalSpeed = -moveSpeed * Time.deltaTime;
        if (keyRight) horisontalSpeed = moveSpeed * Time.deltaTime;
        if((!keyLeft && !keyRight) || (keyLeft && keyRight)) horisontalSpeed = 0;

        if (keyJump && onGround) 
        {
            verticalSpeed = jumpHeight;
            onGround = false;
        }

        if (!onGround) verticalSpeed -= gravity * Time.deltaTime;

        //cheking floor under the legs
        if ((verticalSpeed < 0) && (CheckCollision(botLeft, Vector2.down, Mathf.Abs(verticalSpeed), solid) || CheckCollision(botRight, Vector2.down, Mathf.Abs(verticalSpeed), solid)))
        {
            float dist1 = CheckCollisionDistance(botLeft, Vector2.down, Mathf.Abs(verticalSpeed), solid);
            float dist2 = CheckCollisionDistance(botRight, Vector2.down, Mathf.Abs(verticalSpeed), solid);
            if (dist1 <= dist2) verticalSpeed = -dist1;
            else verticalSpeed = -dist2;

            transform.position = new Vector2(transform.position.x, transform.position.y + verticalSpeed+ pixelSize/2);
            verticalSpeed = 0; 
        }

        transform.position = new Vector2(transform.position.x + horisontalSpeed, transform.position.y + verticalSpeed);
    }

    private bool CheckCollision(Vector2 raycastOrigin, Vector2 direction, float distance, LayerMask layer) 
    {
        return Physics2D.Raycast(raycastOrigin, direction, distance, layer);
    }

    private float CheckCollisionDistance(Vector2 raycastOrigin, Vector2 direction, float distance, LayerMask layer) 
    {
        int i = 0;
        while (Physics2D.Raycast(raycastOrigin, direction, distance, layer)) 
        {
            i++;

            if (distance > pixelSize) distance -= pixelSize;
            else distance = pixelSize;

            if (i > 1000) return 0;
        }
        return distance;
    }

    private void CalculateBounds() 
    {
        Bounds b = GetComponent<BoxCollider2D>().bounds;
        topLeft = new Vector2(b.min.x, b.max.y);
        topRight = new Vector2(b.max.x, b.max.y);
        botLeft = new Vector2(b.min.x, b.min.y);
        botRight = new Vector2(b.max.x, b.min.y);
    }
}
