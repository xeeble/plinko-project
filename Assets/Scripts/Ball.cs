using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallData Data;
    public bool isCheckingCollision = false;
    public bool inBucket = false;
    public bool isFalling = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
            collision.gameObject.TryGetComponent(out Ball ball);
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (isCheckingCollision == false || inBucket == false) { 
                CheckBallCombination(Data.BallID, ball.Data.BallID); }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bucket"))
        {
            inBucket = true;
            isFalling = false;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFalling = false;
        }
    }

    private void CheckBallCombination(int ballID1, int ballID2)
    {
        if (isCheckingCollision)
        {
            return;
        }
        isCheckingCollision = true;
        if (ballID1 == ballID2 && inBucket)
        {
            Destroy(this.gameObject);
            EventsHandler.OnBallCombined.Invoke(Data.BallID);
        }

        isCheckingCollision = false;
    }
}
