using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreBucket : MonoBehaviour
{
    public int value;
    public int BucketValue { get; private set; }
    public ScoreHandler handler;
    public BallTypes BucketBonusType;
    public TextMeshProUGUI BucketValueText;

    private void Start()
    {
        BucketValue = value;
        UpdateText(value.ToString());
        UpdateAppearance(BucketBonusType);
    }

    private void UpdateAppearance(BallTypes bucketBonusType)
    {
        //get sprite renderers
        //change colour to matching
    }

    private void UpdateText(string newText) 
    {
        BucketValueText.text = newText;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {

            EventsHandler.OnBucketScored.Invoke(this,collision.gameObject.GetComponent<Ball>().Data.BallValue);
        }
    }
}
