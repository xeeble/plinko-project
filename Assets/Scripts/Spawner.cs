using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DestroyMode { All, Bucket, Grounded, Falling, OutOfBucket};
public class Spawner : MonoBehaviour
{
    public Slider slider;
    public int ballsRemaining { get; private set; }

    [SerializeField] int SpawnAmount;
    private Dictionary<int, GameObject> BallSpawnObjectsDict = new Dictionary<int, GameObject>();
    [SerializeField]private float SpawnTimer = 1f;
    [SerializeField]private Queue<int> SpawnQueue = new Queue<int>();
    [SerializeField]private int StartingBalls = 100;
    private Queue<int> CombineQueue = new Queue<int>();
    private int FirstBallID = 1;
    private bool canSpawn = true;

    private void OnEnable()
    {
        //Register for Events
        EventsHandler.OnBallCombined.AddListener(HandleBallCombined);
        EventsHandler.OnBoardReset.AddListener(HandleBoardReset);
        UpdateBallsRemaining();
    }

    private void HandleBoardReset()
    {
        DestroyBalls(DestroyMode.All, false);
        StartingBalls = 100;
        SpawnQueue.Clear();
        CombineQueue.Clear();
        canSpawn = true;
    }

    private void HandleBallCombined(int arg0)
    {
        if (arg0 <= BallSpawnObjectsDict.Count)
        {
            CombineQueue.Enqueue(arg0+1);
            AddBonusBalls(arg0);
        } 
    }

    private void AddBonusBalls(int arg0)
    {
        Queue<int> bonusBalls = new Queue<int>();
        for (int i = arg0; i > 1; i--)
        {
            for (int b = 0; b < CalcBonusBallAmount(i - 1); b++)
            {
                bonusBalls.Enqueue(i - 1);
                Debug.Log("BallID: " + (i - 1));
            }
        }
        
        Debug.Log("bonus ball queue length: " + bonusBalls.Count);
        EnqueueBalls(bonusBalls);
    }

    private int CalcBonusBallAmount(int arg0)
    {
        int bonusMultiplier = 5;
        Debug.Log("bonus ball amount: "+arg0 * bonusMultiplier);
        return arg0 * bonusMultiplier;
    }

    private Queue<int> CleanQueue(Queue<int> queue)
    {
        Queue<int> CleanedQueue = new Queue<int>();
        if (queue.TryDequeue(out int result))
        {
            CleanedQueue.Enqueue(result);
        }
        
        queue.TryPeek(out int Combineball);
        CleanedQueue.TryPeek(out int CleanBall);
       
        if (Combineball == CleanBall)
        {
            queue.TryDequeue(out int dq);
        }
        if (queue.Count > 0)
        {
            CleanQueue(queue);
        }

        EventsHandler.OnBallsCombinedClean.Invoke(CleanedQueue);
        return CleanedQueue;
    }

    private void EnqueueCleanBalls(Queue<int> Queue)
    {
        foreach (var ball in CleanQueue(Queue))
        {
            SpawnQueue.Enqueue(ball);
        } 
    }

    private void EnqueueBalls(Queue<int> Queue)
    {
        foreach (var ball in Queue)
        {
            SpawnQueue.Enqueue(ball);
        }
    }

    private void EnqueueBalls(Queue<int> Queue, bool canClean)
    {
        if (canClean)
        {
            foreach (var ball in CleanQueue(Queue))
            {
                SpawnQueue.Enqueue(ball);
            }
        }
        else
        {
            foreach (var ball in Queue)
            {
                SpawnQueue.Enqueue(ball);
            }
        }
    }

    void Start()
    {
        int i = FirstBallID;
        //Create ball Dict
        foreach (var item in Resources.LoadAll("Prefabs/", typeof(GameObject)))
        {
            GameObject ball = item as GameObject;
            ball.GetComponent<Ball>().Data.BallID = i;
            BallSpawnObjectsDict.Add(i, ball);
            i++;
        }

        for (int b = 0; b < 5; b++)
        {
            SpawnQueue.Enqueue(FirstBallID);
        }
    }

    private void Update()
    {
        if (slider.value > 0)
        {
            SpawnBall();
        }
    }

    public void SpawnBall()
    {
        if (canSpawn)
        {
            EnqueueCleanBalls(CombineQueue);
            if (SpawnQueue.Count > 0)
            {
                EventsHandler.OnBallSpawned.Invoke(BallSpawnObjectsDict[SpawnQueue.Peek()].GetComponent<Ball>());
                StartCoroutine(Spawn(SpawnQueue.Dequeue()));
            }
            else if (StartingBalls > 0)
            {
                StartingBalls -= 1;
                SpawnQueue.Enqueue(FirstBallID);
                EventsHandler.OnBallSpawned.Invoke(BallSpawnObjectsDict[SpawnQueue.Peek()].GetComponent<Ball>());
                StartCoroutine(Spawn(SpawnQueue.Dequeue()));
            }
            else
            {
                //remove remaining balls in buckets and continue
                //if there are none game over
                if (transform.childCount > 0)
                {
                    if (!AreBallsStillFalling())
                    {
                        DestroyBalls(DestroyMode.Bucket, true);
                    }

                }
                else
                {
                    Debug.Log("Game Over");
                }

            }
        }
        
        UpdateBallsRemaining();
    }

    private void UpdateBallsRemaining()
    {
        ballsRemaining = SpawnQueue.Count;
    }

    private bool AreBallsStillFalling()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Ball>().isFalling)
            {
                return true;
            }
        }
        return false;
    }
    private void DestroyBalls(DestroyMode mode, bool queueBuckets)
    {
        foreach (Transform child in transform)
        {
            if (queueBuckets)
            {
                SpawnQueue.Enqueue(child.gameObject.GetComponent<Ball>().Data.BallID);
            }
            switch (mode)
            {
                case DestroyMode.All:
                    Destroy(child.gameObject);
                    break;
                case DestroyMode.Bucket:
                    if (child.gameObject.GetComponent<Ball>().inBucket)
                    {
                        Destroy(child.gameObject);
                    }
                    break;
                case DestroyMode.Grounded:
                    if (!child.gameObject.GetComponent<Ball>().isFalling)
                    {
                        Destroy(child.gameObject);
                    }
                    break;
                case DestroyMode.Falling:
                    if (child.gameObject.GetComponent<Ball>().isFalling)
                    {
                        Destroy(child.gameObject);
                    }
                    break;
                case DestroyMode.OutOfBucket:
                    if (!child.gameObject.GetComponent<Ball>().inBucket)
                    {
                        Destroy(child.gameObject);
                    }
                    break;
                default:
                    break;
            }
        }
    }
          

    // Update is called once per frame
    public IEnumerator Spawn(int BallID)
    {
        canSpawn = false;
        for (int i = 0; i < SpawnAmount; i++)
        {
            var obj = Instantiate(BallSpawnObjectsDict[BallID],transform.position,Quaternion.identity,transform);
            float Variation = UnityEngine.Random.Range(-0.05f, 0.06f);
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-slider.value+ UnityEngine.Random.Range(-slider.value - 0.002f, -slider.value + 0.002f), 0),ForceMode2D.Impulse);
        }

        yield return StartCoroutine(BallSapwnDelay());
    }

    public IEnumerator BallSapwnDelay()
    {
        yield return new WaitForSeconds(SpawnTimer);
        canSpawn = true;
    }

    private void OnDisable()
    {
        EventsHandler.OnBallCombined.RemoveListener(HandleBallCombined);
        EventsHandler.OnBoardReset.RemoveListener(HandleBoardReset);
    }
}
