using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventsHandler {

    public static UnityEvent<int> OnBallCombined = new UnityEvent<int>();
    public static UnityEvent<ScoreBucket, int> OnBucketScored = new UnityEvent<ScoreBucket,int>();
    public static UnityEvent<Queue<int>> OnBallsCombinedClean = new UnityEvent<Queue<int>>();
    public static UnityEvent OnBoardReset = new UnityEvent();
    public static UnityEvent<Ball> OnBallSpawned = new UnityEvent<Ball>();

}
