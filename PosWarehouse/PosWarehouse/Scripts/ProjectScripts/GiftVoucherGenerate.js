function totalItemCount() {
    var row = $('#GiftVoucherItemTableBody tr').length;
    $("#totalGenerateItem").val(row);

}

function giftVoucherGenerateItemList(giftVoucherId) {

    var dataId = JSON.stringify({ giftVoucherId: giftVoucherId });
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        //url: '@Url.Action("GiftVoucherItemListById", "Setup")',
        url: '/Setup/GiftVoucherItemListById/',
        data: dataId,
        success: function (data) {
            var tableQuantity = data.length;
            $("#GiftVoucherItemTableBody").html("");
            for (var i = 0; i < tableQuantity; i++) {
                $("#GiftVoucherItemTableBody").append('<tr>' +
                    '<td>' +
                    parseInt(i + 1) +
                    '</td>' +
                    '<td>' +
                    data[i].GiftVoucherCode +
                    '</td>' +
                    '<td>' +
                    data[i].GiftVoucherValue +
                    '</td>' +
                    '</tr>'
                );
                totalItemCount();
            }
        }
    });
}
function giftVoucherGeneratePrintItemList(giftVoucherId) {

    var dataId = JSON.stringify({ giftVoucherId: giftVoucherId });
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        //url: '@Url.Action("GiftVoucherItemListById", "Setup")',
        url: '/Setup/GiftVoucherItemListById/',
        data: dataId,
        success: function (data) {
            var tableQuantity = data.length;
            $("#GiftVoucherPrintItemTableBody").html("");
            for (var i = 0; i < tableQuantity; i++) {
                $("#GiftVoucherPrintItemTableBody").append('<tr>' +
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
                    '<td><a href="#" class="deleteItem btn btn-danger btn-xs"><i class="fa fa-trash-o"></i> Remove</a></td>' +
                    '</tr>'
                );
                totalItemCount();
            }
        }
    });
}

jQuery(document).ready(function () {

    $('#noOfcardText').on('change',
        function () {
            $(this).val($(this).val());
            $.ajax({
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                //url: '@Url.Action("GiftVoucherLastStartFrom", "Setup")',
                url: '/Setup/GiftVoucherLastStartFrom/',
                success: function (data) {
                    var startData = 1;
                    if (data.EndFrom !== null) {
                        startData = parseInt(data.EndFrom);
                    }
                    compute(startData);
                }
            });

        });

    // After Add A New Order In The List, If You Want, You Can Remove It.
    $(document).on('click', 'a.deleteItem', function (e) {
        e.preventDefault();
        var self = $(this);
        if (self !== null) {
            $(this).parents('tr').css("background-color", "#dc143c").fadeOut(800, function () {
                $(this).remove();
            });
        } else {
            toastr.error("Data cann't delete");
        }
    });
    $('#startfromtext').on('change', function () {
        $(this).val($(this).val());
        compute2();
    });
    $('#printGiftVoucher').on('click', function () {
        var allGiftVoucherPrintItem = [];
        allGiftVoucherPrintItem.length = 0;
        $.each($("#GiftVoucherPrintItemTableBody tr"),
            function () {
                allGiftVoucherPrintItem.push({
                    GiftVoucherId: $(this).find('td:eq(1)').text(),
                    GiftVoucherCode: $(this).find('td:eq(2)').text(),
                    Quantity: 1,
                    GiftVoucherValue: $(this).find('td:eq(3)').text()
                });
            });
        var dataList = JSON.stringify({ objGiftVoucherGeneratePrintItemModel: allGiftVoucherPrintItem });

        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json',
            //url: '@Url.Action("PrintAllGiftVoucherCode", "Setup")',
            url: '/Setup/PrintAllGiftVoucherCode/',
            data: dataList,
            beforeSend: function () {
                $('#preLoader').show();
            },
            success: function (data) {
                var giftVoucherId = data.GiftVoucherId;
                if (data.length > 0) {
                    window.open('/Report/GiftVoucherGenerateCodePrint?giftVoucherId=' + data[0].GiftVoucherId, '_blank');
                } else {
                    toastr.error("This Data Cann't Saved !!");
                    $('#preLoader').hide();
                }
                $('#preLoader').hide();
            }

        });
    });

    $(document).on('click', '.giftVoucherItem', function () {
        var giftVoucherId = $(this).data('id');
        giftVoucherGenerateItemList(giftVoucherId);
    });

    $(document).on('click', '.giftVoucherPrintItem', function () {
        var giftVoucherId = $(this).data('id');
        giftVoucherGeneratePrintItemList(giftVoucherId);
    });

    function compute(startData) {

        var first = parseInt($('#noOfcardText').val());
        var second = startData;
        var result = $('#endfromtext');
        result.val(first);
        $('#startfromtext').val(second);
        var end = (first + second) - 1;
        $('#endfromtext').val(end);
    }

    function compute2() {
        var first = ~~$('#noOfcardText').val();
        var second = ~~$('#startfromtext').val();
        var result = $('#endfromtext');
        var grade = first + second;
        result.val(grade);
    }

   
});