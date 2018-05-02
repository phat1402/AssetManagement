var TableDatatablesManaged = function () {

    var initTable1 = function () {

        var table = $('#sample_1');

        // begin first table
        table.dataTable({

            // Internationalisation. For more info refer to http://datatables.net/manual/i18n
            "language": {
                "aria": {
                    "sortAscending": ": activate to sort column ascending",
                    "sortDescending": ": activate to sort column descending"
                },
                "emptyTable": "No data available in table",
                "info": "Showing _START_ to _END_ of _TOTAL_ records",
                "infoEmpty": "No records found",
                "infoFiltered": "(filtered1 from _MAX_ total records)",
                "lengthMenu": "Show _MENU_",
                "search": "Search:",
                "zeroRecords": "No matching records found",
                "paginate": {
                    "previous":"Prev",
                    "next": "Next",
                    "last": "Last",
                    "first": "First"
                }
            },

            // Or you can use remote translation file
            //"language": {
            //   url: '//cdn.datatables.net/plug-ins/3cfcc339e89/i18n/Portuguese.json'
            //},

            // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
            // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.js). 
            // So when dropdowns used the scrollable div should be removed. 
            //"dom": "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r>t<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",

            "bStateSave": true, // save datatable state(pagination, sort, etc) in cookie.

            "lengthMenu": [
                [5, 15, 20, -1],
                [5, 15, 20, "All"] // change per page values here
            ],
            // set the initial value
            "pageLength": 5,            
            "pagingType": "bootstrap_full_number",
            "columnDefs": [
                {  // set default column settings
                    'orderable': false,
                    'targets': [0]
                }, 
                {
                    "searchable": false,
                    "targets": [0]
                },
                {
                    "className": "dt-right", 
                    //"targets": [2]
                }
            ],
            "order": [
                [1, "asc"]
            ] // set first column as a default sort by asc
        });

        var tableWrapper = jQuery('#sample_1_wrapper');

        table.find('.group-checkable').change(function () {
            var set = jQuery(this).attr("data-set");
            var checked = jQuery(this).is(":checked");
            jQuery(set).each(function () {
                if (checked) {
                    $(this).prop("checked", true);
                    $(this).parents('tr').addClass("active");
                } else {
                    $(this).prop("checked", false);
                    $(this).parents('tr').removeClass("active");
                }
            });
        });

        table.on('change', 'tbody tr .checkboxes', function () {
            $(this).parents('tr').toggleClass("active");
        });
    }

    return {
        //main function to initiate the module
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            initTable1();
        }


    };

}();

if (App.isAngularJsApp() === false) { 
    jQuery(document).ready(function () {
        var storeData;
        var employeeData;
        $.ajax({
            async: false,
            url: '/DataSource/GetStoreList',
            success: function (response) {
                storeData = response.items;
            },
            error: function () {
                storeData = [];
            }
        });
        $.ajax({
            async: false,
            url: '/DataSource/GetEmployeeList',
            success: function (response) {
                employeeData = response.items;
            },
            error: function () {
                employeeData = [];
            }
        });

        $(".select-store").select2({          
            data: storeData
        });

        $(".assign-staff").select2({
            data: employeeData
        });
        TableDatatablesManaged.init();
    });
}


function filterAssetByStore() {
    var storeId = $('.select-store').select2('val');
    $.ajax({
        async: false,
        url: '/Home/FilterAssetByStoreForDistribute',
        data: { storeId: storeId },
        success: function (response) {
            $("#assetByStoreTable").html(response);
            TableDatatablesManaged.init()
        },
        error: function () {
            alert("Oops! Something wrong!");
            location.reload();
        }
    });
}

function assignAsset() {
    var selected = [];
    var employeeId = $('.assign-staff').select2('val');
    $('#check_box_body input:checked').each(function () {
        selected.push($(this).val());
    });
    $.ajax({
        method: "POST",
        async: false,
        url: '/Home/AssignAsset',
        data: { assetIdList: selected, employeeId: employeeId },
        success: function (response) {
            alert(response);
            location.reload()
        },
        error: function () {
            alert("Something wrong happened!");
            location.reload();
        }
    });
}