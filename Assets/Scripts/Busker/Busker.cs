using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class Busker : MonoBehaviour
{
    public static Busker Instance; //Can we not have this be an instance?

    [HideInInspector]
    public float valueOne = 440f;
    [HideInInspector]
    public float valueTwo = .025f;
    [HideInInspector]
    public float valueTempo = 1f;
    [HideInInspector]
    public float valuePitchRange = 1f;

    public Slider pitchSliderOne;
    public Slider pitchSliderTwo;
    public Slider pitchSliderTempo;
    public Slider pitchSliderPitchRange;
    public Text HeavyOutput;

    int noteIndex = 0;
    const int NUMNOTES = 5; const int NUMCOLUMNS = 8;
    SequencerObject[] SequencerObjects = new SequencerObject[NUMNOTES * NUMCOLUMNS];

    public Transform Sequencer_UIRoot;
    public GameObject SequencerCellPrefab;
    public Vector3 CellStart; public Vector3 CellOffset;
    float[] buffer = new float[NUMNOTES * NUMCOLUMNS];

    public Hv_augmentedBusker_1_AudioLib heavyPdRef;

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            heavyPdRef.RegisterSendHook();
            heavyPdRef.FloatReceivedCallback += OnFloatMessage;

            heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitch1, (120f + pitchSliderOne.value * 760f));
            heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitch2, (120f + pitchSliderTwo.value * 760f));
            //heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter. .Tempo, (30f + pitchSliderOne.value * 1970f));
            heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitchrange, (50f + pitchSliderPitchRange.value * 1550f));

            pitchSliderOne.onValueChanged.AddListener(delegate { OnValueChangeSliderOne(); });
            pitchSliderTwo.onValueChanged.AddListener(delegate { OnValueChangeSliderTwo(); });
            pitchSliderTempo.onValueChanged.AddListener(delegate { OnValueChangeSliderTempo(); });
            pitchSliderPitchRange.onValueChanged.AddListener(delegate { OnValueChangeSliderPitchChange(); });

            CreateSequencer();
            heavyPdRef.FillTableWithFloatBuffer("columnOne", buffer); //set values to zero
        }
        else
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
            return;

        for (int i = 0; i < NUMCOLUMNS * NUMNOTES; i++)
        {
            SequencerObjects[i].UpdateObject(DataObject.Instance.ARObjectToTrack, 1f);
        }
    }

    private void OnDestroy()
    {
        heavyPdRef.FloatReceivedCallback -= OnFloatMessage;
    }

    #endregion

    void OnFloatMessage(Hv_augmentedBusker_1_AudioLib.FloatMessage message)
    {
        HeavyOutput.text = "Name: " + message.receiverName + " and value: " + message.value.ToString();
        noteIndex = (int)message.value;
    }

    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("On serialize view.");

        if (stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
            {
                stream.SendNext(valueOne);
                stream.SendNext(valueTwo);
                stream.SendNext(valueTempo);
                stream.SendNext(valuePitchRange);
                stream.SendNext(noteIndex);
            }
                
        }
        else
        {
            valueOne = (float) stream.ReceiveNext();
            valueTwo = (float)stream.ReceiveNext();
            valueTempo = (float)stream.ReceiveNext();
            valuePitchRange = (float)stream.ReceiveNext();
            int temp = noteIndex;
            noteIndex = (int)stream.ReceiveNext();
            if (temp != noteIndex)
            {
                for (int i = (noteIndex * NUMNOTES); i < (noteIndex * NUMNOTES) + NUMNOTES; i++)
                {
                    if (SequencerObjects[i] == null)
                        Debug.Log("Sequencer object: " + i.ToString() + " is null!");
                    else
                        SequencerObjects[i].PlayNote();
                }
            }
        }
    }
    #endregion

    #region Busker Controls 

    void CreateSequencer()
    {
        for (int i = 0; i < NUMCOLUMNS; i++)
        {
            for (int j = 0; j < NUMNOTES; j++)
            {
                //create Sequence Cell object to hold note and data
                GameObject objSO = PhotonNetwork.Instantiate("SequencerObject", Vector3.zero, Quaternion.identity, 0);
                GameObject obj = CreateObject(i, j);
                SequencerObject sO = objSO.GetComponent<SequencerObject>();
                sO.Init_Busker(obj, i, j);
                SequencerObjects[j + i * NUMNOTES] = sO;
            }
        }
    }

    GameObject CreateObject(int i, int j)
    {
        GameObject obj = Instantiate(SequencerCellPrefab);
        obj.transform.SetParent(Sequencer_UIRoot);
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localPosition = CellStart + new Vector3(CellOffset.x * i, CellOffset.y * -j, 0f);
        return obj;
    }

    public void AddObject(SequencerObject sO)
    {
        SequencerObjects[sO.row + (sO.column * NUMNOTES)] = sO;
    }

    public void SequencerCellToggled(int row, int column, bool b)
    {
        //Will have to make a big switch statement. Probably a better way, but for now it will work.
        Debug.Log("Setting value for note: " + row.ToString() + " " + column.ToString());

        buffer[row + (column * NUMNOTES)] = Convert.ToInt32(b);
        heavyPdRef.FillTableWithFloatBuffer("columnOne", buffer);

        SequencerObject.Notes note = (SequencerObject.Notes)column;
    }

    public void OnValueChangeSliderOne()
    {
        valueOne = (120f + pitchSliderOne.value * 760f);
        heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitch1, valueOne);
    }

    public void OnValueChangeSliderTwo()
    {
        valueTwo = (120f + pitchSliderTwo.value * 760f);
        heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitch2, valueTwo);
    }

    public void OnValueChangeSliderTempo()
    {
        valueTempo = (30f + pitchSliderTempo.value * 1970f);
        //heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Tempo, valueTempo);
    }

    public void OnValueChangeSliderPitchChange()
    {
        valuePitchRange = (50f + pitchSliderPitchRange.value * 1550f);
        heavyPdRef.SetFloatParameter(Hv_augmentedBusker_1_AudioLib.Parameter.Pitchrange, valuePitchRange);
    }

    #endregion
}
