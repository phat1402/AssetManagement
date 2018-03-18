google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawChart);

function drawChart() {

    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['Work', 11],
        ['Eat', 2],
        ['Commute', 2],
        ['Watch TV', 2],
        ['Sleep', 7]
    ]);

    var options = {
        title: 'My Daily Activities'
    };

    var category_chart = new google.visualization.PieChart(document.getElementById('category_pie_chart'));
    var department_chart = new google.visualization.PieChart(document.getElementById('department_pie_chart'));
    var vendor_chart = new google.visualization.PieChart(document.getElementById('vendor_pie_chart'));

    category_chart.draw(data, options);
    department_chart.draw(data, options);
    vendor_chart.draw(data, options);
}