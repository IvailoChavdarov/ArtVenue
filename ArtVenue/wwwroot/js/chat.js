"use strict";
//establishes connection for instant messaging between user
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//sets chat box height
document.getElementsByClassName("msger-chat")[0].scrollTop = document.getElementsByClassName("msger-chat")[0].scrollHeight

//disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

//on recieving message from server
connection.on("ReceiveMessage", function (message) {
    //checks if message is in group chat(user recieves his messages from server if in group too)
    if (inGroupChat) {
        //renders message from sender with sender styling
        if (message.senderId == currentUserId) {
            console.log("goes here");
            var msgContainer = document.createElement("div");
            msgContainer.classList.add("msg", "right-msg");
            var msgBubble = document.createElement("div");
            msgBubble.classList.add("msg-bubble")
            var msgText = document.createElement("div");
            msgText.classList.add("msg-text")
            msgText.textContent = message.messageContent
            msgBubble.appendChild(msgText)
            msgContainer.appendChild(msgBubble)
            document.getElementsByClassName("msger-chat")[0].appendChild(msgContainer)
            document.getElementsByClassName("msger-chat")[0].scrollTop = document.getElementsByClassName("msger-chat")[0].scrollHeight
            return;
        }
    }

    //renders message from other sender in the chat box
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

    if (inGroupChat) {
        //adds link to message sender if message is in group chat
        var senderProfileLink = document.createElement("a");
        senderProfileLink.href = "/posts/users/" + message.senderId;
        senderProfileLink.textContent = message.senderName;
        msgInfoName.appendChild(senderProfileLink)
    }
    else {
        msgInfoName.textContent = message.senderName
    }

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
        //if in group chat the user connects to that group's instant chat
        connection.invoke("AddToGroupInstantChat", ChatId).catch(function (err) {
            return console.error(err.toString());
        });
    }
}).catch(function (err) {
    return console.error(err.toString());
});

//gets today's data for instant message sending
function getDate() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    today = mm + '/' + dd + '/' + yyyy;
    return today;
}

//sends message to server for saving to database and instant rendering to other user
function sendDirectMessageToUser() {
    var user = document.getElementById("userInput").value;
    var messageContent = document.getElementById("messageInput").value;
    var reciever = document.getElementById("recieverIdInput").value;
    var message = { SenderId: user, MessageContent: messageContent, Send_Time: getDate() }
    connection.invoke("SendMessageToUser", message, reciever).catch(function (err) {
        return console.error(err.toString());
    });
}

//sending of message
document.getElementById("sendButton").addEventListener("click", function (e) {
    //sets message data
    var user = document.getElementById("userInput").value;
    var messageContent = document.getElementById("messageInput").value;
    var reciever = document.getElementById("recieverIdInput").value;
    var today = getDate();


    if (inGroupChat) {
        //sends message to group chat for saving to database and rendering to all users in group chat at the moment
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        connection.invoke("SendMessageToGroup", message, ChatId).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        //sends message for saving to database and rendering for reciever
        var message = { SenderId: user, MessageContent: messageContent, Send_Time: today }
        connection.invoke("SendMessageToUser", message, reciever).catch(function (err) {
            return console.error(err.toString());
        });

        //renders user's message to himself
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
    document.getElementById("messageInput").value = "";
    e.preventDefault();
});