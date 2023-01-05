﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    console.log(message);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${message.senderName} says ${message.messageContent} at ${message.sendTime}`;
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

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var messageContent = document.getElementById("messageInput").value;
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    var reciever = document.getElementById("recieverIdInput").value;
    today = mm + '/' + dd + '/' + yyyy;
    console.log(user);
    console.log(message);
    console.log(today);
    console.log("reciever is:" + reciever);
    if (reciever != "" && reciever) {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        console.log(message);
        connection.invoke("SendMessageToUser", message, reciever).catch(function (err) {
            return console.error(err.toString());
        });
        console.log("Sent to user");
    }
    else if (inGroupChat) {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        console.log(message);
        connection.invoke("SendMessageToGroup", message, ChatId).catch(function (err) {
            return console.error(err.toString());
        });
        console.log("Sent to group");
    }
    else {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        console.log(message);
        connection.invoke("SendMessage", message).catch(function (err) {
            return console.error(err.toString());
        });
    }

    event.preventDefault();
});