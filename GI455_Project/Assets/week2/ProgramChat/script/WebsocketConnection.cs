using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;


namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        
        private int UserCheck = 0;
        public GameObject _Login;
        public GameObject _IP_Adress;
        public GameObject _Port;
        public GameObject _WindowChat;
        public GameObject _TextChat;
        public GameObject _Chat;
        public GameObject _Send;
        public GameObject _ScrollView;

        public InputField ChatText;
        public InputField Adress;
        public InputField Port;
        public Text DisplayChat;
 
        private string adress;
        private string port;
        private string TextChat;
        private WebSocket websocket;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                websocket.Send("Number :" + Random.Range(0, 99999));
            }

            

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
            _WindowChat.SetActive(true);
            _TextChat.SetActive(true);
            _Chat.SetActive(true);
            _Send.SetActive(true);
            _ScrollView.SetActive(true);

        }
       
        public void TextChatSend()
        {             
            TextChat = ChatText.text;
            ChatText.text = "";
            websocket.Send(TextChat);
            UserCheck++;
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
            
            DisplayChat.text += "\n" + messageEventArgs.Data;
            Debug.Log("Message from server : " + messageEventArgs.Data);

            
            if (UserCheck == 0)
            {
                DisplayChat.alignment = TextAnchor.LowerLeft;
            }
            
        }
       
    }
}