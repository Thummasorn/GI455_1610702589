const sqlite = require('sqlite3').verbose();

var database = new sqlite.Database('./database/dbchat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>
{

    if(err) throw err;

    console.log("Connect to database");

    var userID = "test888";
    var password = "888888";
    var name = "test8";

    var sqlSelect = "SELECT * FROM userData WHERE UserID='test06' AND UserPassword='123456'"// Login
    var sqlInsert =  "INSERT INTO userData (userID, password, name) VALUES ('"+userID+"', '"+password+"', '"+name+"')";// Register
    var sqlUpdate = "UPDATE userData SET Money=500 WHERE UserID='"+(userID)+"'";

    var sqlAddMoney = "SELECT Money FROM UserData WHERE userID='"+(userID)+"'";

    database.all(sqlAddMoney, (err, rows)=>{
        if(err){
            console.log(err)
        }
        else{
            if(rows.length > 0){
                //console.log("Log in Success")
                var currentMoney = rows[0].Money;
                currentMoney += 200;

                database.all("UPDATE UserData SET Money='"+currentMoney+"' WHERE userID='"+userID+"'", (err, rows)=>{
                    if(err){
                        console.log("Add Money Fail");
                    }
                    else{
                        var result ={
                            eventName: "Add Money",
                            data: currentMoney
                        }

                        console.log(JSON.stringify(result));
                    }
                })
            }
            else{
                //console.log("Log in Fail")
                console.log("UserID Not Found!")
            }
            console.log(rows);
        }
    });
});