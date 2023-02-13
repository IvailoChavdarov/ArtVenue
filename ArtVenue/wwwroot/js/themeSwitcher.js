//changes between light and dark theme for the website and stores user preference in localStorage
var theme = localStorage.getItem("theme")
function switchTheme() {
    if (theme == "dark") {
        localStorage.setItem("theme", "light")
        document.querySelector("body").classList.remove("theme-dark")
        document.getElementById('themeIdenticator').classList.remove("fa-moon")
        document.getElementById('themeIdenticator').classList.add("fa-sun")
        document.getElementById('themeIdenticatorMobile').classList.remove("fa-moon")
        document.getElementById('themeIdenticatorMobile').classList.add("fa-sun")
        theme = "light"
    }
    else {
        localStorage.setItem("theme", "dark")
        document.querySelector("body").classList.add("theme-dark")
        document.getElementById('themeIdenticator').classList.remove("fa-sun")
        document.getElementById('themeIdenticator').classList.add("fa-moon")
        document.getElementById('themeIdenticatorMobile').classList.remove("fa-sun")
        document.getElementById('themeIdenticatorMobile').classList.add("fa-moon")
        theme = "dark"
    }
}
function handleIcon() {
    if (theme == "dark") {
        document.getElementById('themeIdenticator').classList.add("fa-moon")
        document.getElementById('themeIdenticatorMobile').classList.add("fa-moon")
        document.querySelector("body").classList.add("theme-dark")
    }
    else {
        document.getElementById('themeIdenticator').classList.add("fa-sun")
        document.getElementById('themeIdenticatorMobile').classList.add("fa-sun")
    }
}
handleIcon()