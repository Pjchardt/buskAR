using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    private float metronomeTime;
    public float MetronomeTime
    {
        get { return metronomeTime; }
        private set { metronomeTime = value; }
    }

    const float METRONOMELENGTH = 2f;
    bool metronomePlaying = false;   
    float beatTime;
    int beats;
    int currentBeat;

    public void StartSequencer(float time)
    {
        metronomePlaying = true;
        metronomeTime = time;
    }

    #region Monobehaviour Functions
    private void Start()
    {
        beats = Busker.NUMCOLUMNS;
        beatTime = METRONOMELENGTH / (float)beats;
        currentBeat = 0;
    }

    private void Update()
    {
        if (!metronomePlaying)
        {
            return;
        }

        MetronomeTime += Time.deltaTime;

        if (MetronomeTime > METRONOMELENGTH)
        {
            while (MetronomeTime > METRONOMELENGTH)
            {
                MetronomeTime -= METRONOMELENGTH;
            }
        }

        //Convert metronome time to int and check if new
        int newBeat = Mathf.FloorToInt((metronomeTime / METRONOMELENGTH) * (float)beats);
        if (newBeat != currentBeat)
        {
            //Play Note
            Busker.Instance.PlayBeat(newBeat);
            currentBeat = newBeat;
        }
    }
    #endregion
}
