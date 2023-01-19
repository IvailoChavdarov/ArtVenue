//drag scrolling post slider
const sliders = document.querySelectorAll('.gallery');
sliders.forEach(function (slider) {
    let isDown = false;
    let startX;
    let scrollLeft;

    slider.addEventListener('mousedown', (e) => {
        isDown = true;
        slider.classList.add('active');
        startX = e.pageX - slider.offsetLeft;
        scrollLeft = slider.scrollLeft;
    });

    slider.addEventListener('mouseleave', () => {
        isDown = false;
        slider.classList.remove('active');
    });

    slider.addEventListener('mouseup', () => {
        isDown = false;
        slider.classList.remove('active');
    });

    slider.addEventListener('mousemove', (e) => {
        if (!isDown) return;  // stop the fn from running
        e.preventDefault();
        const x = e.pageX - slider.offsetLeft;
        const walk = (x - startX) * 2;
        slider.scrollLeft = scrollLeft - walk;

    });

    slider.addEventListener('touchstart', (e) => {
        isDown = true;
        slider.classList.add('active');
        startX = e.pageX - slider.offsetLeft;
        scrollLeft = slider.scrollLeft;
    });

    slider.addEventListener('touchend', () => {
        isDown = false;
        slider.classList.remove('active');
    });

    slider.addEventListener('touchcancel', () => {
        isDown = false;
        slider.classList.remove('active');
    });

    slider.addEventListener('touchmove', (e) => {
        if (!isDown) return;  // stop the fn from running
        e.preventDefault();
        const x = e.pageX - slider.offsetLeft;
        const walk = (x - startX) * 2;
        slider.scrollLeft = scrollLeft - walk;
    });
})

//resize post images
const postResource = document.querySelectorAll('.optionalMedia img, .optionalMedia video, .optionalMedia iframe')
postResource.forEach((resource) => {
    if (resource.clientWidth > resource.clientHeight) {
        resource.style.width = "600px"
    }
    else {
        resource.style.height = "600px"
    }
})

const closeChatButtons = document.querySelectorAll('.closeChatButton')
console.log(closeChatButtons);
closeChatButtons.forEach((btn) => {
    btn.addEventListener('click', () => {
        var postContainer = btn.parentElement.parentElement;
        if (postContainer.classList.contains("hiddenComments")) {
            postContainer.classList.remove("hiddenComments")
        }
        else {
            postContainer.classList.add("hiddenComments")
        }
        
    })
})
