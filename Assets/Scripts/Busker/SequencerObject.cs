using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequencerObject : InteractableObject
{
    public enum Notes { C, Dsharp, G, F, Asharp };
    public int row;
    public int column;

    #region Busker 
    /// <summary>
    /// If we are the busker, who is the masterClient on the network, we have a different initialization and variables. We do this to have the serialization occur over the network.
    /// TODO: Move serialization to the Busker class, and send updates to the SequenceObject_Viewer and SequenceObject_Busker
    /// </summary>

    Toggle toggle;

    public void Init_Busker(GameObject obj, int c, int r)
    {
        column = c;
        row = r;
        toggle = obj.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(delegate { OnValueChanged_Toggle(toggle); });
    }

    public void OnValueChanged_Toggle(Toggle t)
    {
        isOn = t.isOn;
        Busker.Instance.SequencerCellToggled(row, column, t.isOn);
    }
    #endregion

    #region Viewer
    /// <summary>
    /// If we are the viewer, we need to intialize with our own variables and functions.
    /// </summary>
    public GameObject SequencerVizPrefab;
    public GameObject ClickEffect;
    GameObject vizObject;
    Animation anim;

    bool isOn;

    float radius;
    float elevationAngle;
    float inclinationLevel;

    public void Init_Viewer()
    {
        vizObject = Instantiate(SequencerVizPrefab);
        anim = vizObject.GetComponentInChildren<Animation>();

        elevationAngle = Random.Range(-Mathf.PI, Mathf.PI);
        inclinationLevel = Random.Range(-Mathf.PI, Mathf.PI); //should this be (0 - elevation Level?)
        radius = Random.Range(.2f, .3f);

        UpdateObject(null, 1f);
    }

    public void UpdateObject(GameObject obj, float delta)
    {
        if (obj == null)
        {
            vizObject.SetActive(false);
            return;
        }

        Vector3 origin = obj.transform.position;
        Debug.Log(origin);
        Vector3 v = Vector3.zero;
        v.x = origin.x + radius * Mathf.Sin(elevationAngle * delta) * Mathf.Cos(inclinationLevel * delta);
        v.y = origin.y + radius * Mathf.Sin(elevationAngle * delta) * Mathf.Sin(inclinationLevel * delta);
        v.z = origin.z + radius * Mathf.Cos(elevationAngle * delta);
        vizObject.transform.position = v;
    }

    public void PlayNote()
    {
        if (!isOn)
            return;

        if (isOn && !vizObject.activeSelf)
            vizObject.SetActive(true);

        anim.Play();
    }

    public void OnValueChanged_Toggle()
    {
        if (!isOn)
            vizObject.SetActive(false);
    }

    public override void OnClick()
    {
        base.OnClick();

        GameObject obj = Instantiate(ClickEffect);
        obj.transform.position = transform.position;
        Destroy(obj, 3f);
    }
#endregion

#region Photon
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Init_Viewer();
            GetComponent<PhotonView>().RPC("GetMyInfo", PhotonTargets.MasterClient);
        }
    }

    [PunRPC]
    public void GetMyInfo()
    {
        GetComponent<PhotonView>().RPC("SendInfo", PhotonTargets.Others, row, column, isOn);
    }

    [PunRPC]
    public void SendInfo(int r, int c, bool b)
    {
        row = r;
        column = c;
        isOn = b;
        Busker.Instance.AddObject(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("On serialize view.");

        if (stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
            {
                stream.SendNext(row);
                stream.SendNext(column);
                stream.SendNext(isOn);
            }

        }
        else
        {
            row = (int)stream.ReceiveNext();
            column = (int)stream.ReceiveNext();
            isOn = (bool)stream.ReceiveNext();
            vizObject.name = "Viz object: " + row.ToString() + " " + column.ToString();
            OnValueChanged_Toggle();
        }
    }
    #endregion
}
