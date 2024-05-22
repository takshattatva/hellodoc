/************************************** Date validations **************************************/
$(document).ready(function () {
    var today = new Date();
    var day = today.getDate();
    var month = today.getMonth() + 1;
    var year = today.getFullYear();

    if (month < 10)
        month = '0' + month.toString();
    if (day < 10)
        day = '0' + day.toString();

    var maxDate = year + '-' + month + '-' + day;

    $('#birthDate').attr('max', maxDate);

    $('#birthDate').attr('value', "");
})