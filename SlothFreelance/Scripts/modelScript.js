function ajaxFormValid() {
    var form = $("#modal_content form");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}
function modalShow() {
    $('.overlay').fadeIn(400,
        function () {
            $('.modaldiv')
                .css('display', 'block')
                .animate({ opacity: 1, top: '40%' }, 200);
        });
}
function modalHidden() {
    $('.modaldiv')
        .animate({ opacity: 0, top: '35%' }, 200,
            function () {
                $(this).css('display', 'none');
                $('.overlay').fadeOut(400);
            }
        );
}
function modalButtonDisable() {
    $('.modaldiv button[type = "submit"]').attr("disabled", "");
}
function modalHaveErrors(response) {
    console.log("error");
}
$('.overlay, .close').click(function () {
    modalHidden();
});