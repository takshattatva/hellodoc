document.getElementById('ContactProviderForm').addEventListener('submit', function (event) {
    event.preventDefault();

    var selectedRadio = document.querySelector('input[name="ContactRadio"]:checked').value;

    if (selectedRadio === '1') {
        ContactToProvider();
    }
    else if (selectedRadio === '2') {
        SendEmailToProvider();
    }
    else if (selectedRadio === '3') {
        SendEmailToProvider();
        ContactToProvider();
    }
});

$(document).ready(function () {
    $('.provider').DataTable({
        "initComplete": function (settings, json) {

            $('#my-search-input').val(settings.oPreviousSearch.sSearch);

            $('#my-search-input').on('keyup', function () {
                var searchValue = $(this).val();
                settings.oPreviousSearch.sSearch = searchValue;
                settings.oApi._fnReDraw(settings);
            });
        },
        "lengthMenu": [[10, 20, -1], [10, 20, "All"]],
        "pageLength": 10,
        language: {
            oPaginate: {
                sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
                sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

            }
        }
    });
    //$('.dataTables_filter').hide();
});