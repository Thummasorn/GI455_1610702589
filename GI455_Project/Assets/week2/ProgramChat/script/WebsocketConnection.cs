using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;


namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        class MessageData
        {
            public string username;
            public string message;
        }

        //private int UserCheck = 0;
        public GameObject _Login;
        public GameObject _IP_Adress;
        public GameObject _Port;
        public GameObject _DisplayChat;
        public GameObject _TextChat;
        public GameObject _Chat;
        public GameObject _Send;

        public InputField InputUsername;
        //public InputField inputText;
        public InputField ChatText;
        public InputField Adress;
        public InputField Port;
        //public Text DisplayChat;
        public Text sendText;
        public Text receiveText;

        private string adress;
        private string port;
        //private string TextChat;
        private string tempMessageString;
        private WebSocket websocket;

        // Update is called once per frame
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
            //if (Input.GetKeyDown(KeyCode.Return))
            //{
            //    websocket.Send("Number :" + Random.Range(0, 99999));
            //}

        }
        public void Login()
        {
            adress = Adress.text;
            port = Port.text;
             
            
             websocket = new WebSocket("ws://" + adress + ":" + port + "/");
             websocket.OnMessage += OnMessage;
             websocket.Connect();
             
            

            _Login.SetActive(false);
            _IP_Adress.SetActive(false);
            _Port.SetActive(false);
            _DisplayChat.SetActive(true);
            _TextChat.SetActive(true);
            _Chat.SetActive(true);
            _Send.SetActive(true);
            

        }
       
        public void TextChatSend()
        {
            MessageData messageData = new MessageData();
            messageData.username = InputUsername.text;
            messageData.message = ChatText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);

            websocket.Send(toJsonStr);
            ChatText.text = "";
            //TextChat = ChatText.text;
            //ChatText.text = "";
            //websocket.Send(TextChat);
            //UserCheck++;
        }
        public void OnDestroy()
        {
            if(websocket != null)
            {
                
                websocket.Close();
                
            }
        }
        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
            //DisplayChat.text += "\n" + messageEventArgs.Data;
            Debug.Log("Message from server : " + messageEventArgs.Data);


            //if (UserCheck == 0)
            //{
            //    DisplayChat.alignment = TextAnchor.LowerLeft;
            //}

        }
       
    }
}