$(document).ready(function () {
    var employeeData;
    $(".choose-asset").select2({
        placeholder: "Input an asset tag or asset name",
        allowClear: true,
        ajax: {
            url: '/DataSource/GetAssetList',
            width: 'resolve',
            data: function (params) {
                return {
                    query: params.term// search term
                };
            },
            processResults: function (data) {
                return {
                    results: data.items
                };
            },
            minimumInputLength: 2
        }
    }); 
    //from employee must be depend on the asset 
    var fromEmployee;
    $('.choose-asset').on('select2:select', function (e) {
        var data = e.params.data;
        var assetID = data.id;
        $.ajax({
            async: false,
            url: '/DataSource/GetSourceEmployee',
            data: { assetID: assetID },
            success: function (response) {
                fromEmployee = response.items;
            },
            error: function () {
                fromEmployee = [];
            }
        });
        $(".choose-from-employee").select2({
            data: fromEmployee
        }); 
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
    $(".choose-to-employee").select2({
        placeholder: "Input an employee name",
        allowClear: true,
        data: employeeData
    }); 
});