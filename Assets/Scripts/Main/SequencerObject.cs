using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequencerObject : MonoBehaviour
{
    public AudioClip[] Notes;
    AudioSource aS; //For AR, we should move this to note in space, for 3d audio
    public int row;
    public int column;

    #region Monobehaviours
    private void Awake()
    {
        aS = GetComponent<AudioSource>();
    }
    #endregion

    #region Sequencer Object Functions
    public void PlayNote()
    {
        if (!isOn)
            return;

        //Debug.Log("Playing note: " + row + ":" + column);
        aS.Play();

        if (!PhotonNetwork.isMasterClient)
        {
            if (isOn && !vizObject.activeSelf)
            {
                vizObject.SetActive(true);
            }
                
            anim.Play();
        }
    }
    #endregion

    #region Busker 
    // If we are the busker, we need to intialize with our own variables and functions.
    Toggle toggle;

    public void Init_Busker(GameObject obj, int c, int r)
    {
        column = c;
        row = r;
        aS.clip = Notes[row];

        toggle = obj.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(delegate { OnValueChanged_Toggle(toggle); });
    }

    public void OnValueChanged_Toggle(Toggle t)
    {
        isOn = t.isOn;
    }
    #endregion

    #region Viewer
    // If we are the viewer, we need to intialize with our own variables and functions.

    public GameObject SequencerVizPrefab;
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
        inclinationLevel = Random.Range(-Mathf.PI, Mathf.PI); 
        radius = .2f; //we will update radius when we get our row and column

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
        Vector3 v = Vector3.zero;
        v.x = origin.x + radius * Mathf.Sin(elevationAngle * delta) * Mathf.Cos(inclinationLevel * delta);
        v.y = origin.y + inclinationLevel;
        v.z = origin.z + radius * Mathf.Cos(elevationAngle * delta);
        vizObject.transform.position = v;
    }

    public void OnValueChanged_Toggle()
    {
        if (!isOn)
            vizObject.SetActive(false);
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
        aS.clip = Notes[row];
        vizObject.name = "Viz object: " + row.ToString() + " " + column.ToString();

        isOn = b;
        radius = .2f + (r * .025f);
        elevationAngle = -Mathf.PI + ((2f * Mathf.PI) / 8f) * c;
        inclinationLevel = -.2f + (.1f * r);
        Busker.Instance.AddObject(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("On serialize view.");

        if (stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
            {
                stream.SendNext(isOn);
            }

        }
        else
        {
            isOn = (bool)stream.ReceiveNext();
            OnValueChanged_Toggle();
        }
    }
    #endregion
}
