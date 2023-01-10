document.getElementById('mobileMenu').style.height = window.innerHeight - 70;

function toggleMobileMenu() {
    $('#mobileMenu').toggleClass('open');
    $('body').toggleClass('overflow-hidden');
    $('#mobileMenuToggler').toggleClass('open');
}
if (document.getElementsByTagName('body')[0].clientHeight < window.innerHeight) {
    document.getElementById('pageFooter').style.position = "absolute"

}