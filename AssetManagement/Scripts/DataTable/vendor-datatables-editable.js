var TableDatatablesEditable = function () {

    var handleTable = function () {

        function restoreRow(oTable, nRow) {
            var aData = oTable.fnGetData(nRow);
            var jqTds = $('>td', nRow);

            for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
                oTable.fnUpdate(aData[i], nRow, i, false);
            }

            oTable.fnDraw();
        }

        function editRow(oTable, nRow) {
            var aData = oTable.fnGetData(nRow);
            var jqTds = $('>td', nRow);
            jqTds[1].innerHTML = '<input type="text" class="form-control input" value="' + aData[1] + '">';
            jqTds[2].innerHTML = '<input type="text" class="form-control input" value="' + aData[2] + '">';
            jqTds[3].innerHTML = '<input type="text" class="form-control input" value="' + aData[3] + '">';
            jqTds[4].innerHTML = '<a class="edit" href="">Save</a>';
            jqTds[5].innerHTML = '<a class="cancel" href="">Cancel</a>';
        }
        function saveRow(oTable, nRow) {
            var jqInputs = $('input', nRow);
            vendorID = $(nRow).data("id");
            vendorName = jqInputs[0].value;
            vendorEmail = jqInputs[1].value;
            vendorTelephoneNo = jqInputs[2].value;
            $.ajax({
                data: {
                    ID: vendorID,
                    Name: vendorName,
                    Email: vendorEmail,
                    TelephoneNo: vendorTelephoneNo
                },
                type: "POST",
                url: "/Home/CreateOrUpdateVendor",
                success: function (response) {
                    if (response.RequestType == "Update") {
                        alert(response.Message);
                        if (response.Message !== "Update successfully") {
                            location.reload();
                        }
                        else {
                            oTable.fnUpdate(jqInputs[0].value, nRow, 1, false);
                            oTable.fnUpdate(jqInputs[1].value, nRow, 2, false);
                            oTable.fnUpdate(jqInputs[2].value, nRow, 3, false);
                            oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
                            oTable.fnUpdate('<a class="delete" href="">Delete</a>', nRow, 5, false);
                            oTable.fnDraw();
                        }
                    }
                    else {
                        alert(response.Message);
                        if (response.Message !== "Create successfully") {
                            location.reload();
                        }
                        else {
                            oTable.fnUpdate(response.ID, nRow, 0, false);
                            oTable.fnUpdate(response.Name, nRow, 1, false);
                            oTable.fnUpdate(response.Email, nRow, 2, false);
                            oTable.fnUpdate(response.TelephoneNo, nRow, 3, false);
                            oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
                            oTable.fnUpdate('<a class="delete" href="">Delete</a>', nRow, 5, false);
                            oTable.fnDraw();
                        }
                    }
                }
            });
            $('#vendor-list-table_new').prop("disabled", false);
        }

        function cancelEditRow(oTable, nRow) {
            var jqInputs = $('input', nRow);
            oTable.fnUpdate(jqInputs[0].value, nRow, 1, false);
            oTable.fnUpdate(jqInputs[1].value, nRow, 2, false);
            oTable.fnUpdate(jqInputs[2].value, nRow, 3, false);
            oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
            oTable.fnDraw();
        }

        var table = $('#vendor-list-table');

        var oTable = table.dataTable({

            // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
            // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.js). 
            // So when dropdowns used the scrollable div should be removed. 
            //"dom": "<'row'<'col-md-6 col-sm-12'l><'col-md-sample_editable_16 col-sm-12'f>r>t<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",

            "lengthMenu": [
                [5, 15, 20, -1],
                [5, 15, 20, "All"] // change per page values here
            ],

            // Or you can use remote translation file
            //"language": {
            //   url: '//cdn.datatables.net/plug-ins/3cfcc339e89/i18n/Portuguese.json'
            //},

            // set the initial value
            "pageLength": 5,

            "language": {
                "lengthMenu": " _MENU_ records"
            },
            "columnDefs": [{ // set default column settings
                'orderable': true,
                'targets': [0]
            }, {
                "searchable": true,
                "targets": [0]
            }],
            "order": [
                [0, "asc"]
            ] // set first column as a default sort by asc
        });

        var tableWrapper = $("#vendor-list-table_wrapper");

        var nEditing = null;
        var nNew = false;

        $('#vendor-list-table_new').click(function (e) {
            e.preventDefault();

            if (nNew && nEditing) {
                if (confirm("Previose row not saved. Do you want to save it ?")) {
                    saveRow(oTable, nEditing); // save
                    $(nEditing).find("td:first").html("Untitled");
                    nEditing = null;
                    nNew = false;

                } else {
                    oTable.fnDeleteRow(nEditing); // cancel
                    nEditing = null;
                    nNew = false;
                    
                    return;
                }
            }

            var aiNew = oTable.fnAddData(['', '', '', '', '', '']);
            var nRow = oTable.fnGetNodes(aiNew[0]);
            editRow(oTable, nRow);
            nEditing = nRow;
            nNew = true;
            $('#vendor-list-table_new').prop("disabled", true);
        });

        table.on('click', '.delete', function (e) {
            e.preventDefault();

            if (confirm("Are you sure to delete this row ?") == false) {
                return;
            }
            var vendorId = $(this).data("id");
            var nRow = $(this).parents('tr')[0];
            oTable.fnDeleteRow(nRow);
            $.ajax({
                url: "/Home/DeleteVendor",
                type: "POST",
                data: {
                    vendorId: vendorId
                },
                async: false,
                success: function (response) {
                    if (response == "Success") {
                        alert("Delete Successfully!");
                        location.reload();
                    }
                    else {
                        alert("Delete Failed");
                        location.reload();
                    }
                }
            });
        });

        table.on('click', '.cancel', function (e) {
            e.preventDefault();
            if (nNew) {
                oTable.fnDeleteRow(nEditing);
                nEditing = null;
                nNew = false;
                $('#vendor-list-table_new').prop("disabled", false);
            } else {
                restoreRow(oTable, nEditing);
                nEditing = null;
            }
        });

        table.on('click', '.edit', function (e) {
            e.preventDefault();

            nNew = false;
            
            /* Get the row as a parent of the link that was clicked on */
            var nRow = $(this).parents('tr')[0];

            if (nEditing !== null && nEditing != nRow) {
                /* Currently editing - but not this row - restore the old before continuing to edit mode */
                restoreRow(oTable, nEditing);
                editRow(oTable, nRow);
                nEditing = nRow;
            } else if (nEditing == nRow && this.innerHTML == "Save") {
                /* Editing this row and want to save it */
                saveRow(oTable, nEditing);
                nEditing = null;
            } else {
                /* No edit in progress - let's start one */
                editRow(oTable, nRow);
                nEditing = nRow;
            }
        });
    }

    return {

        //main function to initiate the module
        init: function () {
            handleTable();
        }

    };

}();

jQuery(document).ready(function() {
    TableDatatablesEditable.init();
});