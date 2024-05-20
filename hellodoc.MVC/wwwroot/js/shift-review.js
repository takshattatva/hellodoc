$('#approveBtn').click(function () {
    var shiftDetailsId = [];
    var regionId = $('#regionValue').val();

    $('.fileCheckbox:checked').each(function () {
        var fileId = $(this).val();
        shiftDetailsId.push(fileId);
    });

    ApproveShift(shiftDetailsId, regionId);
});

$('#deleteBtn').click(function () {
    var shiftDetailsId = [];
    var regionId = $('#regionValue').val();

    $('.fileCheckbox:checked').each(function () {
        var fileId = $(this).val();
        shiftDetailsId.push(fileId);
    });

    DeleteSelectedShift(shiftDetailsId, regionId);
});

$(document).ready(function () {
    $('#ShiftReviewTable').DataTable({
        "lengthMenu": [[10, 20, -1], [10, 20, "All"]],
        "pageLength": 10,

        language: {
            oPaginate: {
                sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
                sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

            }
        }
    });
    $('.dataTables_filter').hide();
});