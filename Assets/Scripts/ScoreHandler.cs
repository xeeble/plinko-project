using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreHandler : MonoBehaviour
{

    public TextMeshProUGUI scoreText;

    public int Score{ get; private set; }
    public int GreyBucketBonus;
    public int GreenBucketBonus;
    public int BlueBucketBonus;
    public int PurpleBucketBonus;

    private void OnEnable()
    {
        EventsHandler.OnBucketScored.AddListener(HandleBucketScored);
        EventsHandler.OnBallsCombinedClean.AddListener(HandleBallsCombined);
        EventsHandler.OnBoardReset.AddListener(HandleBoardReset);
    }

    private void Start()
    {
        Score = 0;
        UpdateScore();
    }

    public void IncreaseScore(int Value)
    {
        Score += Value;
        UpdateScore();
    }

    public void DecreaseScore(int Value)
    {
        Score -= Value;
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = Score.ToString();
        //animate score text
        Sequence sequence = DOTween.Sequence();
        sequence.Prepend(scoreText.DOScale(2,.5f).SetEase(Ease.OutElastic))
           .AppendInterval(.05f)
           .Append(scoreText.DOScale(1, .5f).SetEase(Ease.OutElastic));
    }

    private void ResetScore()
    {
        Score = 0;
        UpdateScore();
    }

    private void HandleBoardReset()
    {
        ResetScore();
    }

    private void HandleBallsCombined(Queue<int> arg0)
    {
        CalculateCombineBonus(arg0);
       

    }

    private void CalculateCombineBonus(Queue<int> arg0)
    {
        //takes the ball ID and determines score bonus for combination
        int Bonus = 0;
        foreach (var item in arg0)
        {
            switch (item - 1)
            {
                case 1:
                    Bonus = 100;
                    break;
                case 2:
                    Bonus = 300;
                    break;
                case 3:
                    Bonus = 1000;
                    break;
                case 4:
                    Bonus = 5000;
                    break;
                default:
                    Bonus = 100;
                    break;
            }
            IncreaseScore(Bonus);
        }
    }

    private void HandleBucketScored(ScoreBucket arg0, int arg1)
    {
        IncreaseScore(CalculateScore(arg0, arg1));
    }

    private int CalculateScore(ScoreBucket bucketValue, int BallValue)
    {
        int bucketBonus = 1;
        if ((int)bucketValue.BucketBonusType == BallValue)
        {

        }
        switch (bucketValue.BucketBonusType)
        {
            case BallTypes.Basic:
                bucketBonus = GreyBucketBonus;
                break;
            case BallTypes.Stage1:
                bucketBonus = GreenBucketBonus;
                break;
            case BallTypes.Stage2:
                bucketBonus = BlueBucketBonus;
                break;
            case BallTypes.Stage3:
                bucketBonus = PurpleBucketBonus;
                break;
            case BallTypes.none:
                break;
            default:
                break;
        }
        return (bucketValue.BucketValue * bucketBonus) * BallValue;
    }

    private void OnDisable()
    {
        EventsHandler.OnBucketScored.RemoveListener(HandleBucketScored);
        EventsHandler.OnBallsCombinedClean.RemoveListener(HandleBallsCombined);
        EventsHandler.OnBoardReset.RemoveListener(HandleBoardReset);
    }
}
