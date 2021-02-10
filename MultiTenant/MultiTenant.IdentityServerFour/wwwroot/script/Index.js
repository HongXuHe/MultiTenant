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
    $.each($('#navbarNav').find('li'), function () {
        $(this).toggleClass('active',
            window.location.pathname.indexOf($(this).find('a').attr('href')) > -1);
    }); 
});