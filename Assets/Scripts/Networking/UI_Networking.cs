using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Networking : MonoBehaviour
{
    #region Public Variables

    /// <summary>
    /// References to objects in the UI heirarchy needed for onboarding flow
    /// </summary>
    public GameObject OnboardingPanel;
    public GameObject ConnectingPanel;

    public Button Button_BuskerConnect;
    public InputField BuskerNameInput;

    public Button Button_ViewerConnect;
    public InputField ViewerNameInput;

    #endregion

    #region Private Variables

    /// <summary>
    /// We need references to components to launch photon correctly depending on player type. We get a reference to these components by looking up through name.
    /// </summary>
    Launcher launcher;

    #endregion

    private void Awake()
    {
        ConnectingPanel.SetActive(false);

        launcher = GameObject.Find("Launcher").GetComponent<Launcher>();
        if (launcher == null)
            Debug.LogError("Could not find Launcher_Busker component in scene!");

        Button_BuskerConnect.interactable = false;
        Button_BuskerConnect.onClick.AddListener(OnButtonPressed_LaunchBusker);
        BuskerNameInput.onValueChanged.AddListener(BuskerNameInputUpdated);

        Button_ViewerConnect.interactable = false;
        Button_ViewerConnect.onClick.AddListener(OnButtonPressed_LaunchViewer);
        ViewerNameInput.onValueChanged.AddListener(Viewer_BuskerNameInputUpdated);
    }

    public void BuskerNameInputUpdated(string value)
    {
        if (value.Length > 0)
            Button_BuskerConnect.interactable = true;
        else
            Button_BuskerConnect.interactable = false;
    }

    public void Viewer_BuskerNameInputUpdated(string value)
    {
        if (value.Length > 0)
            Button_ViewerConnect.interactable = true;
        else
            Button_ViewerConnect.interactable = false;
    }

    public void OnButtonPressed_LaunchBusker()
    {
        launcher.ConnectToPhoton(Launcher.PlayerType.Busker, "@" + BuskerNameInput.text, ConnectedToPhoton);
        OnboardingPanel.SetActive(false);
        ConnectingPanel.SetActive(true);
    }

    public void OnButtonPressed_LaunchViewer()
    {
        launcher.ConnectToPhoton(Launcher.PlayerType.Viewer, "@" + ViewerNameInput.text, ConnectedToPhoton);
        OnboardingPanel.SetActive(false);
        ConnectingPanel.SetActive(true);
    }

    public delegate void ConnectedToPhotonDelegate(bool b);

    public void ConnectedToPhoton(bool success)
    {
        ConnectingPanel.SetActive(false);
    }
}
