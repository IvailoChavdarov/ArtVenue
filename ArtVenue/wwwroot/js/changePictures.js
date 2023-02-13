//connections to the Cloudinary API
const CLOUDINARY_URL = 'https://api.cloudinary.com/v1_1/ddo3vrwcb/image/upload';
const CLOUDINARY_UPLOAD_PRESET = 'k4nawbiw';

//images inputs
const image = document.querySelector('#fileupload');
const background = document.querySelector('#backgroundUpload');

//submitting form containing images uploads
document.getElementById('update-profile-button').addEventListener('click', (e) => {
    e.preventDefault();

    //checks if images are uploaded or connected by url
    if (image.files.length > 0 || background.files.length > 0) {
        if (image.files.length > 0) {

            //uploads profile image to cloudinary and sets input value to posted image url
            const file = image.files[image.files.length - 1];
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
                        //sets image url upload to uploaded image url
                        const uploadedFileUrl = data.secure_url;
                        document.getElementById('profileImageInput').value = uploadedFileUrl;
                    }
                }).then(() => {
                    //checks if there is another image to upload
                    if (!background.files.length > 0) {
                        document.getElementById('profile-form').submit()
                    }

                })
        }
        if (background.files.length > 0) {
            //uploads account cover image to cloudinary and sets input value to posted image url
            const file = background.files[background.files.length - 1];
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
                        document.getElementById('profileBackgroundInput').value = uploadedFileUrl;
                    }
                }).then(() => {
                    document.getElementById('profile-form').submit()
                })
        }
    }
    else {
        document.getElementById('profile-form').submit()
    }

})

//validates the uploaded profile image
image.addEventListener('change', (e) => {
    if (e.target.files[image.files.length - 1].name.endsWith(".png") || e.target.files[image.files.length - 1].name.endsWith(".jpg") || e.target.files[image.files.length - 1].name.endsWith(".jpeg") | e.target.files[image.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 1100000) {
            document.getElementById('profileImageInput').value = e.target.files[0].name
            document.getElementById('imageValidationText').innerText = ""
        }
        else {
            document.getElementById('imageValidationText').innerText = "Image is too large"
            image.value = null;
            document.getElementById('profileImageInput').value = ""
        }
    }
    else {
        document.getElementById('imageValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        image.value = null;
        document.getElementById('profileImageInput').value = ""
    }
})

//validates the uploaded account cover image
background.addEventListener('change', (e) => {
    if (e.target.files[image.files.length - 1].name.endsWith(".png") || e.target.files[image.files.length - 1].name.endsWith(".jpg") || e.target.files[image.files.length - 1].name.endsWith(".jpeg") | e.target.files[image.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 1100000) {
            document.getElementById('profileBackgroundInput').value = e.target.files[0].name
            document.getElementById('backgroundValidationText').innerText = ""
        }
        else {
            document.getElementById('backgroundValidationText').innerText = "Image is too large"
            image.value = null;
            document.getElementById('profileBackgroundInput').value = ""
        }
    }
    else {
        document.getElementById('backgroundValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        image.value = null;
        document.getElementById('profileBackgroundInput').value = ""
    }
})