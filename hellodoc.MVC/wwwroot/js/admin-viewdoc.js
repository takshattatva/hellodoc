function updateFileName(input) {
    var fileName = input.files[0].name;
    document.getElementById('upload-name').value = fileName;
}

$('.downloadAll').click(function () {
    $('.fileCheckbox:checked').each(function () {
        var filePath = $(this).closest('tr').find('.download-btn')[0].click();
    });
});

$('.deleteAll').click(function () {
    $('.fileCheckbox:checked').each(function () {
        var filePath = $(this).closest('tr').find('.delete-btn')[0].click();
    });
});

$('.mailAll').click(function () {
    var requstWiseFileId = [];
    var requestId = $('#requestId').val();
    var status = $('#statusForName').val();
    var callId = $('#callId').val();
    $('.fileCheckbox:checked').each(function () {
        var fileId = $(this).val();
        requstWiseFileId.push(fileId);
    });
    SendFile(requstWiseFileId, requestId, status, callId);
});

$(document).ready(function () {
    $('#inputGroupFile').on('change', function () {
        var inputValue = $('#inputGroupFile').val();
        console.log(inputValue);
        if (inputValue != "") {
            $("#upload-btn").removeClass("d-none");
        }
        else {
            $("#upload-btn").addClass("d-none");
        }
    });
});
