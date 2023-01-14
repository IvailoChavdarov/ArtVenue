"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    li.textContent = `${message.senderName} says ${message.messageContent} at ${message.sendTime}`;
    document.getElementById("messagesList").appendChild(li);
    console.log(message);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    if (inGroupChat) {
        console.log("ChatId is " + ChatId);
        connection.invoke("AddToGroupInstantChat", ChatId).catch(function (err) {
            return console.error(err.toString());
        });
    }
}).catch(function (err) {
    return console.error(err.toString());
});

function getDate() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    today = mm + '/' + dd + '/' + yyyy;
    return today;
}

function sendDirectMessageToUser() {
    console.log("Right around here");
    var user = document.getElementById("userInput").value;
    var messageContent = document.getElementById("messageInput").value;
    var reciever = document.getElementById("recieverIdInput").value;
    var message = { SenderId: user, MessageContent: messageContent, Send_Time: getDate() }
    connection.invoke("SendMessageToUser", message, reciever).catch(function (err) {
        return console.error(err.toString());
    });
}

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var messageContent = document.getElementById("messageInput").value;
    var reciever = document.getElementById("recieverIdInput").value;
    var today = getDate();
    if (reciever != "" && reciever) {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        connection.invoke("SendMessageToUser", message, reciever).catch(function (err) {
            return console.error(err.toString());
        });
        var li = document.createElement("li");
        li.textContent = `${messageContent}  ${today}`;
        document.getElementById("messagesList").appendChild(li);
    }
    else if (inGroupChat) {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        connection.invoke("SendMessageToGroup", message, ChatId).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});