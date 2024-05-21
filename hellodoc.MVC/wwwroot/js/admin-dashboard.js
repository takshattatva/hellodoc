function showLoader() {
    var loaderContainer = document.querySelector(".loader-container");
    var backdrop = document.querySelector(".backdrop");
    if (loaderContainer && backdrop) {
        loaderContainer.style.display = "flex";
        backdrop.style.display = "flex";
    }
}

function hideLoader() {
    var loaderContainer = document.querySelector(".loader-container");
    var backdrop = document.querySelector(".backdrop");
    if (loaderContainer && backdrop) {
        loaderContainer.style.display = "none";
        backdrop.style.display = "none";
    }
}

hideLoader();
/******************************************** Get Dashboard data On Update Records *****************************************************/
function GetDashboard(status) {
    event.preventDefault();

    showLoader();

    if (status == 0) {
        stauts = $('#statusForName').val();
    }

    $.ajax({
        methd: "GET",
        url: "/Admin/GetDashboard",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#home-tab-pane').html(result);

            if (status != 1) {
                $('#new-tab').removeClass("active");
            }
            if (status == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (status == 4) {
                $('#Active-tab').addClass("active");
            }
            if (status == 6) {
                $('#Conclude-tab').addClass("active");
            }
            if (status == 3) {
                $('#ToClose-tab').addClass("active");
            }
            if (status == 9) {
                $('#Unpaid-tab').addClass("active");
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
function TableRecords(status, requesttypeid, regionid) {

    $.ajax({
        method: "POST",
        url: "/Admin/TableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#tableRecords').html(result);
        },

        error: function () {
            alert('Error loading table records');
        }
    });
}
/******************************************* To show filtered table records (by region or request type) **************************************************/
function FilterTableRecords(status, requesttypeid, regionid) {

    $.ajax({
        method: "POST",
        url: "/Admin/FilterTableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#requestTable').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show view case records *******************************************************************************/
function ViewCaseRecords(status, requestid, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ViewCaseRecords",
        data: { status: status, requestid: requestid, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            else {
                if (callId == 1) {
                    $('#home-tab-pane').html(result);
                }
                if (callId == 2) {
                    $('#Patient-tab-pane').html(result);
                }
                if (callId == 3) {
                    $('#provider-home-tab-pane').html(result);
                }
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update view case records *****************************************************/
function UpdateViewCaseRecords() {
    event.preventDefault();

    var callId = $('#callId').val();

    if ($('#viewCaseDataForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateViewCaseRecords",
            data: $('#viewCaseDataForm').serialize(),

            success: function (result) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Case Data Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                ViewCaseRecords(result.status, result.requestid, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }

    // Show the edit button, hide the submit button
    document.getElementById('submitBtn').style.display = 'none';
    document.getElementById('editBtn').style.display = 'block';

    // After editing field, To desable field
    document.querySelectorAll('.input-disable ').forEach(function (element) {
        element.disabled = true;
    });
}
/******************************************** To show view notes records *****************************************************/
function ViewNotes(requestid, status, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ViewNotes",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 3) {
                $('#provider-home-tab-pane').html(result);
                $('#adminNote').val("");
            }
            else if (callId == 4) {
                $('#provider-home-tab-pane').html(result);
                $('#adminNote').val("");
            }
            else {
                $('#home-tab-pane').html(result);
                $('#adminNote').val("");
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
/******************************************** To update view notes records *****************************************************/
function UpdateViewNotes(requestId, callId) {
    event.preventDefault();

    if ($('#viewNotesForm').valid()) {
        var formData = new FormData($('#viewNotesForm')[0]);
        formData.append('callId', callId);

        $.ajax({
            method: "POST",
            url: "/Admin/UpdateViewNotes",
            data: formData,
            contentType: false,
            processData: false,

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                ViewNotes(result.requestid, result.status, result.callId);
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Notes Submitted!",
                    showConfirmButton: false,
                    timer: 1500
                });
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show cancel case records *****************************************************/
function CancelModal(requestid, status, callId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/CancelModal",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#cancelModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update cancel case records *****************************************************/
function CancelCase() {
    event.preventDefault();

    var status = $('#statusForName').val();

    if ($('#cancelCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CancelCase",
            data: $('#cancelCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#cancelModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Request Canceled!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetDashboard(status);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show assign case records *****************************************************/
function AssignModal(requestid, status, callId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/AssignModal",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#assignModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show filtered physician name as per region ( for assign case )*****************************************************/
function FilterAssignModal(regionid, RequestId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/FilterAssignModal",
        data: { regionid: regionid, RequestId: RequestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $("#physicianId").append($("<option></option>").attr("value", "").text("Select Physician").prop("disabled", true).prop("selected", true));
            $("#physicianId").empty();
            result.success.forEach(obj => {
                $('#physicianId').append($("<option></option>").attr("value", obj.physicianid).text("Dr. " + obj.firstname + " " + obj.lastname))
            });
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update Assign case records *****************************************************/
function AssignCase() {
    event.preventDefault();

    var status = $('#statusForName').val();

    if ($('#assignCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AssignCase",
            data: $('#assignCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#assignModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Case Assigned!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetDashboard(status);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show block case records *****************************************************/
function BlockModal(requestid, status) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/BlockModal",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#blockModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update block case records *****************************************************/
function BlockCase() {
    event.preventDefault();

    var status = $('#statusForName').val();

    if ($('#blockCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/BlockCase",
            data: $('#blockCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#blockModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Request Blocked!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetDashboard(status);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show View Documents records *****************************************************/
function ViewDocuments(requestid, status, callId) {

    showLoader();

    $.ajax({
        methd: "GET",
        url: "/Admin/ViewDocuments",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            else {
                if (callId == 1) {
                    $('#home-tab-pane').html(result);
                }
                if (callId == 2) {
                    $('#Patient-tab-pane').html(result);
                }
                if (callId == 3) {
                    $('#provider-home-tab-pane').html(result);
                }
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

/******************************************** To update View Documents records *****************************************************/
function UploadDocument(requestid, status, callId) {

    var formdata = new FormData($('#viewDocumentFormId')[0]);
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/UploadDocument",
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
                title: "Document Uploaded!",
                showConfirmButton: false,
                timer: 1500
            });
            ViewDocuments(result, status, callId);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
function DeleteFile(requestwisefileid, requestid, status, callId) {
    event.preventDefault();

    var status = $('#statusForName').val();
    var callId = $('#callId').val();

    $.ajax({
        methd: "GET",
        url: "/Admin/DeleteFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Files Deleted!",
                showConfirmButton: false,
                timer: 1500
            });
            ViewDocuments(result.requestid, result.status, result.callId);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}
function SendFile(requestwisefileid, requestid, status, callId) {
    event.preventDefault();

    $.ajax({
        methd: "GET",
        url: "/Admin/SendFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status, callId: callId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Mail Sent!",
                showConfirmButton: false,
                timer: 1500
            });
            ViewDocuments(result.requestid, result.status, result.callId);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}
/******************************************** To show Send Order records *****************************************************/
function SendOrder(requestid, status, callId) {
    event.preventDefault();

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/SendOrder",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 1) {
                $('#home-tab-pane').html(result);
            }
            if (callId == 3) {
                $('#provider-home-tab-pane').html(result);
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
/******************************************** To filter Send Order records based on healthprofessionalid *****************************************************/
function FilterSendOrder(health_professional_id) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/FilterSendOrder",
        data: { health_professional_id: health_professional_id },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $("#healthprofessionid").empty();
            let defaultOption = $("<option></option>").attr("value", "").text("Select Health Professional").prop("disabled", true).prop("selected", true);
            $('#healthprofessionid').append(defaultOption);
            result.success.forEach(obj => {
                $('#healthprofessionid').append($("<option></option>").attr("value", obj.vendorid).text(obj.vendorname))
            });
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show Vender data records *****************************************************/
function VendorData(vendorid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/VendorData",
        data: { vendorid: vendorid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $("#floatingEmail").val(result.success.email);
            $("#floatingcontect").val(result.success.businessContact);
            $("#floatingFaxNum").val(result.success.faxNum);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update Send Order records *****************************************************/
function UpdateSendOrder() {
    event.preventDefault();

    var status = $('#statusForNameId').val();
    var callId = $('#callId').val();

    if ($('#sendOrderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendOrder",
            data: $('#sendOrderForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Order Sent!",
                    showConfirmButton: false,
                    timer: 1500
                });
                if (callId == 1) {
                    SendOrder(result.requestid, result.status, result.callId);
                }
                if (callId == 3) {
                    SendOrder(result.requestid, result.status, result.callId);
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show Transfer case records *****************************************************/
function TransferModal(requestid, status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/TransferModal",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#transferModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show filtered physician name as per region ( for Transfer case )*****************************************************/
function FilterTransferModal(regionid, RequestId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/FilterTransferModal",
        data: { regionid: regionid, RequestId: RequestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $("#physicianId").append($("<option></option>").attr("value", "").text("Select Physician").prop("disabled", true).prop("selected", true));
            $("#physicianId").empty();
            result.success.forEach(obj => {
                $('#physicianId').append($("<option></option>").attr("value", obj.physicianid).text("Dr. " + obj.firstname + " " + obj.lastname))
            });
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update Transfer case records *****************************************************/
function TransferCasetophy() {
    event.preventDefault();

    var status = $('#statusForName').val();

    if ($('#transferCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/TransferCase",
            data: $('#transferCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#transferModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Request Transfered!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetDashboard(status);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show Clear case records *****************************************************/
function ClearModel(requestid, status) {

    $.ajax({
        method: "GET",
        url: "/Admin/ClearModal",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#clearModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To update Clear case records *****************************************************/
function ClearCase() {
    event.preventDefault();

    var status = $('#statusForNameId').val();

    $.ajax({
        method: "POST",
        url: "/Admin/ClearCase",
        data: $('#clearCaseForm').serialize(),

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#clearModalId').modal('hide');
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Request Cleared!",
                showConfirmButton: false,
                timer: 1500
            });
            GetDashboard(status);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To show Send Agreement records *****************************************************/
function SendAgreementModal(requestid, requesttypeid, status, callId) {
    $.ajax({
        methd: "GET",
        url: "/Admin/SendAgreementModal",
        data: { requestid: requestid, requesttypeid: requesttypeid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 1) {
                $('#showCaseModal').html(result);
            }
            if (callId == 3) {
                $('#showProviderCaseModal').html(result);
            }
            $('#SendAgreementModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To Send Mail for Submit Send Agreement *****************************************************/
function SendAgreement(callId) {
    event.preventDefault();

    var status = $('#statusForNameId').val();

    if ($('#sendAgreementFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendAgreement",
            data: $('#sendAgreementFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#SendAgreementModalId').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Agreement Sent!",
                    showConfirmButton: false,
                    timer: 1500
                });
                if (callId == 3) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard(status);
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** To show Send Link Modal *****************************************************/
function sendLinkModal(status, callId) {
    $.ajax({
        method: "GET",
        url: "/Admin/sendLinkModal",
        data: { status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 1) {
                $('#showCaseModal').html(result);
                $('#sendLinkModal').modal('show');
            }
            if (callId == 2) {
                $('#showProviderCaseModal').html(result);
                $('#sendLinkModal').modal('show');
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** To submit Send Link *****************************************************/
function SendLink(status, callId) {
    event.preventDefault();

    var formdata = $('#sendLinkFormId').serialize();

    if ($('#sendLinkFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendLink",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#sendLinkModal').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Link Sent!",
                    showConfirmButton: false,
                    timer: 1500
                });
                if (callId == 1) {
                    GetDashboard(status);
                }
                if (callId == 2) {
                    GetProviderDashboard();
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Create Request *****************************************************/
function adminCreateRequest(status, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/adminCreateRequest",
        data: { status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (callId == 1) {
                $('#home-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
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
/******************************************** Set/Submit Req data in Table(r,rc) *****************************************************/
function sendAdminCreateRequest(status, callId) {
    event.preventDefault();

    if ($('#AdminCreateRequestFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/sendAdminCreateRequest",
            data: $('#AdminCreateRequestFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Request Created!",
                    showConfirmButton: false,
                    timer: 1500
                });
                if (callId == 1) {
                    GetDashboard(status);
                }
                if (callId == 2) {
                    GetProviderDashboard();
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Close case *****************************************************/
function CloseCase(requestid, status) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/CloseCase",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#home-tab-pane').html(result);
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
/******************************************** Update Close case Data *****************************************************/
function updateCloseCase() {
    event.preventDefault();

    if ($('#closeCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/updateCloseCase",
            data: $('#closeCaseForm').serialize(),

            success: function (result) {
                console.log("close case updated");
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Case Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                CloseCase(result.reqId, result.status);
            },

            error: function () {
                alert("error in close case update")
            }
        });
    }
}
/******************************************** Post Close case Data *****************************************************/
function PostCloseCase(requestId, status) {
    $.ajax({
        method: "POST",
        url: "/Admin/PostCloseCase",
        data: { requestId: requestId, status: status },

        success: function () {
            console.log("success in post close case");
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Case Closed!",
                showConfirmButton: false,
                timer: 1500
            });
            GetDashboard(status);
        },

        error: function () {
            alert("error in post close case");
        }
    });
}
/******************************************** Export *****************************************************/
function Export(arr, requesttypeid) {

    showLoader();

    var regionid = $('#regionId').val();

    requesttypeid = $('#reqTypeValueId').val();
    if (requesttypeid == 0) {
        requesttypeid = null;
    }
    var arr2 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

    if (JSON.stringify(arr) == JSON.stringify(arr2)) {
        regionid = 0;
        requesttypeid = null;
    }

    $.ajax({
        method: "POST",
        url: "/Admin/Export",
        data: { arr: arr, requesttypeid: requesttypeid, regionid: regionid },
        xhrFields: {
            resultType: 'blob'
        },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'RequestData.xlsx';

            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(link.href);

            hideLoader();
            Swal.fire("Hurrah", "File Downloaded", "success");
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Request DTY Support *****************************************************/
function RequestSupport() {
    $.ajax({
        method: "GET",
        url: "/Admin/RequestSupport",

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#requestSupportModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Request DTY Support *****************************************************/
function requestDTYPost() {
    event.preventDefault();

    if ($('#requestDTYFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/RequestDTYPost",
            data: $('#requestDTYFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Requested Support Send!",
                    showConfirmButton: false,
                    timer: 1500
                });
                $('#requestSupportModalId').modal('hide');
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Admin Edit Profile *****************************************************/
function GetAdminProfile(aspnetId, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetAdminProfile",
        data: { aspnetId: aspnetId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            else {
                setTimeout(function () {
                    hideLoader();
                }, 300);
                if (callId == 1) {
                    $('#profile-tab-pane').html(result);
                }
                if (callId == 2) {
                    $('#User-tab-pane').html(result);
                }
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Updating Admin Edit Profile *****************************************************/
function AdminResetPassword() {
    event.preventDefault();
    var formdata = $('#ResetPasswordForm').serialize();

    if ($('#ResetPasswordForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdminResetPassword",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Password Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                $('#floatingPassword').val("");
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
function AdminAccountEdit() {
    event.preventDefault();

    if ($('#ResetPasswordForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdminAccountEdit",
            data: $('#ResetPasswordForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Account Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                disableFields3();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}
function AdministratorDetail() {
    event.preventDefault();
    var formdata = $('#AdministratorForm').serialize();

    if ($('#AdministratorForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdministratorDetail",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Administrator Details Updated!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    disableFields();
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Confirm Email Should Be Same!",
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
function MailingDetail() {
    event.preventDefault();

    var formdata = $('#MailingForm').serialize();

    if ($('#MailingForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/MailingDetail",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Mailing And Billing Information Details Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                disableFields2();
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function DeleteAdminAccount(adminId) {
    $.ajax({
        method: "POST",
        url: "/Admin/DeleteAdminAccount",
        data: { adminId: adminId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurehh!",
                text: "Admin Deleted!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            });
            GetUserAccess(0);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Admin Encounter *****************************************************/
function AdminEncounter(requestid, status, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/AdminEncounter",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            if (callId == 3) {
                $('#provider-home-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#Patient-tab-pane').html(result);
            }
            if (callId == 1) {
                $('#home-tab-pane').html(result);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Submit Admin Encounter *****************************************************/
function SubmitEncounter() {
    event.preventDefault();

    var formdata = $('#EncounterForm').serialize();

    if ($('#EncounterForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SubmitEncounter",
            data: formdata,
            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Encounter Form Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                AdminEncounter(result.reqId, result.status, result.callId);
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Providers Table *****************************************************/
function GetProvider(regionId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetProvider",
        data: { regionId: regionId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            $('#Provider-tab-pane').html(result);
            $('#regionValue').val(regionId);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Set Stop Notification for Providers *****************************************************/
function stopNotification(physicianId) {
    $.ajax({
        method: "POST",
        url: "/Admin/stopNotification",
        data: { physicianId: physicianId },

        success: function () {
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Notification Status Changed!!!",
                showConfirmButton: false,
                timer: 1500
            });
            GetProvider(0);
        },

        error: function () {
            Swal.fire("Yehhh!", "Notification Stopped!", "success");
        }
    });
}
/******************************************** Fetch Providers Contact Modal *****************************************************/
function ContactProvider(email) {
    $.ajax({
        method: "GET",
        url: "/Admin/ContactProvider",
        data: { email: email },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#ContactProviderModal').modal('show');
            $('#contactEmailId').val(email);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Submit Providers Contact Modal Via Email *****************************************************/
function SendEmailToProvider() {
    event.preventDefault();

    if ($('#ContactProviderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendEmailToProvider",
            data: $('#ContactProviderForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                $('#ContactProviderModal').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Mail Sent!",
                    showConfirmButton: false,
                    timer: 1500
                });
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Submit Providers Contact Modal Via Contact *****************************************************/
function ContactToProvider() {
    event.preventDefault();

    if ($('#ContactProviderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendContactToProvider",
            data: $('#ContactProviderForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                $('#ContactProviderModal').modal('hide');
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Message Sent!",
                    showConfirmButton: false,
                    timer: 1500
                });
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Fetch Edit Provider Page *****************************************************/
function GetEditProvider(aspId, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEditProvider",
        data: { aspId: aspId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#User-tab-pane').html(result);
            }
            if (callId == 3) {
                $('#provider-profile-tab-pane').html(result);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Submit Providers Edit Page *****************************************************/
function PhysicianProfileResetPassword(callId) {
    event.preventDefault();

    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianProfileResetPassword",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Password Updated!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    GetEditProvider(result.aspId, callId);
                }
                else {
                    Swal.fire({
                        title: "Opps!",
                        text: "Please Enter Password!",
                        icon: "error",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}
function PhysicianAccountEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianAccountEdit",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Account Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}
function PhysicianAdministratorEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAdministratorForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianAdministratorEdit",
            data: $('#PhysicianAdministratorForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Administrator Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}
function PhysicianMailingEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianMailingForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianMailingEdit",
            data: $('#PhysicianMailingForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Mailing Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
function PhysicianBusinessInfoEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianBusinessInfoForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianBusinessInfoEdit",
            data: new FormData($('#PhysicianBusinessInfoForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Business Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
function UpdateOnBoarding(callId) {
    event.preventDefault();

    if ($('#OnboardingEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateOnBoarding",
            data: new FormData($('#OnboardingEditForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Onboarding Information Updated!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
function DeletePhysicianAccount(physicianId, callId) {
    $.ajax({
        method: "POST",
        url: "/Admin/DeletePhysicianAccount",
        data: { physicianId: physicianId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (result.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Physician Deleted!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else {
                Swal.fire({
                    title: "Oopps, You Can't Delete this Account!!",
                    text: "Physician Currently Providing Service!",
                    icon: "error",
                    timer: 3000, // The alert will close after 3000ms (3 seconds)
                    timerProgressBar: true, // Show a progress bar to indicate the time remaining
                });
            }
            if (callId == 1) {
                GetProvider(0);
            }
            else {
                GetUserAccess(0);
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Create Providers *****************************************************/
function CreateProviderAccount(callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/CreateProviderAccount",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            else {
                $('#User-tab-pane').html(result);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Submit Create Provider *****************************************************/
function CreateProviderAccountPost(callId) {
    event.preventDefault();

    if ($('#CreatePhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateProviderAccountPost",
            data: new FormData($('#CreatePhysicianAccountForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        title: "Hurrehhh!",
                        text: "Physician Created!",
                        icon: "success",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                else {
                    Swal.fire({
                        title: "Oopps!!",
                        text: "Physician Already Exists!",
                        icon: "error",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                if (callId == 1) {
                    GetProvider(0);
                }
                else {
                    GetUserAccess(0);
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Get Account Access *****************************************************/
function GetAccountAccess() {
    event.preventDefault();

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetAccountAccess",
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Get Create Account Access *****************************************************/
function GetCreateAccess() {
    event.preventDefault();

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetCreateAccess",
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Filter Checkboxes for Create Access Account *****************************************************/
function FilterRolesMenu(accounttype) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/FilterRolesMenu",
        data: { accounttype: accounttype },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#RolesMenuList').empty();
                result.forEach(obj => {
                    $('#RolesMenuList').append(`<div class='form-check form-check-inline px-2 mx-3 my-2 col-auto'><input class='form-check-input d2class' name='AccountMenu' type='checkbox' id='${obj.menuid}' value='${obj.menuid}'/><label class='form-check-label' for='${obj.menuid}'>${obj.name}</label></div>`)
                });
            }
        },
        error: function () {
            Swal.fire("Oops", "Something is Wrong", "error");
        }
    });
}
/******************************************** Set Create Account Access *****************************************************/
function SetCreateAccessAccount() {
    event.preventDefault();

    if ($('#AccountCreateForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SetCreateAccessAccount",
            data: $('#AccountCreateForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result == true) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Role Created Successfully!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    GetAccountAccess();
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Role already exists!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}
/******************************************** Get Edit Account Access *****************************************************/
function GetEditAccess(accounttypeid, roleid) {
    event.preventDefault();

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEditAccess",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Filter Checkboxes for Edit Access Account *****************************************************/
function FilterEditRolesMenu(accounttypeid, roleid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/FilterEditRolesMenu",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#EditRoleMenu').html(result);
            }
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Set Edit Account Access *****************************************************/
function SetEditAccessAccount(accounttypeid, roleid) {
    event.preventDefault();

    if ($('#AccountEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SetEditAccessAccount",
            data: $('#AccountEditForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Role Edited Successfully!",
                    showConfirmButton: false,
                    timer: 1500
                });
                GetAccountAccess();
            },

            error: function () {
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }
        });
    }
}
/******************************************** Delete All Access List For physician or admin *****************************************************/
function DeleteAccountAccess(roleid) {

    $.ajax({
        method: "POST",
        url: "/Admin/DeleteAccountAccess",
        data: { roleid: roleid },

        success: function (result) {
            Swal.fire({
                title: "Hurrehhh!",
                text: "Role Deleted Successfully!",
                icon: "success",
                timerProgressBar: true,
                timer: 1500,
            });
            GetAccountAccess();
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Get User Access *****************************************************/
function GetUserAccess(accountTypeId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetUserAccess",
        data: { accountTypeId: accountTypeId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#User-tab-pane').html(result);
                $('#roleValue').val(accountTypeId);
                if (accountTypeId == 1) {
                    $('#createAdmin').removeClass('d-none');
                }
                if (accountTypeId == 2) {
                    $('#createProvider').removeClass('d-none');
                }
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Get Create Admin *****************************************************/
function GetCreateAdminAccount(callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetCreateAdminAccount",
        data: { callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Admin-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#User-tab-pane').html(result);
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
/******************************************** Set Create Admin *****************************************************/
function CreateAdminAccountPost(callId) {
    event.preventDefault();

    if ($('#CreateAdminAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateAdminAccountPost",
            data: $('#CreateAdminAccountForm').serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hurreehh!",
                        text: "Admin Created!",
                        icon: "success",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                else {
                    Swal.fire({
                        title: "Oopps!",
                        text: "Admin Already Exists!",
                        icon: "error",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                GetUserAccess(0);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Scheduling *****************************************************/
function GetScheduling(callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetScheduling",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                setTimeout(function () {
                    hideLoader();
                }, 300);
                if (callId == 1) {
                    $('#Scheduling-tab-pane').html(result);
                }
                if (callId == 3) {
                    $('#provider-schedule-tab-pane').html(result);
                }
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Fetch Add Shift Modal *****************************************************/
function CreateNewShift(callId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/CreateNewShift",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                $('#schedulingModals').html(result);
                $('#createShiftModal').modal('show');
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Set Add Shift *****************************************************/
function createShiftPost(callId) {
    event.preventDefault();

    if ($('#createShiftForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/createShiftPost",
            data: $('#createShiftForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    setTimeout(function () { location.reload(); }, 2000);
                    Swal.fire("Oops!", "Session Expired !!!", "error");
                } else {
                    if (result) {
                        $('#createShiftModal').modal('hide');
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Shift Created Succesfully!",
                            showConfirmButton: false,
                            timer: 1500
                        });
                        if (callId == 1) {
                            GetScheduling(1);
                        }
                        if (callId == 3) {
                            GetScheduling(3);
                        }
                    }
                    else {
                        Swal.fire({
                            title: "Oopps!",
                            text: "This Time Slot is not Available",
                            icon: "error",
                            timer: 3000,
                            timerProgressBar: true,
                        });
                    }
                }
            },

            error: function () {
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }
        });
    }
}
/******************************************** Fetch Shift Review Page *****************************************************/
function ShiftReview(regionId, callId) {
    event.preventDefault();

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ShiftReview",
        data: { regionId: regionId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#regionValue').val(regionId);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Set Select Shift Aprroval *****************************************************/
function ApproveShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/ApproveShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire({
                        title: "Oopps!",
                        text: "Please Select Any Shift To Proceed Ahead",
                        icon: "error",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                else {
                    Swal.fire({
                        title: "Oopps!",
                        text: "Selected Shifts Approved",
                        icon: "success",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Set Selected Shift Deletion *****************************************************/
function DeleteSelectedShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/DeleteSelectedShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire({
                        title: "Oopps!",
                        text: "Please Select Any Shift To Proceed Ahead",
                        icon: "error",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                }
                else {
                    Swal.fire({
                        title: "Oopps!",
                        text: "Selected Shifts Deleted",
                        icon: "success",
                        timer: 3000,
                        timerProgressBar: true,
                    });
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Fetch MDOnCall *****************************************************/
function MDOnCall(regionId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/MDOnCall",
        data: { regionId: regionId },

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#regionValue').val(regionId);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Provider Location *****************************************************/
function ProviderLocation() {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ProviderLocation",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#provider-location-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}
/******************************************** Partners Tab *****************************************************/
function Partners(professionid) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/Partners",
        data: { professionid: professionid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $("#Partners-tab-pane").html(result);
            if (professionid != 0) {
                $(".ProfessionsDropdown").val(professionid);
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        },
    });
}
/******************************************** Get Add Business / Edit Business *****************************************************/
function AddBusiness(vendorID) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/AddBusiness",
        data: { vendorID: vendorID },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $("#Partners-tab-pane").html(result);
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        },
    });
}
/******************************************** Create New Business *****************************************************/
function CreateNewBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateNewBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "New Business Added!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    Partners(0);
                } else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Business Already Exists!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            },
        });
    }
}
/******************************************** Set Edit Business *****************************************************/
function UpdateBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Business Data Updated!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                    AddBusiness(result.vendorid);
                } else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Do some Changes First!!!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            },
        });
    }
}
/******************************************** Delete Business *****************************************************/
function DelPartner(VendorID) {
    $.ajax({
        method: "POST",
        url: "/Admin/DelPartner",
        data: { VendorID: VendorID },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "HelloDoc",
                text: "Business Partner Deleted Successfully",
                icon: "success",
                timer: 1500,
                timerProgressBar: true,
            });
            Partners(0);
        },
        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        },
    });
}
/******************************************** Fetch Patient History *****************************************************/
function GetRecordsTab() {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetRecordsTab",
        data: $('#patientRecordsform').serialize(),

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(result);
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
/******************************************** Search Filter in Patient History *****************************************************/
function patientRecordsFilter() {
    event.preventDefault();
    var formdata = $('#patientRecordsform').serialize();

    $.ajax({
        method: "POST",
        url: "/Admin/GetRecordsTab",
        data: formdata,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(result);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Explore in Patient History *****************************************************/
function GetPatientRecordExplore(UserId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetPatientRecordExplore",
        data: { UserId: UserId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(result);
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Block History *****************************************************/
function GetBlockedRequest() {

    showLoader();
    var formdata = $('#BlockedRecordsform').serialize();

    $.ajax({
        method: "GET",
        url: "/Admin/GetBlockedRequest",
        data: formdata,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(result);
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
/******************************************** Filter in Block History *****************************************************/
function BlockedRecordsFilter() {
    event.preventDefault();
    var formdata = $('#BlockedRecordsform').serialize();

    $.ajax({
        method: "POST",
        url: "/Admin/GetBlockedRequest",
        data: formdata,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(res);
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
/******************************************** unblock Block History *****************************************************/
function UnblockRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/Admin/UnblockRequest",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(result);
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Request Unblocked",
                showConfirmButton: false,
                timer: 1000
            });
            GetBlockedRequest();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch EmailLog *****************************************************/
function GetEmailSmsLog(tempId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEmailSmsLog",
        data: { tempId: tempId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (tempId == 0) {
                $('#Email-tab-pane').html(result);
            }
            else {
                $('#SMS-tab-pane').html(result);
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
/******************************************** Filter Email/SMS lOG *****************************************************/
function emailSmsLogFilter() {
    event.preventDefault();

    var formdata = $('#logsRecordFilter').serialize();
    var tempid = $('#tempidsmsemaillog').val();

    $.ajax({
        method: "POST",
        url: "/Admin/emailSmsLogFilter",
        data: formdata,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (tempid == 0) {
                $('#Email-tab-pane').html(result);
            }
            else {
                $('#SMS-tab-pane').html(result);
            }
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Search Records *****************************************************/
function GetSearchRecords() {

    showLoader();

    var formdata = $('#searchRecordsForm').serialize();

    $.ajax({
        method: "GET",
        url: "/Admin/GetSearchRecords",
        data: formdata,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(result);
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
/******************************************** Export in Search Records *****************************************************/
function ExportSearchRecords() {

    showLoader();

    $.ajax({
        method: "POST",
        url: "/Admin/ExportSearchRecords",
        data: $('#searchRecordsForm').serialize(),
        xhrFields: {
            responseType: 'blob'
        },

        success: function (data) {

            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'Requests.xlsx';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            hideLoader();
            Swal.fire({
                title: "Hurreehh!",
                text: "Records Exported",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            });

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Filter Search Records *****************************************************/
function searchRecordsFilter(callId) {
    event.preventDefault();

    var formData;

    if (callId != 1) {
        formData = $('#searchRecordsForm').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetSearchRecords",
        data: formData,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Delete in Search Records *****************************************************/
function deletRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/Admin/deletRequest",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(result);
            Swal.fire({
                title: "Hurreehh!",
                text: "Request Deleted!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            });
            GetSearchRecords();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Payrate *****************************************************/
function GetEnterPayrate(aspId, phyid) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEnterPayrate",
        data: { aspId: aspId, phyid: phyid },

        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            $('#Provider-tab-pane').html(response);
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
/******************************************** Update Payrate Records *****************************************************/
function PostPayrate(counter) {
    event.preventDefault();

    var category = $(`#payratecategory${counter}`).val();
    var payrate = $(`#payRateField${counter}`).val();
    var phyid = $(`#phyid${counter}`).val();

    $.ajax({
        method: "POST",
        url: "/Admin/PostPayrate",
        data: { category: category, payrate: payrate, phyid: phyid },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurreehh!",
                text: "Payrate Updated",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            })
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Admin Invocing *****************************************************/
function GetAdminInvoicing() {
    $.ajax({
        method: "GET",
        url: "/Admin/GetAdminInvoicing",

        success: function (result) {
            $('#Invoicing-tab-pane').html(result);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}
/******************************************** Fetch Admin Invocing for Particular Physician's Selected Time Duration *****************************************************/
function GetTimeSheetDetail() {
    var phyid = $('#phyDropdown').val();
    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        method: "GET",
        url: "/Admin/GetTimeSheetDetail",
        data: { phyid: phyid, dateSelected: dateSelected },

        success: function (result) {
            $('#Invoicing-tab-pane').html(result);
            $('#phyDropdown').val(phyid);
            $('#searchByTimeperiod').val(dateSelected);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}
/******************************************** Fetch Admin Finalize Timesheet *****************************************************/
function GetAdminFinalizeTimeSheet() {

    var dateSelected = $('#searchByTimeperiod').val();
    var phyid = $('#phyDropdown').val();

    $.ajax({
        methd: "GET",
        url: "/Admin/GetAdminFinalizeTimeSheet",
        data: { dateSelected: dateSelected, phyid: phyid },

        success: function (result) {
            $('#Invoicing-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Approve Timesheet *****************************************************/
function ConfirmApproveTimeSheet(timeSheetId) {
    event.preventDefault();

    var bonus = $('#bonusAmount').val();
    var notes = $('#adminNotes').val();
    if (bonus < 0 || notes == null || notes.trim() == "") {
        Swal.fire({
            title: "Opps!",
            text: "Enter Valid Inputs",
            icon: "error",
            timer: 3000,
            timerProgressBar: true,
        });
        return;
    }
    $.ajax({
        method: "POST",
        url: "/Admin/ConfirmApproveTimeSheet",
        data: { timeSheetId: timeSheetId, bonus: bonus, notes: notes },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurehh!",
                text: "Timesheet Approved!",
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
            });
            GetAdminInvoicing();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** _ *****************************************************/
/******************************************** _ *****************************************************/
/******************************************** _ *****************************************************/