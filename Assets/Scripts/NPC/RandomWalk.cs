using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseNPC))]
public class RandomWalk : MonoBehaviour
{
    private BaseNPC npc;
    private Vector3 directionVector;
    private Transform myTransform;
    public float speed;
    private Rigidbody2D myRigidbody;
    private AnimatorController anim;
    public Collider2D bounds;
    private bool isMoving;
    public float minMoveTime;
    public float maxMoveTime;
    private float moveTimeSeconds;
    public float minWaitTime;
    public float maxWaitTime;
    private float waitTimeSeconds;

    // Awake is called before the first frame update
    void Awake()
    {
        moveTimeSeconds = Random.Range(minMoveTime, maxMoveTime);
        waitTimeSeconds = Random.Range(minMoveTime, maxMoveTime);
        anim = GetComponent<AnimatorController>();
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody2D>();
        npc = GetComponent<BaseNPC>();
        ChangeDirection();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if(npc.GetNPCState().myState != GenericState.dead)
        {
            if(isMoving)
            {
                moveTimeSeconds -= Time.deltaTime;
                if(moveTimeSeconds <= 0)
                {
                    moveTimeSeconds = Random.Range(minMoveTime, maxMoveTime);
                    isMoving = false;
                }
                anim.SetAnimParameter("walking", isMoving);
                Move();
            }
            else
            {
                waitTimeSeconds -= Time.deltaTime;
                if(waitTimeSeconds <= 0)
                {
                    ChooseDifferentDirection();
                    isMoving = true;
                    waitTimeSeconds = Random.Range(minMoveTime, maxMoveTime);
                }
            }
        }
    }

    private void ChooseDifferentDirection()
    {
        Vector3 temp = directionVector;
        ChangeDirection();
        int loops = 0;
        while (temp == directionVector && loops < 100)
        {
            loops++;
            ChangeDirection();
        }
    }

    private void Move()
    {
        Vector3 temp = myTransform.position + directionVector * speed * Time.deltaTime;
        if (bounds.bounds.Contains(temp))
        {
            myRigidbody.MovePosition(temp);
        }
        else
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        int direction = Random.Range(0, 4);
        switch(direction)
        {
            case 0:
                // Walking to the right
                directionVector = Vector3.right;
                break;
            case 1:
                // Walking up
                directionVector = Vector3.up;
                break;
            case 2:
                // Walking Left
                directionVector = Vector3.left;
                break;
            case 3:
                // Walking down
                directionVector = Vector3.down;
                break;
            default:
                break;
        }
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        anim.SetAnimParameter("xMovement", directionVector.x);
        anim.SetAnimParameter("yMovement", directionVector.y);

        anim.ChangeAnim(new Vector2(directionVector.x, directionVector.y));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ChooseDifferentDirection();
    }
}