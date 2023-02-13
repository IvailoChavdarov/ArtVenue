//copies discord connection name for footer
function copyDiscordName() {
    var button = document.getElementById('discordLink')
    if (!button.classList.contains('used')) {
        navigator.clipboard.writeText('Ивайло Чавдаров#5796')
        button.classList.add('used')
        var popup = document.getElementById("discordPopup");
        popup.classList.toggle("show");
        setTimeout(() => {
            button.classList.remove('used');
            popup.classList.toggle("show");
        }, 2500)
    }
}