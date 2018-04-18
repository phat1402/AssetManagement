function updateUserInfor() {
    var url = $('#personal-infor-form').data('url');
    var data = {
        ID: $('#ID').val(),
        Firstname: $('#Firstname').val(),
        Lastname: $('#Lastname').val(),
        Phone: $('#Phone').val(),
        Country: $('#Country').val()
    }

    $.ajax({
        method: "POST",
        url: url,
        data: data,
        success: function (response) {
            alert(response.message);
            location.reload();
        },
        error: function (error) {
            alert(error);
            location.reload();
        }
    });
}

function changePassword() {
    var url = $('#change-password-form').data('url');
    var data = {
        ID: $('#ID').val(),
        CurrentPass: $('#PasswordTab_CurrentPass').val(),
        NewPass: $('#PasswordTab_NewPass').val(),
        RetypePass: $('#PasswordTab_RetypePass').val()
    }

    $.ajax({
        method: "POST",
        url: url,
        data: data,
        success: function (response) {
            alert(response);
            location.reload();
        },
        error: function (error) {
            alert(error);
            location.reload();
        }
    });
}

function updateCompanySetting() {
    var url = $('#setting-infor-form').data('url');
    var data = {
        CompanyID: $('#CompanyID').val(),
        CompanyDescription: $('#CompanyDescription').val(),
        CompanyPhone: $('#CompanyPhone').val(),
        CompanyEmail: $('#CompanyEmail').val(),
        Address: $('#Address').val(),
        Country: $('#Country').val()
    }

    $.ajax({
        method: "POST",
        url: url,
        data: data,
        success: function (response) {
            alert(response);
            location.reload();
        },
        error: function (error) {
            alert(error);
            location.reload();
        }
    });
}

function changeRole(ele) {
    var role = '#role' + $(ele).data('id');
    var selectrole = '#select-role' + $(ele).data('id');
    var changerole = '#change-role' + $(ele).data('id');
    var saverole = '#save-role' + $(ele).data('id');
    var cancel = '#cancel' + $(ele).data('id');

    $(role).addClass('hide');
    $(selectrole).removeClass('hide');

    $(changerole).addClass('hide');
    $(saverole).removeClass('hide');
    $(cancel).removeClass('hide');

}

function saveRole(ele) {
    var userId = $(ele).data('id');
    var selectedrole = '#select-role' + $(ele).data('id') + ' option:selected';
    var selected = $(selectedrole).val();
    $.ajax({
        method: "POST",
        url: '/Account/UpdateRole',
        data: {
            userId: userId,
            roleId: selected
        },
        success: function (response) {
            alert(response);
            location.reload();
        }
    });
}
function cancelSaveRole() {
    location.reload();
}
