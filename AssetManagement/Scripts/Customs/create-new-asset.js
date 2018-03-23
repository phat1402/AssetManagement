$(document).ready(function () {
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
            minimumInputLength: 2,
            width: 'resolve'
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
            minimumInputLength: 2,
            width: 'resolve'
        }
    });

});