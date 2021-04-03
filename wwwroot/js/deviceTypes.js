var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/deviceTypes/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "deviceName", "width": "40%" },
            { "data": "transportCost", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {

                    return `<div class="text-center"> 
                            <a href="/admin/deviceTypes/Manage/${data}" class='btn btn-success text-white' style='cursor:pointer; width:120px;' >
                                <i class='far fa-edit'>  </i> Edytuj
                            </a>                      
                            &nbsp;
                            <a class='btn btn-danger text-white' style='cursor:pointer; width:100px;' onclick=Delete('/admin/deviceTypes/Delete/${data}')>
                               <i class='far fa-trash-alt'> </i> Usuń   
                            </a></div>                        
                        `;
                }, "width": "40%"
            }


        ],
        "language": {
            "emptyTable": "Brak urządzeń.",
            "info": "Wyświetla od <b>_START_ - _END_ </b>z _TOTAL_ urządzeń",
            "infoEmpty": "Wyświetla 0 na 0 of 0 typów urządzeń",
            "infoFiltered": "(wyszukano z _MAX_ wszystkich urządzeń)",
            "zeroRecords": "Nie znaleziono żadnych dopasowań",
            "loadingRecords": "Wczytywanie...",
            "processing": "Przetwarzanie...",
            "search": "Wyszukaj:",
            "lengthMenu": "Pokaż _MENU_ urządzeń",
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
        title: "Czy napewno chcesz usunąć?",
        text: "Nie będzie możliwość cofnięcia zmian. Urządzenie zostanie usunięte!",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: "Anuluj",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Tak, usuń!",
        closeOnConfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();

                }
                else {
                    toastr.error(data.message);
                }
            }
        });
    });
}

