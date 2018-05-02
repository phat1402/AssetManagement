$(document).ready(function () {
    var categoryData;
    var vendorData;
    var departmentData;
    var locationData;
    var employeeData;

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
    $.ajax({
        async: false,
        url: '/DataSource/GetVendorList',
        success: function (response) {
            vendorData = response.items;
        },
        error: function () {
            vendorData = [];
        }
    });
    $.ajax({
        async: false,
        url: '/DataSource/GetDepartmentList',
        success: function (response) {
            departmentData = response.items;
        },
        error: function () {
            departmentData = [];
        }
    });
    $.ajax({
        async: false,
        url: '/DataSource/GetLocationList',
        success: function (response) {
            locationData = response.items;
        },
        error: function () {
            locationData = [];
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
    
    
    $(".choose-category").select2({
        data: categoryData
    });
    $(".choose-vendor").select2({
        data: vendorData
    }); 
    $(".choose-department").select2({
        data: departmentData
    });
    $(".choose-location").select2({
        data: locationData
    });
    $(".assign-staff").select2({
        data: employeeData
    });
});
function OnSuccess(result) {
    alert(result);
    location.reload();
}