$('.avafile').change(function () {
    if ($(this).val() != '') {
        $('#myAvaForm').submit();
    }
});