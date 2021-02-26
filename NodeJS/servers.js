const app = require('express')();
const server = require('http').Server(app);
var websocket = require('ws');
const sqlite = require('sqlite3').verbose();

var wss = new websocket.Server({port:48484}, ()=>{
    console.log("Nasu Server is running");
});

var database = new sqlite.Database('./database/dbchat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>
{
    if(err) throw err;
    var wsList = [];
    var roomList = [];
    
    wss.on("connection", (ws)=>
    {
    
        //Lobby
        console.log("client connected.");
        //Reception
        ws.on("message", (data)=>
        {
            
            console.log("send from client :"+ data);

            //========== Convert jsonStr into jsonObj =======

            //toJsonObj = JSON.parse(data);

            // I change to line below for prevent confusion
            var toJsonObj = 
            { 
                eventName:"Login",
                data:"bot01#1234#Nasu"
            }
            toJsonObj = JSON.parse(data);
            //===============================================

   /*var dataFromClient =
    {
        eventName:"",
        data:""
    }
    dataFromClient = JSON.parse(data)*/

            var splitStr = toJsonObj.data.split('#');
            var userID = splitStr[0];
            var password = splitStr[1];
            var name = splitStr[2];

            var sqlInsert = "INSERT INTO UserData (userID, password, name ) VALUES ('"+userID+"', '"+password+"', '"+name+"')";//Register
            var sqlSelect = "SELECT * FROM UserData WHERE userID='"+userID+"' AND password='"+password+"'";//Login
            //var sqlSelect = "SELECT * FROM UserData WHERE userID='test888' AND password='888888'";

            if(toJsonObj.eventName == "Register")
            {
                //Register
                database.all(sqlInsert,(err, rows)=>
                {
                    if(err)
                    {
                        var callbackMsg = 
                        {
                            eventName:"Register",
                            data:"fail"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send (toJsonStr);
                        console.log("[0]" +toJsonStr);
                    }
                    else
                    {
                        var callbackMsg = 
                        {
                            eventName:"Register",
                            data:"Success"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send (toJsonStr);
                        console.log("[1]" +toJsonStr);
                        console.log(rows);
                    }
                });
            }
            
            if(toJsonObj.eventName == "Login")
            {
                //Login
                database.all(sqlSelect,(err, rows)=>
                {
                    if(err)
                    {
                        console.log(err);
                    }
                    else
                    {
                        if(rows.length > 0)
                        {
                            console.log(rows);
                            var callbackMsg = 
                            {
                                eventName:"Login",
                                data:"success#"+rows[0].name

                            }
                            console.log("Login Success")
                            var toJsonStr = JSON.stringify(callbackMsg);
                            ws.send(toJsonStr);
                            console.log("[2]" +toJsonStr);
                            
                        }
                        else
                        {
                            var callbackMsg = 
                            {
                                eventName:"Login",
                                data:"fail"
                            }
                            var toJsonStr = JSON.stringify(callbackMsg);
                            ws.send(toJsonStr);
                            console.log("[3]" +toJsonStr);
                            
                        }                    
                    }
                });
            }
            

            if(toJsonObj.eventName == "CreateRoom")//CreateRoom
            {
            //============= Find room with roomName from Client =========
                var isFoundRoom = false;
            
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }
                //===========================================================
           
                if(isFoundRoom == true)// Found room
                {
                //Can't create room because roomName is exist.
                //========== Send callback message to Client ============

                    ws.send("CreateRoomFail"); 

                //I will change to json string like a client desi. Please see below
                    var callbackMsg = 
                    {
                        eventName:"CreateRoom",
                        data:"fail"
                    }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                    //=======================================================

                        console.log("client create room fail.");
            }
            else
            {
                //============ Create room and Add to roomList ==========
                var newRoom = 
                {
                    roomName: toJsonObj.data,
                    wsList: []
                }

                newRoom.wsList.push(ws);

                roomList.push(newRoom);
                //=======================================================

                //========== Send callback message to Client ============

                //ws.send("CreateRoomSuccess");

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = 
                {
                    eventName:"CreateRoom",
                    data:"Success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================
                console.log("client create room success.");
            }

            console.log("client request CreateRoom ["+toJsonObj.data+"]");
            
        }
        else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
        {
            //============= Home work ================
                // Implementation JoinRoom event when have request from client.
    
                var isJoinRoom = false;
    
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        if(roomList[i].roomName == toJsonObj.data)
                        {
                            isJoinRoom = true;
                            roomList[i].wsList.push(ws);
                            break;
                        }
                    }
                }
    
                if (isJoinRoom == true)
                    {
                        var callbackMsg = 
                        {
                            eventName:"JoinRoom",
                            data:"success",
                        }
                        //===============================================
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //=======================================================

                        console.log("client join room success.");
                        console.log("client request JoinRoom ["+toJsonObj.data+"]");
                    }
                    else
                    {
                        var callbackMsg = 
                        {
                            eventName:"JoinRoom",
                            data:"fail",
                        }
                        //===============================================
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //===============================================
                        console.log("client join room fail.");
                    }
                
                //================= Hint =================
                //roomList[i].wsList.push(ws);
    
                console.log("client request JoinRoom");
                //========================================
        }
        else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
        {
            //============ Find client in room for remove client out of room ================
            var isLeaveSuccess = false;//Set false to default.
            for(var i = 0; i < roomList.length; i++)//Loop in roomList
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                {
                    if(ws == roomList[i].wsList[j])//If founded client.
                    {
                        roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                        if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                        {
                            roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            //===============================================================================

            if(isLeaveSuccess)
            {
                //========== Send callback message to Client ============

                ws.send("LeaveRoomSuccess");

                //I will change to json string like a client side. Please see below
                var callbackMsg = 
                {
                    eventName:"LeaveRoom",
                    data:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room success");
            }
            else
            {
                //========== Send callback message to Client ============

                ws.send("LeaveRoomFail");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room fail");
            }
        }
        else if(toJsonObj.eventName == "SendMessage")
        {
            console.log("send MSG from client" +data);
            Boardcast(ws, data);
            
        }
    });
    
        ws.on("close", ()=>
        {
            console.log("client disconnected.");

        //============ Find client in room for remove client out of room ================
            for(var i = 0; i < roomList.length; i++)//Loop in roomList
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                {
                    if(ws == roomList[i].wsList[j])//If founded client.
                    {
                        roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                        if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                        {
                            roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                        }
                        break;
                    }
                }
            }    
        //===============================================================================
        });

});
    function Boardcast(ws, message)
    {   
        var selectRoomIndex = -1;
        for(var i = 0; i < roomList.length; i++)
        {
            for(var j = 0; j < roomList[i].wsList.length; j++ )
            {
                if(ws == roomList[i].wsList[j])
                {
                    selectRoomIndex = i;
                    break;
                }
            }
        }   

        for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
        {
            roomList[selectRoomIndex].wsList[i].send(message);
        }
    }
});
