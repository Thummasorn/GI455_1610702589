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

        [System.Serializable]
        class MessageData
        {
            public string username;
            public string message;
        }

        public string Username;
  
        public GameObject _ConnectDisplay;
        public GameObject _LoginDisplay;
        public GameObject _RegisterDisplay;
        public GameObject _CreateAndJoin;
        public GameObject _RoomNameCreate;
        public GameObject _RoomNameJoin;
        public GameObject _DisplayChat;
        public GameObject _CreateFail;
        public GameObject _Joinfail;
        public GameObject _PopupDataNotComplete;
        public GameObject _PasswordNotMatch;
        public GameObject _PopUserIDalready;
        public GameObject _PopupLoginFail;
        //public GameObject _NameUserPanel;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnRegister;
        public DelegateHandle OnLogin;
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        public DelegateHandle OnNameUser;

        private string tempmessage;
        private string roomNames;
        public InputField RoomName;
        public Text Room;

        public InputField UserIDLogin;
        public InputField PasswordLogin;
        public Text nameUser;

        public InputField UserIDRegister;
        public InputField InputUsername;
        public InputField PasswordRegister;
        public InputField RePassword;


        public InputField roomjoin;
        public InputField ChatText;
        
        public Text sendText;
        public Text receiveText;

        
        private string tempMessageString;
        //private string _tempMessageString;
        private WebSocket websocket;

        public string Join;

        public void Start()
        {
            OnCreateRoom += PopUpCreateFail;
            OnJoinRoom += PopUpJoinFail;
            OnRegister += PopupRegisterFail;
            OnLogin += PopupLoginFail;
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateNotifyMessage();

        }
      
        public void Connect()
        {
            string url = "ws://127.0.0.1:48484/";
            websocket = new WebSocket(url);
            websocket.OnMessage += OnMessage;
            websocket.Connect();
             
         
            _ConnectDisplay.SetActive(false);

            _LoginDisplay.SetActive(true);
  
        }

        public void PopupDataNotCompleteButton()
        {
            _PopupDataNotComplete.SetActive(false);
        }
        
        public void Register()
        {
            _LoginDisplay.SetActive(false);
            _RegisterDisplay.SetActive(true);

        }

        public void Login(string Lg)
        {
            Lg = UserIDLogin.text + "#" + PasswordLogin.text;
            SocketEvent socketEvent = new SocketEvent("Login", Lg);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            websocket.Send(toJsonStr);

            //nameUser.text = Lg;


            if (UserIDLogin.text == "" || PasswordLogin.text == "")
            {
                _PopupLoginFail.SetActive(true);
            }
            else
            {
                _LoginDisplay.SetActive(false);
                //_NameUserPanel.SetActive(true);
                _CreateAndJoin.SetActive(true);
            }

        }
        public void PopupLoginFail(SocketEvent LgFail)
        {
            if (LgFail.data == "fail")
            {
                _LoginDisplay.SetActive(true);
                _PopupLoginFail.SetActive(true);
                _CreateAndJoin.SetActive(false);

            }
        }
        public void PopupLoginFailButton()
        {
            _PopupLoginFail.SetActive(false);
        }

        public void Registered(string Rg)
        {


            if (UserIDRegister.text == "" || InputUsername.text == "" || PasswordRegister.text == "" || RePassword.text == "")
            {
                _PopupDataNotComplete.SetActive(true);

            }
            else
            {
                if (PasswordRegister.text == RePassword.text)
                {
                    Rg = UserIDRegister.text + "#" + PasswordRegister.text + "#" + InputUsername.text;
                    SocketEvent socketEvent = new SocketEvent("Register", Rg);

                    string toJsonStr = JsonUtility.ToJson(socketEvent);

                    websocket.Send(toJsonStr);
                    _PasswordNotMatch.SetActive(false);
                    _RegisterDisplay.SetActive(false);
                    _LoginDisplay.SetActive(true);
                }
                else
                {
                    _PasswordNotMatch.SetActive(true);
                }
            }
        }

        public void PopupRegisterFail(SocketEvent RgFail)
        {
            if (RgFail.data == "fail")
            {
                _LoginDisplay.SetActive(false);
                _PopUserIDalready.SetActive(true);
                _RegisterDisplay.SetActive(true);

            }
            else
            {
                _PopUserIDalready.SetActive(false);
                _LoginDisplay.SetActive(true);
            }
        }
        public void CreateRoom()
        {
            
            _RoomNameCreate.SetActive(true);
            _CreateAndJoin.SetActive(false);
            
        }
        public void JoinRoom()
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
        public void PopUpCreateFailButton()
        {
            _CreateFail.SetActive(false);
            _CreateAndJoin.SetActive(true);
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

        public void PopUpJoinFailButton()
        {
            _Joinfail.SetActive(false);
            _CreateAndJoin.SetActive(true);
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

        public void SendMessage()
        {
            if (ChatText.text == "" || websocket.ReadyState != WebSocketState.Open)
                return;
            MessageData messageData = new MessageData();
            messageData.username = Username;
            messageData.message = ChatText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);
            SocketEvent socketEvent = new SocketEvent("SendMessage" ,toJsonStr);
            string fromJsonStr = JsonUtility.ToJson(socketEvent);
            websocket.Send(fromJsonStr);
    
            ChatText.text = "";

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
                

                if (receiveMessageData.eventName == "Register")
                {
                    if (OnRegister != null)
                        OnRegister(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "Login")
                {
                    if (OnLogin != null)
                        OnLogin(receiveMessageData);
                }

                else if(receiveMessageData.eventName == "name")
                {
                    if (OnNameUser != null)
                        OnNameUser(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "SendMessage")
                {
                    MessageData _Message = JsonUtility.FromJson<MessageData>(receiveMessageData.data);
                    if (_Message.username == Username)
                    {
                        sendText.text += _Message.username + " : " + _Message.message + "\n";
                        receiveText.text += "\n";
                    }
                    else
                    {
                        sendText.text += "\n";
                        receiveText.text += _Message.username + " : " + _Message.message + "\n";
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
            messageEventArgs.Data.Split('#');
            tempmessage = messageEventArgs.Data;
            Debug.Log("Message from server : " + messageEventArgs.Data);

        }
       
    }
}