$(document).ready(function () {
    var employeeData;
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

    $(".choose-assigned-staff").select2({
        data: employeeData
    });
});