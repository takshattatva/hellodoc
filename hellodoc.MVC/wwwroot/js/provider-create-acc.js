function getcoordinates() {
    event.preventDefault();
    var postalCode = document.getElementById('physicianZipCode').value;
    fetchCoordinates(postalCode);
}

function fetchCoordinates(postalCode) {
    var apiUrl = 'https://nominatim.openstreetmap.org/search?postalcode=' + postalCode + '&format=json';

    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            if (data.length > 0) {
                var lat = data[0].lat;
                var lon = data[0].lon;
                console.log(lat);
                console.log(lon);
                document.getElementById("longitude").setAttribute("value", lon);
                document.getElementById("latitude").setAttribute("value", lat);
            } else {
                Swal.fire("Oops", "Please Enter Valid PostalCode", "error");
            }
        })
        .catch(error => {
            Swal.fire("Oops", "Please Enter Valid PostalCode", "error");
        });
}

var input = document.querySelector("#telephone6");
window.intlTelInput(input, {
    separateDialCode: true,
    preferredCountries: ["in", "us", "ca"],
});
var input = document.querySelector("#telephone7");
window.intlTelInput(input, {
    separateDialCode: true,
    preferredCountries: ["in", "us", "ca"],
});

function checkEmail() {
    var email = $('#ProfileInputemail').val();

    $.ajax({
        url: '@Url.Action("EmailCheckForPhysician", "Admin")',
        type: "post",

        async: true,
        datatype: 'html',
        cache: false,
        data: { email: email },
        success: function (result) {
            if (!result) {
                $("#emailcheckerror").addClass("d-none");
            }
            else {
                $("#emailcheckerror").removeClass("d-none");
            }
        },
        error: function () {
            Swal.fire("Oops", "Please Enter Valid PostalCode", "error");
        }
    });
};

$(document).ready(function () {
    $('.onBoardInpFile').change(function () {
        var checkbox = $(this).closest('.onBoardContainer').find('.Onboarding');
        var buttonId = checkbox.prop('value');
        var file = this.files[0];

        if (file) {
            var blobUrl = URL.createObjectURL(file);

            checkbox.prop('checked', true);
            $('#OnboardingView' + buttonId).show();
            $('#OnboardingView' + buttonId).prop('href', blobUrl);
        }
        else {
            checkbox.prop('checked', false);
            $('#OnboardingView' + buttonId).hide();
        }
    });
});

$(document).ready(function () {
    $('#fileUploadInp').on('input', function () {
        var inputValue = $('#fileUploadInp').val();

        if (inputValue != "") {
            $("#upload-btn").prop("disabled", false);
        }
        else {
            $("#upload-btn").prop("disabled", true);
        }
    });
});

$(document).ready(function () {
    $('#fileUploadInp2').on('input', function () {
        var inputValue = $('#fileUploadInp2').val();

        if (inputValue != "") {
            $("#upload-btn2").prop("disabled", false);
        }
        else {
            $("#upload-btn2").prop("disabled", true);
        }
    });
});

function checkFileExtension() {
    var fileName = document.getElementById('fileUploadInp').value.toLowerCase();
    var tagToDisplayMessage = document.getElementById('errormessgespan');
    if (fileName) {
        if (!(fileName.endsWith('.png'))) {
            tagToDisplayMessage.innerText = "Please Upload File With Valid Format";
            return false;
        }
        else {
            tagToDisplayMessage.innerText = "";
            return true;
        }
    }
    else {
        return true;
    }
}

function checkOnboardingFiles(inputid) {
    var fileName = document.getElementById('inputOnboardFile' + inputid).value.toLowerCase();
    var tagToDisplayMessage = document.getElementById('errorSpan' + inputid);
    if (fileName) {
        if (!(fileName.endsWith('.pdf'))) {
            tagToDisplayMessage.innerText = "Please Upload File with .pdf extension";
            return false;
        }
        else {
            tagToDisplayMessage.innerText = "";
            return true;
        }
    }
    else {
        return true;
    }
}