const CLOUDINARY_URL = 'https://api.cloudinary.com/v1_1/ddo3vrwcb/image/upload';
const CLOUDINARY_UPLOAD_PRESET = 'k4nawbiw';

const singleImageInput = document.querySelector('#singleImageUpload');
const multipleImagesInputUpload = document.querySelector('#multipleImagesUpload');
const multipleImagesInput = document.querySelectorAll(".multiple-image-input")
const videoInput = document.querySelector('#videoUpload');

//submit create post with single image form
document.getElementById('singleImageCreateButton').addEventListener('click', (e) => {
    e.preventDefault();
    var imageTextInput = document.getElementById('singleImageName')
    if (singleImageInput.files.length > 0) {
        const file = singleImageInput.files[singleImageInput.files.length - 1];
        const formData = new FormData();
        formData.append('file', file);
        formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET);

        fetch(CLOUDINARY_URL, {
            method: 'POST',
            body: formData,
        })
            .then(response => response.json())
            .then((data) => {
                if (data.secure_url !== '') {
                    const uploadedFileUrl = data.secure_url;
                    imageTextInput.value = uploadedFileUrl;
                }
            }).then(() => {
                document.getElementById('postSingleImagePublicationForm').submit()
            })
    }
    else {
        if (imageTextInput.value == "" || imageTextInput.value == null) {
            document.getElementById('singleImageValidationText').innerText = "Must add an image to an image publication"
        }
        else {
            document.getElementById('postSingleImagePublicationForm').submit()
        }
    }
})

//on change of single image input
singleImageInput.addEventListener('change', (e) => {
    if (e.target.files[singleImageInput.files.length - 1].name.endsWith(".png") || e.target.files[singleImageInput.files.length - 1].name.endsWith(".jpg") || e.target.files[singleImageInput.files.length - 1].name.endsWith(".jpeg") || e.target.files[singleImageInput.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 1100000) {
            document.getElementById('singleImageName').value = e.target.files[0].name
            document.getElementById('singleImageValidationText').innerText = ""
        }
        else {
            document.getElementById('singleImageValidationText').innerText = "Image is too large"
            singleImageInput.value = null;
            document.getElementById('singleImageName').value = ""
        }
    }
    else {
        document.getElementById('singleImageValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        singleImageInput.value = null;
        document.getElementById('singleImageName').value = ""
    }
})

//submit create post with multiple images form
document.getElementById('multipleImagesCreateButton').addEventListener('click', (e) => {
    e.preventDefault();
    if (multipleImagesInputUpload.files.length > 0) {
        const files = multipleImagesInputUpload.files;
        var counter = 0;
        console.log(Array.from(files));
        Array.from(files).forEach((file) => {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET);
            fetch(CLOUDINARY_URL, {
                method: 'POST',
                body: formData,
            })
            .then(response => response.json())
                .then((data) => {
                if (data.secure_url !== '') {
                    const uploadedFileUrl = data.secure_url;
                    console.log(counter + uploadedFileUrl);
                    document.getElementById('multipleImagesInput-' + counter).value = uploadedFileUrl;
                }
            })
                .then(() => {
                if (counter == files.length - 1) {
                    document.getElementById('postMultipleImagePublicationForm').submit()
                    }
                    counter++;
            })

        })
    }
    else {
        document.getElementById('postMultipleImagePublicationForm').submit()
    }
})

//on change of multiple images input
multipleImagesInputUpload.addEventListener('change', (e) => {
    if (multipleImagesInputUpload.files.length > 5) {
        multipleImagesInputUpload.value = null;
        document.getElementById('multipleImagesValidationText').innerText = "Only 5 files are allowed in one publication"
    }
    else {
        for (var i = 0; i < multipleImagesInputUpload.files.length; i++) {
            if (multipleImagesInputUpload.files[i].name.endsWith(".png") || multipleImagesInputUpload.files[i].name.endsWith(".jpg") || multipleImagesInputUpload.files[i].name.endsWith(".jpeg") | multipleImagesInputUpload.files[i].name.endsWith(".gif")) {
                if (e.target.files[0].size < 1100000) {
                    document.getElementById('multipleImagesInput-' + i).value = multipleImagesInputUpload.files[i].name
                    document.getElementById('multipleImagesValidationText').innerText = ""
                }
                else {
                    document.getElementById('multipleImagesValidationText').innerText = "Image is too large"
                    multipleImagesInputUpload.value = null;
                    multipleImagesInput.forEach((el) => {
                        el.value = ""
                    })
                }
            }
            else {
                document.getElementById('multipleImagesValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
                multipleImagesInputUpload.value = null;
                multipleImagesInput.forEach((el) => {
                    el.value = ""
                })
            }
        }

    }
})

//submit create post with video form
document.getElementById('videoCreateButton').addEventListener('click', (e) => {
    e.preventDefault();
    var videoTextInput = document.getElementById('videoName');
    if (videoInput.files.length > 0) {
        const file = videoInput.files[videoInput.files.length - 1];
        const formData = new FormData();
        formData.append('file', file);
        formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET);
        fetch('https://api.cloudinary.com/v1_1/ddo3vrwcb/video/upload', {
            method: 'POST',
            body: formData,
        })
            .then(response => response.json())
            .then((data) => {
                if (data.secure_url !== '') {
                    const uploadedFileUrl = data.secure_url;
                    videoTextInput.value = uploadedFileUrl;
                }
            }).then(() => {
                document.getElementById('postVideoPublicationForm').submit()
            })
    }
    else {
        if (videoTextInput.value == "" || videoTextInput.value == null) {
            document.getElementById('videoValidationText').innerText = "Must add a video to a video publication"
        }
        else {
            document.getElementById('postVideoPublicationForm').submit()
        }
    }
})

//on change of video input
videoInput.addEventListener('change', (e) => {
    var videoTextInput = document.getElementById('videoName');
    if (e.target.files[videoInput.files.length - 1].name.endsWith(".mp4") || e.target.files[videoInput.files.length - 1].name.endsWith(".ogg") || e.target.files[videoInput.files.length - 1].name.endsWith(".webm")) {
        if (e.target.files[0].size < 20000000) {
            videoTextInput.value = e.target.files[0].name
            document.getElementById('videoValidationText').innerText = ""
        }
        else {
            document.getElementById('videoValidationText').innerText = "Video is too large"
            videoInput.value = null;
            videoTextInput.value = ""
        }
    }
    else {
        document.getElementById('videoValidationText').innerText = "Video must be in mp4, ogg or webm format"
        videoInput.value = null;
        videoTextInput.value = ""
    }
})


//manage forms to show
var formShowButtons = document.querySelectorAll(".activateFormButton")
formShowButtons.forEach((button) => {
    button.addEventListener('click', () => {
        var formToShowId = button.getAttribute('data-formToActivate')
        document.querySelectorAll('.postForm').forEach((ele) => {
            console.log(ele);
            if (ele.id === formToShowId) {
                console.log(ele);
                ele.style.display = "block"
                ele.classList.add("show")
            }
            else {
                ele.remove()
            }
        })
        document.querySelector("#activateFormButtonsContainer").classList.add("hide")
        setTimeout(() => {
            document.querySelector("#activateFormButtonsContainer").remove()
        }, 1000)
        
    })
})