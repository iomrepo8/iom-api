$(document).ready(function () {
    "use strict";

    $('#resetPassword').off().on('click', function () {

        var result = {}, token = $.fn.GetUrlParams('token');

        if ($('#resetPasswordForm').ValidateForm()) {
            
            var data = {
                Token: token,
                Email: $('#Email').val(),
                Password: $('#Password').val()
            };

            $.ajax({
                type: "POST",
                url: `../user/reset_pass`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(data),
                success: function (response) {
                    result = response;
                },
                complete: function () {
                    $.displayResponseMessage(result);
                    if (!result.isSuccessful) {

                        return false;
                    }

                    $(".modal.sm").modal('hide');
                    localStorage.clear();
                    $('#respass_form').remove();
                    $('#success_reset').show();
                }
            });
        }

        
    });

});