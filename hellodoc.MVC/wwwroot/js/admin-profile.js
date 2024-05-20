function disableFields3() {
    document.querySelectorAll('.d3class').forEach(function (element) {
        element.disabled = true;
    });
    $('.submitBtn3').addClass("d-none");
    $('.cancelBtn3').addClass("d-none");
    $('.editBtn3').removeClass("d-none");
}
function enableFields3() {
    document.querySelectorAll('.d3class').forEach(function (element) {
        element.disabled = false;
    });
    $('.submitBtn3').removeClass("d-none");
    $('.cancelBtn3').removeClass("d-none");
    $('.editBtn3').addClass("d-none");
}
disableFields3();

function disableFields() {
    document.querySelectorAll('.d1class').forEach(function (element) {
        element.disabled = true;
    });
    $('.submitBtn').addClass("d-none");
    $('.cancelBtn').addClass("d-none");
    $('.editBtn').removeClass("d-none");
}
function enableFields() {
    document.querySelectorAll('.d1class').forEach(function (element) {
        element.disabled = false;
    });
    $('.submitBtn').removeClass("d-none");
    $('.cancelBtn').removeClass("d-none");
    $('.editBtn').addClass("d-none");
}
disableFields();

function disableFields2() {
    document.querySelectorAll('.dclass').forEach(function (element) {
        element.disabled = true;
    });
    $('.submitBtn2').addClass("d-none");
    $('.cancelBtn2').addClass("d-none");
    $('.editBtn2').removeClass("d-none");
}
function enableFields2() {
    document.querySelectorAll('.dclass').forEach(function (element) {
        element.disabled = false;
    });
    $('.submitBtn2').removeClass("d-none");
    $('.cancelBtn2').removeClass("d-none");
    $('.editBtn2').addClass("d-none");
}
disableFields2();

var input = document.querySelector("#telephone3");
window.intlTelInput(input, {
    separateDialCode: true,
    preferredCountries: ["in", "us", "ca"],
});

var input = document.querySelector("#telephone4");
window.intlTelInput(input, {
    separateDialCode: true,
    preferredCountries: ["in", "us", "ca"],
});