using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class Busker : MonoBehaviour
{
    public static Busker Instance;

    int noteIndex = 0;
    public const int NUMNOTES = 5; 
    public const int NUMCOLUMNS = 8;
    public Sequencer SequencerRef;
    SequencerObject[] SequencerObjects = new SequencerObject[NUMNOTES * NUMCOLUMNS];

    public Transform Sequencer_UIRoot;
    public GameObject SequencerCellPrefab;
    public Vector3 CellStart;
    public Vector3 CellOffset;

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            CreateSequencer();
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
    #endregion

    #region Busker Controls 

    void CreateSequencer()
    {
        for (int i = 0; i < NUMCOLUMNS; i++)
        {
            for (int j = 0; j < NUMNOTES; j++)
            {
                //Create Sequence Cell object to hold note and data
                GameObject objSO = PhotonNetwork.Instantiate("SequencerObject", Vector3.zero, Quaternion.identity, 0);
                GameObject obj = CreateObject(i, j);
                SequencerObject sO = objSO.GetComponent<SequencerObject>();
                sO.Init_Busker(obj, i, j);
                SequencerObjects[j + i * NUMNOTES] = sO;

                //Start sequencer
                SequencerRef.StartSequencer(0f);
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

    public void PlayBeat(int column)
    {
        for (int i = (column * NUMNOTES); i < (column * NUMNOTES) + NUMNOTES; i++)
        {
            if (SequencerObjects[i] == null)
                Debug.Log("Sequencer object: " + i.ToString() + " is null!");
            else
                SequencerObjects[i].PlayNote();
        }
    }
    #endregion

    #region Photon
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("GetMetronomeTime", PhotonTargets.MasterClient);
        }
    }

    [PunRPC]
    public void GetMetronomeTime()
    {
        float mTime = SequencerRef.MetronomeTime;
        GetComponent<PhotonView>().RPC("SendMetronomeTime", PhotonTargets.Others, mTime);
    }

    [PunRPC]
    public void SendMetronomeTime(float mTime, PhotonMessageInfo info)
    {
        double delta = PhotonNetwork.time - info.timestamp;
        SequencerRef.StartSequencer(mTime + (float)delta - Time.deltaTime);
    }
    #endregion
}
