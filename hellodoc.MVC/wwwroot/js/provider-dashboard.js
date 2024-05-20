
/******************************************** Get Dashboard data On Update Records *****************************************************/
function GetProviderDashboard() {
    event.preventDefault();

    showLoader();

    var physicianId = $('#sessionId').val();
    var lastStatus = $('#lastStatus').val();

    $.ajax({
        methd: "GET",
        url: "/Provider/GetProviderDashboard",
        data: { physicianId: physicianId, lastStatus: lastStatus },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#provider-home-tab-pane').html(result);

            if (lastStatus != 1) {
                $('#new-tab').removeClass("active");
            }
            if (lastStatus == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (lastStatus == 4) {
                $('#Active-tab').addClass("active");
            }
            if (lastStatus == 6) {
                $('#Conclude-tab').addClass("active");
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show initial table records (by default new status) *****************************************************/
function ProviderTableRecords(status, requesttypeid) {

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status);

    $.ajax({
        method: "POST",
        url: "/Provider/ProviderTableRecords",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#tableRecords').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show filtered table records(by region or request type) *****************************************************/
function ProviderFilterTable(status, requesttypeid) {

    if (requesttypeid == null) {
        requesttypeid = $('#reqTypeValueId').val();
    }
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status);

    $.ajax({
        method: "POST",
        url: "/Provider/ProviderFilterTable",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#providerRequestTable').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}
/******************************************** Fetch Accept Case *****************************************************/
function AcceptCaseModal(requestid) {

    $.ajax({
        methd: "GET",
        url: "/Provider/AcceptCaseModal",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#acceptModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Accept Case *****************************************************/
function AcceptCase() {
    event.preventDefault();

    var requestId = $('#clearRequestId').val();
    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "POST",
        url: "/Provider/AcceptCase",
        data: { requestId: requestId, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#acceptModalId').modal('hide');
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Case Accepted!",
                showConfirmButton: false,
                timer: 1500
            });
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Transfer Case *****************************************************/
function Transfer(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/Provider/Transfer",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#providerTransferModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}
/******************************************** Update Transfer Case *****************************************************/
function TransferCase() {
    event.preventDefault();

    if ($('#providerTransferCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/TransferCase",
            data: $('#providerTransferCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#providerTransferModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Case Transfer To Admin!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch House call Modal to ask for conclude the request *****************************************************/
function HouseCall(requestId) {

    $.ajax({
        method: "GET",
        url: "/Provider/HouseCall",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#houseCallModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update House call Modal to conclude the request *****************************************************/
function HouseCallPost(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/HouseCallPost",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#houseCallModalId').modal('hide');
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "House Call Care Concluded",
                showConfirmButton: false,
                timer: 1500
            });
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Encounter Modal to select house call or consultant ? *****************************************************/
function EncounterModal(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/Provider/EncounterModal",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterModalBox').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Encounter Modal to selected option house call or consultant *****************************************************/
function PostEncounterCare() {
    event.preventDefault();

    if ($('#EncounterModalForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/PostEncounterCare",
            data: $('#EncounterModalForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#EncounterModalBox').modal('hide');
                GetProviderDashboard();
                if (result) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Care type will be House call",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Care type will be Consultant",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Finalize Modal to ask for finalize encounter form *****************************************************/
function finalizeEncounterModal(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/finalizeEncounterModal",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterFinalizeModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Finalize Encounter Form *****************************************************/
function finalizeEncounter(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/finalizeEncounter",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#EncounterFinalizeModal').modal('hide');
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Case Finalized",
                showConfirmButton: false,
                timer: 1500
            });
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Download Encounter Form Modal *****************************************************/
function DownloadEncounter(requestId, callId) {

    $.ajax({
        method: "GET",
        url: "/Provider/DownloadEncounter",
        data: { requestId: requestId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 1) {
                $('#showCaseModal').html(result);
                $('#DownloadFinalizeModal').modal('show');
            }
            if (callId == 3) {
                $('#showProviderCaseModal').html(result);
                $('#DownloadFinalizeModal').modal('show');
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Download Encounter Form Modal *****************************************************/
function FinalizeDownload(requestid) {
    event.preventDefault();

    window.location.href = './GeneratePDF?requestid=' + requestid;
}
/******************************************** Fetch Conclude Case *****************************************************/
function GetConcludeCare(requestid) {
    showLoader();

    $.ajax({
        methd: "GET",
        url: "/Provider/GetConcludeCare",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-home-tab-pane').html(result);

            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update (upload file) Close Case *****************************************************/
function UploadConcludeDocument() {
    event.preventDefault();

    var formdata = new FormData($('#concludeDocumentFormId')[0]);

    $.ajax({
        method: "POST",
        url: "/Provider/UploadConcludeDocument",
        data: formdata,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Document Uploaded",
                showConfirmButton: false,
                timer: 1500
            });
            GetConcludeCare(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update (delete file) Conclude Case *****************************************************/
function DeleteConcludeFile(requestwisefileid, requestid) {
    event.preventDefault();

    $.ajax({
        methd: "POST",
        url: "/Provider/DeleteConcludeFile",
        data: { requestwisefileid: requestwisefileid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Document Deleted",
                showConfirmButton: false,
                timer: 1500
            });
            GetConcludeCare(requestid);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}
/******************************************** Update (conclude the case) Conclude Case *****************************************************/
function ConcludeCare() {
    event.preventDefault();

    if ($('#ConcludeFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/ConcludeCare",
            data: $('#ConcludeFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                if (result == 1) {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "First you need to finalize the Encounter form!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    GetProviderDashboard();
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Case Concluded",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    GetProviderDashboard();
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}
/******************************************** Update Provider Request to Admin for changes in him/her Profile *****************************************************/
function SendRequestToAdmin() {
    event.preventDefault();

    if ($('#requestToAdminFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/SendRequestToAdmin",
            data: $('#requestToAdminFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#requestToAdminModal').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Requested to Admin",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Provider Invoicing *****************************************************/
function GetProviderInvoicing() {

    showLoader();
    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        methd: "GET",
        url: "/Provider/GetProviderInvoicing",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);
            if (dateSelected != undefined) {
                $('#searchByTimeperiod').val(dateSelected);
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Finalize TimeSheet *****************************************************/
function OpenFinalizeTimeSheet() {

    showLoader();
    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        methd: "GET",
        url: "/Provider/OpenFinalizeTimeSheet",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);
            setTimeout(function () {
                hideLoader();
            }, 300);

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Post Finalize Timesheet *****************************************************/
function PostFinalizeTimesheet(callId) {
    event.preventDefault();

    var formData = $('#finalizeSheetForm').serializeArray();

    $.ajax({
        method: "POST",
        url: "/Provider/PostFinalizeTimesheet",
        data: formData,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (result) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Timesheet Updated",
                    showConfirmButton: false,
                    timer: 1500
                });
                if (callId == 1) {
                    GetAdminInvoicing();
                }
                if (callId == 3) {
                    GetProviderInvoicing();
                }
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Add Receipt *****************************************************/
function GetAddReceipts(timeSheetDetailId) {

    $.ajax({
        methd: "GET",
        url: "/Provider/GetAddReceipts",
        data: { timeSheetDetailId: timeSheetDetailId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#AddReceiptsContainer').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Add Receipt *****************************************************/
function PostAddReceipt(counter) {
    event.preventDefault();

    var formData = new FormData();

    var timeSheetDetailId = $(`#inputTimeSheetDetailId${counter}`).val();
    var item = $(`#inputItem${counter}`).val();
    var amount = $(`#inputAmount${counter}`).val();
    var file = $(`#inputFile${counter}`)[0].files[0];

    formData.append("timeSheetDetailId", timeSheetDetailId);
    formData.append("item", item);
    formData.append("amount", amount);
    formData.append("file", file);

    $.ajax({
        method: "POST",
        url: "/Provider/PostAddReceipt",
        data: formData,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurehh!",
                text: "Receipt Updated!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** _ *****************************************************/
function DeleteReceipt(timeSheetDetailId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/DeleteReceipt",
        data: { timeSheetDetailId: timeSheetDetailId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurehh!",
                text: "Receipt Deleted!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** _ *****************************************************/
function ConfirmFinalizeTimeSheet(timeSheetId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/ConfirmFinalizeTimeSheet",
        data: { timeSheetId: timeSheetId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurehh!",
                text: "Timesheet Finalized!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            })
            GetProviderInvoicing();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** _ *****************************************************/
/******************************************** _ *****************************************************/
/******************************************** _ *****************************************************/