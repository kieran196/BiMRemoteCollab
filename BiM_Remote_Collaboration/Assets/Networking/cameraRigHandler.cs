using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// ===============================
/// AUTHOR: Kieran William May
/// PURPOSE: This class is responsible for users switching between Rigs. SyncVars are used to synchronize data about the different types of rigs to the server/client(s)
/// NOTES:
/// 4 types of rigs are supported
/// * A VR Simulator: (Useful for testing VR. Simulates a SteamVR camerarig without a HMD)
/// * SteamVR Camera Rig: Supports HTC Vive & Oculus Rift HMDs
/// * AR Camera Rig: Using Holo-Toolkit. Supports Hololens & other Mixed Reality HMDs
/// * Opti-Track rig: Communicates with Motif to track Opti-track objects.
/// Currently I'm not using the Opti-Track for anything in the system, but it can be used for full-body tracking or using controllers with the Hololens
/// ===============================


public class cameraRigHandler : NetworkBehaviour {

    public bool usingHololens = false;

    public GameObject VRSimulator_Rig;
    public GameObject SteamVR_Rig;
    public GameObject AR_Rig;

    //[SyncVar]
    public GameObject currentRig;

    //[SyncVar]
    [SyncVar(hook = "OnRigTypeChange")]
    public string rigType;

    [SyncVar]
    public string varRigType;

    [SyncVar]
    public string rigTypeRpcTesting;

    [SyncVar]
    public string lastRig;

    public GameObject mainPanel;
    public GameObject sidePanel;

    public Text label;
    public Transform player;
    public Transform drawParent;
    public enum GAME_STATES {MENU, GAME}
    public GAME_STATES currentState;

    public void updateLabel() {
        Debug.Log("Updating these labels..");
        NetworkHandler.players.Add(currentRig.name);
        label.text = "Current Active Rig:" + currentRig.name + " \n" +
                     "Press 1 to activate VR Sim\n" +
                     "Press 2 to activate VR Rig\n" +
                     "Press 3 to activate AR Rig\n" +
                     "Press 4 to activate Operator";
    }

    public GameObject getRig() {
        return Input.GetKeyDown(KeyCode.Alpha1) ? VRSimulator_Rig : Input.GetKeyDown(KeyCode.Alpha2) ? SteamVR_Rig : null;
    }

    [Command]
    void CmdLastRig() {
        
        RpclastRigType();
    }

    [ClientRpc]
    void RpclastRigType() {
        if(rigType != null) {
            lastRig = rigType;
        }
    }

    void OnRigTypeChange(string newRig) {
        if(isLocalPlayer)
            return;
        rigType = newRig;
    }

    [Command]
    void CmdAssignRig(string rig) {
        RpcAssignRig(rig);
    }

    [ClientRpc]
    void RpcAssignRig(string rig) {
        if(rig != "OperatorPanel") {
            rigType = rig;
            //print("New rig enabled:" + rigType);
        }
    }

    void operatorPanelRig(GameObject rig) {
        rig.GetComponentInChildren<Camera>().targetDisplay = 1;
    }

    public void assignRigs(string rigName) {
        if(isServer) {
            RpcAssignRig(rigName);
        } else {
            CmdAssignRig(rigName);
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();
        OnRigTypeChange(rigType);
    }

    public void loadPrefabs() {
        foreach(var prefab in FindObjectOfType<NetworkManager>().spawnPrefabs) {
            if(prefab != null) {
                ClientScene.RegisterPrefab(prefab);
            }
        }
    }

    void AutoSwitchClientHololens() {
        print("Auto Switching Client..");
        GameObject rig = AR_Rig;
        if(rig == null) return;
        print("Activated Rig:" + rig.name);
        CmdLastRig();
        if(currentRig == null) currentRig = rig; CmdAssignRig(rig.name); mainPanel.SetActive(false); sidePanel.SetActive(true); loadPrefabs();

        if(rig.activeInHierarchy == false) {
            currentRig.SetActive(false);
            currentRig = rig;
            CmdAssignRig(rig.name);
            updateLabel();
        }
    }

    void SwitchClient() {
        if(Input.anyKeyDown) {
            GameObject rig = getRig();
            if(rig == null) return;
            print("Activated Rig:"+rig.name);
            CmdLastRig();
            if(currentRig == null) currentRig = rig; CmdAssignRig(rig.name); mainPanel.SetActive(false); sidePanel.SetActive(true); loadPrefabs(); currentState = GAME_STATES.GAME;

            if(rig.activeInHierarchy == false) {
                currentRig.SetActive(false);
                currentRig = rig;
                CmdAssignRig(rig.name);
                updateLabel();
            }
        }
    }

    /*[SyncVar]
    public string networkID;

    [ClientRpc]
    void RpcSyncVarWithClients(string varToSync) {
        networkID = varToSync;
        this.transform.name = networkID;
    }*/

    public bool VRActivated = false;
    public bool rpcVarAssigned = false;
    private void Update() {
        if(isLocalPlayer && !usingHololens) {
            if(currentRig != null) {
                CmdAssignRig(currentRig.name);
            }
            SwitchClient();
        } else if(isLocalPlayer && usingHololens) {
            if(currentRig == null && AR_Rig != null) {
                AutoSwitchClientHololens();
            }
        }
    }

    public override void OnStartLocalPlayer() {
        print("OnStartLocalPlayer called.. IS THIS COMPILING?");
        if(isLocalPlayer) {
            this.enabled = true;
            this.transform.Find("MenuScreen").GetComponent<Canvas>().enabled = true;
            this.transform.Find("MenuScreen").GetComponentInChildren<Camera>().enabled = true;
            print("Canvas loaded.." + this.transform.Find("MenuScreen").GetComponent<Canvas>().enabled);
            Cursor.visible = true;
        }
        //currentRig = VRSimulator_Rig;
    }

}
