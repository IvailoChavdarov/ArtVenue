const CLOUDINARY_URL = 'https://api.cloudinary.com/v1_1/ddo3vrwcb/image/upload';
const CLOUDINARY_UPLOAD_PRESET = 'k4nawbiw';
const image = document.querySelector('#fileupload');
    document.getElementById('registerButton').addEventListener('click', (e) => {
    e.preventDefault();
        if (image.files.length > 0) {
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
                        const uploadedFileUrl = data.secure_url;
                        document.getElementById('profileImageInput').value = uploadedFileUrl;
                    }
                }).then(() => {
                 document.getElementById('submitButtonHidden').click()
                })
            /*                    .catch(err => console.error(err));*/
        }
        else {
            document.getElementById('submitButtonHidden').click()
        }
            })
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
