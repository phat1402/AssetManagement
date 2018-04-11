function printTag(elem) {
    var tag = $(elem).data('tag');
    var assetName = $(elem).data('assetname');
    var mywindow = window.open('', 'PRINT', 'height=400,width=600');

    mywindow.document.write('<html><head><title> Print Tag </title>');
    mywindow.document.write('</head><body >');
    mywindow.document.write('<div style="border-style:solid;padding:20px">');
    mywindow.document.write('<h1 style= "text-align: center"> ' + assetName + ' </h1>');
    mywindow.document.write('<h2 style= "text-align: center">' + tag + '</h2>');
    mywindow.document.write('</div>');
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    mywindow.print();
    mywindow.close();

    return true;
}

function printMultipleTag() {
    var selected = [];
    $('#check_box_body input:checked').each(function () {
        selected.push($(this).val());
    });
    $.ajax({
        method: "POST",
        async: false,
        url: '/Home/GetDataToPrintLabel',
        data: { assetIdList: selected },
        success: function (response) {
            var mywindow = window.open('', 'PRINT', 'height=400,width=600');

            mywindow.document.write('<html><head><title> Print Tag </title>');
            mywindow.document.write('</head><body >');
            for (var i = 0; i < response.length; i++) {
                mywindow.document.write('<div style="border-style:solid;padding:20px;margin-bottom:10px">');
                mywindow.document.write('<h1 style= "text-align: center"> ' + response[i].AssetName + ' </h1>');
                mywindow.document.write('<h2 style= "text-align: center">' + response[i].AssetTag + '</h2>');
                mywindow.document.write('</div>');
            }
            mywindow.document.write('</body></html>');

            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10*/

            mywindow.print();
            mywindow.close();

            return true;          
        },
        error: function () {
            alert("Something wrong happened!");
        }
    });
    return true;
}

