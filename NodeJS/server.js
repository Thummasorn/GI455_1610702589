var websocket = require('ws');

var websocketServer = new websocket.Server({port:48484}, ()=>{
    console.log("Nasu Server is running");
});

var wsList = [];
var roomList = [];

websocketServer.on("connection", (ws, rq)=>
{
    // Lobby
    {
        console.log('client connected');
        //Reception
        ws.on("message", (data)=>{
            console.log("send from client : "+data);

            var toJsonObj = JSON.parse(data)

            if (toJsonObj.eventName == "CreateRoom")//CreateRoom
            {
                
                var isFoundRoom = false;

                for (var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if (isFoundRoom == true){
                    //callback to client create room fail
                    ws.send("CreateRoom Fail")

                    console.log("client create room fail.")
                }
                else
                {
                    //callback to client : create room success

                    var newRoom =
                    {
                        roomName: toJsonObj.roomName,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);

                    roomList.push(newRoom);

                    ws.send("CreateRoomSuccess")

                    console.log("client create room success")
                }

                console.log("client request CreateRoom ["+toJsonObj.roomName+"]")

            }
            else if (toJsonObj.eventName == "JoinRoom")// JoinRoom Homework
            {
                console.log("client request Joinroom")
            } 
            
            else if (toJsonObj.eventName == "LeaveRoom")
            {
                var isLeaveSuccess = false;

                for ( var i = 0; i < roomList.length; i++)
                {
                    for (var j = 0; j < roomList[i].wsList.length; j++)
                    {
                        if (ws == roomList[i].wsList[j])
                        {
                            roomList[i].wsList.splice(j,1);    

                            if (roomList[i].wsList.length <= 0)
                            {
                                roomList[i].splice(i,1);
                                
                            }    
                            isLeaveSuccess = true;
                            break;                        
                        }
                    }
                }

                if (isisLeaveSuccess)
                {
                    ws.send("LeaveRoomSuccess");
                    console.log("leave room success");
                }
                else
                {
                    ws.send("LeaveRoomFail");
                    console.log("leave room fail");
                }
            } // LeaveRoom

            console.log("client request Createroom [" + toJsonObj.roomName + "]")
        });
    }
    wsList.push(ws);

    ws.on("message", (data)=>{
        console.log("send from client : "+data);
        Boardcast(data);
    });
    
    ws.on("close", ()=>
    {
        wsList = ArrayRemove(wsList, ws);

        console.log("Client Disconnected");

        for ( var i = 0; i < roomList.length; i++)
        {
            for (var j = 0; j < roomList[i].wsList.length; j++)
            {
                if (ws == roomList[i].wsList[j])
                {
                    roomList[i].wsList.splice(j,1);
                    
                    if (roomList[i].wsList.length <= 0)
                    {
                        roomList[i].splice(i,1);
                    }                           

                    break;
                }
            }
        }
    });
});

function ArrayRemove(arr, value){
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(data){
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}