jQuery(document).ready(function () {

    //Checkbox for selecting all
    $("#checkboxAll").live("click", function () {
        var isChecked = this.checked;
        $("#giftVoucherBarcodeDeliveryBody").find("input#checkbox").each(function () {
            this.checked = isChecked;
        });
    });


    $('#GiftVoucherValue').on('change', function () {
        var giftVoucherId = $("#GiftVoucherValue").val();
        giftVoucherGenerateItemList(giftVoucherId);
    });

    $('#btnSave').click(function (){
        giftVoucherDeliverySave();
    });
});

function giftVoucherGenerateItemList(giftVoucherId) {

    var dataId = JSON.stringify({ giftVoucherId: giftVoucherId });
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        url: '/Setup/GiftVoucherItemListById/',
        data: dataId,
        success: function (data) {
            var tableQuantity = data.length;
            $("#giftVoucherBarcodeDeliveryBody").html("");
            for (var i = 0; i < tableQuantity; i++) {
                $("#giftVoucherBarcodeDeliveryBody").append('<tr>' +
                    '<td>' +
                    parseInt(i + 1) +
                    '</td>' +
                    '<td style="display:none;">' +
                    data[i].GiftVoucherId +
                    '</td>' +
                    '<td>' +
                    data[i].GiftVoucherCode +
                    '</td>' +
                    '<td>' +
                    data[i].GiftVoucherValue +
                    '</td>' +
                    '<td>' +
                    "<input type ='checkbox' class ='checker'  id='checkbox'/>"+
                    '</td>' +
                    '</tr>'
                );

            }
        }
    });
}


function giftVoucherDeliverySave() {
    var shopId = $("#ShopId").val();

    if (shopId != "") {
        var allGiftVoucherDeliveryItem = [];
        allGiftVoucherDeliveryItem.length = 0;
        //$.each($("#giftVoucherBarcodeDeliveryBody tr"),
        //    function () {
        //        allGiftVoucherDeliveryItem.push({
        //            GiftVoucherId: $(this).find('td:eq(1)').text(),
        //            GiftVoucherCode: $(this).find('td:eq(2)').text(),
        //            GiftVoucherValue: $(this).find('td:eq(3)').text(),
        //            GiftVoucheritemId: $(this).find('td:eq(4)').text()
        //        });
        //    });
        $.each($("input[id='checkbox']:checked"),
            function () {
                allGiftVoucherDeliveryItem.push({
                    ShopId: shopId,
                    GiftVoucherId: $(this).parents('tr').find('td:eq(1)').text(),
                    GiftVoucherCode: $(this).parents('tr').find('td:eq(2)').text(),
                    GiftVoucherValue: $(this).parents('tr').find('td:eq(3)').text(),
                    GiftVoucheritemId: $(this).parents('tr').find('td:eq(4)').text()
                });
            });

        var dataList = JSON.stringify({ objGiftVoucherDeliveryModel: allGiftVoucherDeliveryItem });
        if (allGiftVoucherDeliveryItem.length) {
            $.ajax({
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                url: '/Setup/SaveAndUpdateGiftVoucherDelivery/',
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

        } else {
            toastr.error("Must Select GiftVoucher!");
        }
    } else {
        toastr.error("Must Select Shop Name!");
    }
}