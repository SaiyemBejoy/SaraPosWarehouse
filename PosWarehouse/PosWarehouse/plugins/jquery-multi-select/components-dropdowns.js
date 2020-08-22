var ComponentsDropdowns = function () {


    var handleMultiSelect = function () {
        $('#my_multi_select1').multiSelect();
    }

    return {
       
        init: function () {            
            handleMultiSelect();
 
        }
    };

}();