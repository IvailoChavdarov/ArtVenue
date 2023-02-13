//shows loader on click of button or link that does not have class NoLoader
$("a").not(".NoLoader").not(".close").click(function () {
    showLoader()
});

$("button").not(".NoLoader").not(".close").click(function () {
    showLoader()
});

function hideLoader() {
    document.getElementById("loaderContainer").style.display = "none"
}

function showLoader() {
    document.getElementById("loaderContainer").style.display = "block"
}

hideLoader()