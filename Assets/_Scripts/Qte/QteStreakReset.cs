using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class QteStreakReset : MonoBehaviour
{
    [SerializeField] bool reset = false;
    QteStreak streak;
    [Range(0.5f, 2)]
    [SerializeField] float timelineSpeed = 1;
    [SerializeField] float maxTimers;
    [SerializeField] List<QteBotao> qb;


    void Awake(){
        this.gameObject.GetComponent<PlayableDirector>().playableGraph.GetRootPlayable(0).SetSpeed(timelineSpeed);
        if(qb.Count > 0){
        foreach (QteBotao qte in qb){
            if(qte != null)
                qte.maxTimer = maxTimers;
        }}
        streak = new QteStreak();
    }
    void Update(){
        if(reset)
            streak.ResetStreak();
    }
}
