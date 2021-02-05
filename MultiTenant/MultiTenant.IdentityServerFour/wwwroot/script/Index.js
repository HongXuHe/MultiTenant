$(document).ready(function () {
    $('#showHost').hide();
    $(".cancel").click(function () {
        location.href = "../"
    });
    $('#selectGrantTypes').change(function () {
        var value = $(this).val();
        if (value == '2' || value =='3') {
            $('#showHost').show();
        } else {
            $('#showHost').hide();
        }
    });
});