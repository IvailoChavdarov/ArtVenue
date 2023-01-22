"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
document.getElementsByClassName("msger-chat")[0].scrollTop = document.getElementsByClassName("msger-chat")[0].scrollHeight
//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var msgContainer = document.createElement("div");
    msgContainer.classList.add("msg", "left-msg");

    var msgImg = document.createElement("div");
    msgImg.classList.add("msg-img")
    msgImg.style.backgroundImage = `url('${message.senderProfileImage}')`

    var msgBubble = document.createElement("div");
    msgBubble.classList.add("msg-bubble")

    var msgInfo = document.createElement("div");
    msgInfo.classList.add("msg-info")

    var msgInfoName = document.createElement("div");
    msgInfoName.classList.add("msg-info-name")
    msgInfoName.textContent = message.senderName

    var msgInfoTime = document.createElement("div");
    msgInfoTime.classList.add("msg-info-time")
    msgInfoTime.textContent = message.sendTime

    var msgText = document.createElement("div");
    msgText.classList.add("msg-text")
    msgText.textContent = message.messageContent

    msgInfo.appendChild(msgInfoName);
    msgInfo.appendChild(msgInfoTime);

    msgBubble.appendChild(msgInfo)
    msgBubble.appendChild(msgText)

    msgContainer.appendChild(msgImg)
    msgContainer.appendChild(msgBubble)

    document.getElementsByClassName("msger-chat")[0].appendChild(msgContainer)
    document.getElementsByClassName("msger-chat")[0].scrollTop = document.getElementsByClassName("msger-chat")[0].scrollHeight
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    if (inGroupChat) {
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

        var msgContainer = document.createElement("div");
        msgContainer.classList.add("msg", "right-msg");
        var msgBubble = document.createElement("div");
        msgBubble.classList.add("msg-bubble")
        var msgText = document.createElement("div");
        msgText.classList.add("msg-text")
        msgText.textContent = messageContent
        msgBubble.appendChild(msgText)
        msgContainer.appendChild(msgBubble)
        document.getElementsByClassName("msger-chat")[0].appendChild(msgContainer)
        document.getElementsByClassName("msger-chat")[0].scrollTop = document.getElementsByClassName("msger-chat")[0].scrollHeight
    }
    else if (inGroupChat) {
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        connection.invoke("SendMessageToGroup", message, ChatId).catch(function (err) {
            return console.error(err.toString());
        });
    }
    document.getElementById("messageInput").value = "";
    event.preventDefault();
});