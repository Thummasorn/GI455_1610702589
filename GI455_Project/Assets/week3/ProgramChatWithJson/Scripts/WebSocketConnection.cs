using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
       //in class
        class MessageData
        {
            public string username;
            public string message;
        }
        //
   
        
        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField InputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:48484/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMessenger.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            //in class
            MessageData messageData = new MessageData();
            messageData.username = InputUsername.text;
            messageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);
            
            ws.Send(toJsonStr);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {

            //if (tempMessageString != null && tempMessageString != "")
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                //receiveText.text += tempMessageString + "\n";
                
                MessageData receivemessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                if (receivemessageData.username == InputUsername.text)
                {
                    sendText.text += receivemessageData.username + " : " + receivemessageData.message + "\n";
                    receiveText.text += "\n";
                }
                else
                {
                    sendText.text += "\n";
                    receiveText.text += receivemessageData.username + " : " + receivemessageData.message + "\n"; 
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
            Debug.Log(messageEventArgs.Data);
        }
    }
}


