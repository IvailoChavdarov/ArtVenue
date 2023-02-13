//choses where to return on pressing return page
//main page if user comes from other website or directly from browser
//last page if user comes from other ArtVenue page
if (document.referrer) {
    if (document.referrer == window.location.href) {
        document.getElementById("backbutton").href = "/posts/index"
    }
    else {
        document.getElementById("backbutton").href = document.referrer;
    }
}
else {
    document.getElementById("backbutton").href = "/posts/index"
}

//details gallery slides managing
let slideIndex = 1;
showSlides(slideIndex);

function plusSlides(n) {
    showSlides(slideIndex += n);
}

function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("gallerySlide");
    let dots = document.getElementsByClassName("demo");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
}