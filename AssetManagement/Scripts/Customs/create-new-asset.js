$(document).ready(function () {
    var categoryData;
    $.ajax({
        async: false,
        url: '/DataSource/GetCategoriesList',
        success: function (response) {
            categoryData = response.items;
        },
        error: function () {
            categoryData = [];
        }
    });
    $(".choose-category").select2({
        data: categoryData
    });
    $(".choose-vendor").select2({
        placeholder: "Select a vendor",
        allowClear: true,
        ajax: {
            url: '/DataSource/GetVendorList',
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
    $(".choose-department").select2({
        placeholder: "Select a department",
        allowClear: true,
        ajax: {
            url: '/DataSource/GetDepartmentList',
            width: 'resolve',
            delay: 250,
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

    $(".choose-location").select2({
        placeholder: "Select a location",
        allowClear: true,
        ajax: {
            url: '/DataSource/GetLocationList',
            width: 'resolve',
            delay: 250,
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

    $("#assetLocation").on('change', function () {
        var data = $(".choose-location").select2("data");
    });

});