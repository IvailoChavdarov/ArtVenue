"use strict"

//hides sidenav on chat pages until opened on mobile

var directChatButton = document.getElementById("directChatButton")
var groupChatButton = document.getElementById("groupChatButton")
var directChats = document.getElementById("directChatsContainer")
var groupChats = document.getElementById("groupChatsContainer")

directChatButton.addEventListener('click', () => {
    if (groupChatButton.classList.contains("active")) {
        groupChatButton.classList.remove("active")
    }
    directChatButton.classList.add("active")

    if (directChats.classList.contains("hidden")) {
        directChats.classList.remove("hidden")
    }
    groupChats.classList.add("hidden")
})

groupChatButton.addEventListener('click', () => {
    if (directChatButton.classList.contains("active")) {
        directChatButton.classList.remove("active")
    }
    groupChatButton.classList.add("active")

    if (groupChats.classList.contains("hidden")) {
        groupChats.classList.remove("hidden")
    }

    directChats.classList.add("hidden")
})

if (window.innerWidth <= 720) {
    document.getElementsByClassName("msger-sideBar")[0].style.display = "none";
}

function toggleChatsNav() {
    var chatNav = document.getElementsByClassName("msger-sideBar")[0];
    if (chatNav.style.display == "none") {
        chatNav.style.display = "block"
    }
    else {
        chatNav.style.display = "none"
    }
}