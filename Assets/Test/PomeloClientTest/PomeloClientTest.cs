//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using Core.WindJson;
using System;
using Pomelo.DotNetClient.Test;

public class PomeloClientTest : MonoBehaviour
{
    public  string          url  = "127.0.0.1";
    public  int             port = 3010;

    private PomeloClient    client;

    void OnGUI()
    {
        if (GUILayout.Button("Connect", GUILayout.Height(30), GUILayout.Width(120)))
        {
            Connect();
        }

        if (GUILayout.Button("Send Message", GUILayout.Height(30), GUILayout.Width(120)))
        {
            SendMessage();
        }

        if (GUILayout.Button("Disconnect", GUILayout.Height(30), GUILayout.Width(120)))
        {
            Disconnect();
        }
    }
    
    private void Connect()
    {
        client = new PomeloClient();
        client.initClient(url, port, () =>
        {
            Debug.Log("Pomelo client init success!");
        });

        JsonNode user = new JsonClass();
        user.Add("user", new JsonData("Winddy"));
        user.Add("pwd", new JsonData("123"));

        bool result = client.connect(user, new System.Action<JsonNode>(OnConnection));

        if (result)
            Debug.Log("Connect success!");
        else
            Debug.Log("Connect failed!");

        //ClientTest.loginTest("127.0.0.1", 3014);
    }

    private void OnConnection(JsonNode theJsonClass)
    {
        Debug.Log("Connection Established: " + theJsonClass.ToString());
    }
    
    private void Disconnect()
    {
        client.disconnect();
        Debug.Log("Disconnected!");
    }

    private void SendMessage()
    {
        JsonNode msg = new JsonClass();
        msg.Add("content", new JsonData("Hello unity pomelo !!!"));
        client.request("connector.entryHandler.helloWinddy", msg, new System.Action<JsonNode>(OnSendMessage));
    }

    private void OnSendMessage(JsonNode theJsonClass)
    {
        Debug.Log("Message Received: " + theJsonClass.ToString());
    }
}
