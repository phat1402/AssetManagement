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

    $(".choose-store").select2({
        placeholder: "Select a store",
        allowClear: true,
        ajax: {
            url: '/DataSource/GetStoreList',
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

});