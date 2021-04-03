var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/repairs/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "repairId", "width": "6%" },
            { "data": "status.repairStatus", "width": "32%" },
            { "data": "deviceType.deviceName", "width": "32%" },
            {
                "data": "id",
                "render": function (data) {

                    return `<div class="text-center"> 
                            <a href="/Admin/repairs/UpdateStatus/${data}" class='btn btn-primary text-white' style='cursor:pointer; width:110px;' >
                            <i class='fas fa-screwdriver'> </i> Status
                            </a> 
                            <a href="/Admin/repairs/Details/${data}" class='btn btn-success text-white' style='cursor:pointer; width:140px;' >
                            <i class='fas fa-info-circle'> </i> Szczegóły
                            </a>  
                            </div>
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
