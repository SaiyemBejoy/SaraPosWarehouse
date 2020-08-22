jQuery(document).ready(function () {
    $('.select2').select2({
        allowClear: true
    });

    $(".datepicker2").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"
    });

    $('#btnSave').click(function () {
        shopLaunchDateSave();
    });
});


function validationForm() {
    var styleName = $("#StyleName").val();
    if (styleName === '') {
        toastr.error("StyleName Date Can't be empty.");
        return false;
    }
    var launchDate = $("#LaunchDate").val();
    if (launchDate === '') {
        toastr.error("Launch Date Can't be empty.");
        return false;
    }
    var shopId = $("#ShopId").val();
    if (shopId === '') {
        toastr.error(" Select ShopName !!");
        return false;
    }

    return true;
}

function shopLaunchDateSave() {
    var styleName = $("#StyleName").val();
    var launchDate = $("#LaunchDate").val();
    var shopId = $("#ShopId").val();
    var dateObject= {
        'ProductId': styleName,
        'LaunchDate': launchDate,
        'ShopId': shopId
    };
    var dataList = JSON.stringify({ objShopLaunchModel: dateObject });
    if (validationForm()) {
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json',
            url: '/ShopLaunch/SaveAllData/',
            data: dataList,
            beforeSend: function () {
                $('#preLoader').show();
            },
            success: function (data) {
                if (data.isRedirect) {
                    window.setTimeout(function () {
                        window.location = data.redirectUrl;
                    }, 1000);
                    toastr.success("Save Successfully.");
                    $('#preLoader').hide();
                }
            }
        });
    } 
}