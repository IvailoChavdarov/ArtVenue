const CLOUDINARY_URL = 'https://api.cloudinary.com/v1_1/ddo3vrwcb/image/upload';
const CLOUDINARY_UPLOAD_PRESET = 'k4nawbiw';
const image = document.querySelector('#imageUpload');
const background = document.querySelector('#backgroundUpload');
const groupForm = document.getElementById("groupForm");
document.getElementById('groupFormButton').addEventListener('click', (e) => {
    e.preventDefault();
    if (image.files.length > 0 || background.files.length > 0) {
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
                        document.getElementById('groupImageInput').value = uploadedFileUrl;
                    }
                }).then(() => {
                    if (background.files.length > 0) {
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
                                    document.getElementById('groupBackgroundInput').value = uploadedFileUrl;
                                }
                            }).then(() => {
                                groupForm.submit();
                            })
                    }
                    else {
                        groupForm.submit();
                    }
                })
        }
        else {
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
                        document.getElementById('groupBackgroundInput').value = uploadedFileUrl;
                    }
                }).then(() => {
                    groupForm.submit();
                })
        }
    }
    else {
        groupForm.submit();
    }

})
image.addEventListener('change', (e) => {
    if (e.target.files[image.files.length - 1].name.endsWith(".png") || e.target.files[image.files.length - 1].name.endsWith(".jpg") || e.target.files[image.files.length - 1].name.endsWith(".jpeg") | e.target.files[image.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 1100000) {
            document.getElementById('groupImageInput').value = e.target.files[0].name
            document.getElementById('imageValidationText').innerText = ""
        }
        else {
            document.getElementById('imageValidationText').innerText = "Image is too large"
            image.value = null;
            document.getElementById('groupImageInput').value = ""
        }
    }
    else {
        document.getElementById('imageValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        image.value = null;
        document.getElementById('groupImageInput').value = ""
    }
})


background.addEventListener('change', (e) => {
    if (e.target.files[background.files.length - 1].name.endsWith(".png") || e.target.files[background.files.length - 1].name.endsWith(".jpg") || e.target.files[background.files.length - 1].name.endsWith(".jpeg") | e.target.files[background.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 1100000) {
            document.getElementById('groupBackgroundInput').value = e.target.files[0].name
            document.getElementById('backgroundValidationText').innerText = ""
        }
        else {
            document.getElementById('backgroundValidationText').innerText = "Image is too large"
            background.value = null;
            document.getElementById('groupBackgroundInput').value = ""
        }
    }
    else {
        document.getElementById('backgroundValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        background.value = null;
        document.getElementById('groupBackgroundInput').value = ""
    }
})