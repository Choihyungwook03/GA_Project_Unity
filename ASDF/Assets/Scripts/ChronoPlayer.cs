using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 5f;
    public Material normalMat;
    public Material chronoMat;
    public float rewindTime = 3f;          
    public float recordInterval = 0.05f;   

    private Queue<Vector3> moveQueue = new Queue<Vector3>(); 
    private Stack<Vector3> positionStack = new Stack<Vector3>(); 
    private Vector3 targetPos;

    private bool isRewinding = false; 
    private bool isExecuting = false; 
    private float recordTimer = 0f;
    private Renderer rend;

    void Start()
    {
        targetPos = transform.position;
        rend = GetComponent<Renderer>();
        rend.material = normalMat;
    }

    void Update()
    {
        HandleInput();
        RecordPosition();

        if (isRewinding)
            RewindMovement();
        else if (isExecuting)
            ExecuteQueue();
        else
            SmoothMove();
    }

    void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, y, 0).normalized;

        if (dir != Vector3.zero)
            moveQueue.Enqueue(dir);

        if (Input.GetKeyDown(KeyCode.Space) && !isExecuting && !isRewinding)
            isExecuting = true;

        if (Input.GetKeyDown(KeyCode.R) && !isRewinding)
        {
            isRewinding = true;
            rend.material = chronoMat;
        }
    }

    void RecordPosition()
    {
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            positionStack.Push(transform.position);
            int maxRecords = Mathf.CeilToInt(rewindTime / recordInterval);
            while (positionStack.Count > maxRecords)
                positionStack.Pop();
            recordTimer = 0f;
        }
    }

    void ExecuteQueue()
    {
        if (moveQueue.Count > 0)
        {
            Vector3 moveDir = moveQueue.Dequeue();
            targetPos = transform.position + moveDir * speed * Time.deltaTime;
            SmoothMove();
        }
        else
        {
            isExecuting = false;
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    void RewindMovement()
    {
        if (positionStack.Count > 0)
        {
            targetPos = positionStack.Pop();
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * 2f * Time.deltaTime);
        }
        else
        {
            isRewinding = false;
            rend.material = normalMat;
        }
    }
}
