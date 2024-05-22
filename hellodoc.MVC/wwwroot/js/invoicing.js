//************************************************* _Provider_Invoicing *************************************************
$(document).ready(function () {
    $('.ReimbursementTable').DataTable({
        "initComplete": function (settings, json) {

            $('#my-search-input2').val(settings.oPreviousSearch.sSearch);

            $('#my-search-input2').on('keyup', function () {
                var searchValue = $(this).val();
                settings.oPreviousSearch.sSearch = searchValue;
                settings.oApi._fnReDraw(settings);
            });
        },
        "pageLength": 5,
        pagingType: "full",

        language: {
            oPaginate: {
                sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
                sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

            }
        }
    });
    $('.dataTables_length').hide();
    $('.dataTables_filter').hide();
});


//************************************************* _Provider_FinalizeTimesheet *************************************************

function validateFinalizeForm(index) {
    var TotalHr = $(`#floatingTotalHours${index}`);
    var NoHc = $(`#floatingNumberOfHouseCalls${index}`);
    var NoPc = $(`#floatingNumberOfPhoneConsults${index}`);
    var approved = $('.finalizeform');

    if (TotalHr.val() < 0 || NoHc.val() < 0 || NoPc.val() < 0) {
        approved.prop('disabled', true);
    }
    else {
        approved.prop('disabled', false);
    }
}

function AddTimeSheetDetailId() {
    var timeSheetDetailId = [];

    $('.TimeSheetDetailIdClass').each(function () {
        var detailId = $(this).val();
        timeSheetDetailId.push(detailId);
    });

    GetAddReceipts(timeSheetDetailId);
}

function enableApproveBtn() {
    var bonus = $('.bonus');
    var note = $('.notes');
    var approved = $('.approve');

    if (bonus.val() == "" || note.val() == "" || bonus.val() < 0 || note.val().trim == "") {
        approved.prop('disabled', true);
    }
    else {
        approved.prop('disabled', false);
    }
}

function FinalizeTimeSheet() {
    var timeSheetId = $('#TimeSheetId').val();

    ConfirmFinalizeTimeSheet(timeSheetId);
}

function ApproveTimeSheet() {
    var timeSheetId = $('#TimeSheetId').val();

    ConfirmApproveTimeSheet(timeSheetId);
}


//************************************************* _Provider_AddReceipt *************************************************
function enableSubmit(counter) {
    $(`#inputItem${counter}`).prop("disabled", false);
    $(`#inputAmount${counter}`).prop("disabled", false);
    $(`#upload${counter}`).removeClass('d-none');
    $(`#filename${counter}`).addClass('d-none');
    $(`#EditContainer${counter}`).addClass('d-none');
    $(`#SubmitContainer${counter}`).removeClass('d-none');
}

function disableSubmit(counter) {
    $(`#inputItem${counter}`).prop("disabled", true);
    $(`#inputAmount${counter}`).prop("disabled", true);
    $(`#upload${counter}`).addClass('d-none');
    $(`#filename${counter}`).removeClass('d-none');
    $(`#EditContainer${counter}`).removeClass('d-none');
    $(`#SubmitContainer${counter}`).addClass('d-none');
    document.getElementById('ReceiptForm' + counter).reset();
}

function validatefile(counter) {
    const fileInput = document.getElementById('inputFile' + counter);

    fileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (!file.name.endsWith('.pdf')) {
            Swal.fire({
                title: "Oops!",
                text: "Only .pdf file allowed",
                icon: "error",
                timer: 3000,
                timerProgressBar: true,
            });
            fileInput.value = '';
        }
    });
}

function enableSubmitBtn(counter) {
    var item = $(`#inputItem${counter}`);
    var amount = $(`#inputAmount${counter}`);
    var submit = $(`#submitAddReceipt${counter}`);

    if (item.val() == "" || amount.val() == "" || item.val() < 0 || amount.val() < 0) {
        submit.prop('disabled', true);
    }
    else {
        submit.prop('disabled', false);
    }
}