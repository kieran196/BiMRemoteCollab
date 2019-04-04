using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class SendIFCData : NetworkBehaviour {

    public byte[] byteData;
    string path = "";
    private NetworkTransmitter networkTransmitter;
    public string ifcName;

    public void ByteToFileArray() { }

    private bool arrayReady = false;

    public void FileToByteArray(string fileName) {
        byte[] fileData = null;
        using (FileStream fs = File.OpenRead(fileName)) {
            var binaryReader = new BinaryReader(fs);
            fileData = binaryReader.ReadBytes((int)fs.Length);
        }
        CmdSyncByteData(fileData);
        //readByte(fileData);
    }

    /*private void readByte(Byte[] data) {
        print("Reading byte data..");
        for (int i=0; i<data.Length; i++) {
            if (i != data.Length) {
                byteData += data[i] + ",";
            } else {
                byteData += data[i];
            }
        }
        //CmdSyncByteData(byteData);
    }*/

    void gotData(byte[] bytes) {
        if (!isServer) {
            print("Received byte data.." + bytes.Length);
            
            print("path:"+path);
            File.WriteAllBytes(Application.dataPath + "/StreamingAssets/test.ifc", bytes);
            StreamReader reader = new StreamReader(Application.dataPath + "/StreamingAssets/test.ifc");
            print("written:" + reader.ReadToEnd());
            reader.Close();
            writeIfc(Application.dataPath + "/StreamingAssets/", "test");
            //importIFC("Assets/IFCHandling/IFCImporter/IFCFiles/" + name + ".ifc", ifcName);
        }
    }

    public void writeIfc(string path, string fileName) {
        print("Writing to IFC..");
        GameObject.FindGameObjectWithTag("IFCHandler").GetComponent<Manager>().importIFC(path + fileName + ".ifc", fileName);
    }

    [ClientRpc]
    public void RpcSyncByteData(byte[] byteData_) {
        byteData = byteData_;
        print("Recieved on client..");
        print("Length:"+byteData.Length);
        gotData(byteData);
    }

    [Command]
    public void CmdSyncByteData(byte[] byteData_) {
        RpcSyncByteData(byteData_);
        print("Recieved on server..");
    }

    // Start is called before the first frame update
    void Start() {
        path = Application.dataPath + "/StreamingAssets/test.ifc";
        print("MY PATH:" + path);
        if (isServer) {
            string file = "Assets/IFCHandling/IFCImporter/IFCFiles/" + ifcName + ".ifc";
            FileToByteArray(file);
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        print("BYTE DATA:"+players[0].GetComponent<SendIFCData>().byteData.Length);
        print("BYTE DATA:" + byteData.Length);
    }
}
