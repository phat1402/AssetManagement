google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawChart);
$(document).ready(function () {
    $.ajax({
        url: "Home/GetDataForDashboard",
        method: "POST",
        success: function (response) {
            categoryChartData = response.CategoryDataList;
            departmentChartData = response.DepartmentDataList;
            vendorChartData = response.VendorDataList;
            drawChart(categoryChartData, departmentChartData, vendorChartData);
        }
    });
});
function drawChart(categoryChartData, departmentChartData, vendorChartData) {

    var categoryData = new google.visualization.DataTable();
    categoryData.addColumn('string', 'Category Name');
    categoryData.addColumn('number', 'Count');
    for (var i = 0; i < categoryChartData.length; i++) {
        categoryData.addRow([categoryChartData[i].CategoryName, categoryChartData[i].CategoryCount]);
    }

    var departmentData = new google.visualization.DataTable();
    departmentData.addColumn('string', 'Department Name');
    departmentData.addColumn('number', 'Count');
    for (var i = 0; i < departmentChartData.length; i++) {
        departmentData.addRow([departmentChartData[i].DepartmentName, departmentChartData[i].DepartmentCount]);
    }

    var vendorData = new google.visualization.DataTable();
    vendorData.addColumn('string', 'Category Name');
    vendorData.addColumn('number', 'Count');
    for (var i = 0; i < vendorChartData.length; i++) {
        vendorData.addRow([vendorChartData[i].VendorName, vendorChartData[i].VendorCount]);
    }


    var category_options = {
        title: 'Asset By Categories'
    };

    var deparment_options = {
        title: 'Asset By Department'
    };

    var vendor_options = {
        title: 'Asset By Vendor'
    };

    var category_chart = new google.visualization.PieChart(document.getElementById('category_pie_chart'));
    var department_chart = new google.visualization.PieChart(document.getElementById('department_pie_chart'));
    var vendor_chart = new google.visualization.PieChart(document.getElementById('vendor_pie_chart'));

    category_chart.draw(categoryData, category_options);
    department_chart.draw(departmentData, deparment_options);
    vendor_chart.draw(vendorData, vendor_options);
}