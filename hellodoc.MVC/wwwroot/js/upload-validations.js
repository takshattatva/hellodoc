function validateAndUpdateFileName(input) {
    var file = input.files[0];
    var fileName = file.name;
    var fileExtension = fileName.split('.').pop().toLowerCase();
    var validExtensions = ['pdf', 'doc', 'docx'];
    var maxFileSize = 10 * 1024 * 1024; // 10MB in bytes

    if (validExtensions.includes(fileExtension) && file.size <= maxFileSize) 
    {
        document.getElementById('upload-name').value = fileName;
    }
    else
    {
        if (!validExtensions.includes(fileExtension)) {
            Swal.fire({
                title: 'Invalid file type!',
                text: 'Please select a .pdf or .doc/.docx file.',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
        }

        if (file.size > maxFileSize) {
            Swal.fire({
                title: 'File size exceeded!',
                text: 'The file size must be less than 10MB.',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
        }

        input.value = '';
        document.getElementById('upload-name').value = 'Choose File...';
    }
}
/**********************************************************************************************************************************************/
