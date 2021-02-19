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
        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        //class MessageData
        //{
        //    public string username;
        //    public string message;
        //}


        //public GameObject _IP_Adress;
        //public GameObject _Port;
        public GameObject _loginDisplay;
        public GameObject _CreateAndJoin;
        public GameObject _RoomNameCreate;
        public GameObject _RoomNameJoin;
        public GameObject _DisplayChat;
        
        public GameObject _CreateFail;
        public GameObject _Joinfail;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        private string tempmessage;
        private string roomNames;
        public InputField RoomName;
        public Text Room;

        public InputField roomjoin;
        public InputField InputUsername;
        public InputField ChatText;
        //public InputField Adress;
        //public InputField Port;
        public Text sendText;
        public Text receiveText;

        //private string adress;
        //private string port;
        private string tempMessageString;
        //private string _tempMessageString;
        private WebSocket websocket;

        public string Join;

        public void Start()
        {
            OnCreateRoom += PopUpCreateFail;
            OnJoinRoom += PopUpJoinFail;
            
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateNotifyMessage();

            //if (string.IsNullOrEmpty(tempMessageString) == false)
            //{


            //    MessageData receivemessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

            //    if (receivemessageData.username == InputUsername.text)
            //    {
            //        sendText.text += receivemessageData.username + " : " + receivemessageData.message + "\n";
            //        receiveText.text += "\n";
            //    }
            //    else
            //    {
            //        sendText.text += "\n";
            //        receiveText.text += receivemessageData.username + " : " + receivemessageData.message + "\n";
            //    }

            //    tempMessageString = "";
            //}

        }
      
        public void Login()
        {
            //adress = Adress.text;
            //port = Port.text;

            string url = "ws://127.0.0.1:48484/";
            websocket = new WebSocket(url);
            websocket.OnMessage += OnMessage;
            websocket.Connect();
             
         
            _loginDisplay.SetActive(false);
    
            _CreateAndJoin.SetActive(true);
  
        }
        public void CreateRoom(string roomName)
        {
            
            _RoomNameCreate.SetActive(true);
            _CreateAndJoin.SetActive(false);
            
        }
        public void JoinRoom(string roomName)
        {
            _RoomNameJoin.SetActive(true);
            _CreateAndJoin.SetActive(false);
            
        }
        
        public void CreateRoomButton()
        {

            SocketEvent socketEvent = new SocketEvent("CreateRoom", RoomName.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            websocket.Send(toJsonStr);

            roomNames = RoomName.text;
            Room.text = roomNames;

            _RoomNameCreate.SetActive(false);
            _DisplayChat.SetActive(true);


        }
        public void JoinRoomButton()
        {
            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomjoin.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            websocket.Send(toJsonStr);

            roomNames = roomjoin.text;
            Room.text = roomjoin.text;

            _RoomNameJoin.SetActive(false);
            _DisplayChat.SetActive(true);

        }
        public void PopUpCreateFail(SocketEvent isfail)
        {
            if (isfail.data == "fail")
            {
                _CreateFail.SetActive(true);
                _DisplayChat.SetActive(false);
            }
            else
            {
                _RoomNameCreate.SetActive(false);
                _DisplayChat.SetActive(true);
            }
        }
        public void PopUpJoinFail(SocketEvent isfail)
        {
            if (isfail.data == "fail")
            {
                _DisplayChat.SetActive(false);
                _Joinfail.SetActive(true);
            }
            else
            {
                _DisplayChat.SetActive(true);
                _RoomNameJoin.SetActive(false);
            }
        }

        public void TextChatSend()
        {
            //MessageData messageData = new MessageData();
            //messageData.username = InputUsername.text;
            //messageData.message = ChatText.text;

            //string toJsonStr = JsonUtility.ToJson(messageData);

            //websocket.Send(toJsonStr);
            //ChatText.text = "";
            
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", RoomName.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            websocket.Send(toJsonStr);

            roomNames = RoomName.text;
            Room.text = roomNames;

            _DisplayChat.SetActive(false);
            _CreateAndJoin.SetActive(true);
            

        }

        public void Disconnect()
        {
            if (websocket != null)
                websocket.Close();
        }
        public void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempmessage) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempmessage);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                    if (receiveMessageData.data != "fail")
                    {
                        _CreateFail.SetActive(false);
                    }
                    else
                    {
                        _CreateFail.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                    if (receiveMessageData.data != "fail")
                    {
                        _Joinfail.SetActive(false);
                    }
                    else
                    {
                        _Joinfail.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                tempmessage = "";
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempmessage = messageEventArgs.Data;
            Debug.Log("Message from server : " + messageEventArgs.Data);

        }
       
    }
}