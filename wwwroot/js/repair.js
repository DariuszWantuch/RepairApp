var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({      
        "ajax": {
            "url": "/customer/repair/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "repairId", "width": "6%"},
            { "data": "status.repairStatus", "width": "32%" },
            { "data": "deviceType.deviceName", "width": "32%" },        
            {
                "data": "id",
                "render": function (data) {

                    return `<div class="text-center"> 
                            <a href="/Customer/repair/Details/${data}" class='btn btn-success text-white' style='cursor:pointer; width:120px;' >
                            <i class='far fa-edit'>  <b>Szczegóły</i> 
                            </a>                           
                            
                        `;
                }, "width": "30%"
            }


        ],
        "language": {
            "emptyTable": "Brak zgłoszeń.",
            "info": "Wyświetla od <b>_START_ - _END_ </b>z _TOTAL_ zgłoszeń",
            "infoEmpty": "Wyświetla 0 na 0 of 0 zgłoszeń",
            "infoFiltered": "(wyszukano z _MAX_ wszystkich zgłoszeń)",
            "zeroRecords": "Nie znaleziono żadnych dopasowań",
            "loadingRecords": "Wczytywanie...",
            "processing": "Przetwarzanie...",
            "search": "Wyszukaj:",
            "lengthMenu": "Pokaż _MENU_ zgłoszeń",
            "paginate": {
                "first": "Pierwsza",
                "last": "Ostatnia",
                "next": "Następny",
                "previous": "Poprzedni"
            },
            "aria": {
                "sortAscending": ": aktywowano sortowanie rosnąco",
                "sortDescending": ": aktywowano sortowanie malejąco"
            }
        },
        "width": "100%"
    });
}


    function Delete(url) {
        swal({
            title: "Jesteś pewien?",
            text: "Zakończysz zgłoszenie, twój sprzęt nie zostanie naprawiony!",
            type: "warning",
            showCancelButton: true,
            cancelButtonText: "Anuluj",
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Tak, zakończ!",
            closeOnConfirm: true
        }, function () {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                   
                    }
                    else {
                        toastr.error(data.message);
                      
                    }
                }
            });
                window.location = "/customer/repair/Index";
        });
    }
      
