//helps responsive design
document.getElementById('mobileMenu').style.height = window.innerHeight - 70;

function toggleMobileMenu() {
    $('#mobileMenu').toggleClass('open');
    $('body').toggleClass('overflow-hidden');
    $('#mobileMenuToggler').toggleClass('open');
}
document.getElementById("topNavOpenSearchButton").addEventListener('click', () => {
    document.getElementById("topNav-search")
    var searchForm = document.getElementById("topNav-search");
    console.log(searchForm);
    if (searchForm.clientWidth == 0) {
        searchForm.style.width = 525+"px";
    }
    else {
        searchForm.style.width = 0;
    }
})