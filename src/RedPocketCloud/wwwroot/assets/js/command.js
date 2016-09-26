var lock = false;
var cooldown = null;

$('.txtCommand').css('margin-left', -$('.txtCommand').width() / 2);
$('.txtCommand').css('font-size', 40 / 800 * $(window).height());
$('.txtCommand').css('line-height', 60 / 800 * $(window).height());
$('.txtCommand').css('height', 60 / 800 * $(window).height());

var font_size = 0.048 * $(window).height();
$('.drawn-text').css('font-size', font_size + "px");
$('.drawn-text').css('line-height', font_size + "px");
$('.drawn-text').css('margin-top', -(font_size / 2.0) + "px");

$('.pending').click(function () { Close(); });
$('.drawn').click(function () { Close(); });
$('.undrawn').click(function () { Close(); });

function KeyUp()
{
    $.post('/WeChat/DrawnCommand/' + Merchant, { Command: $('.txtCommand').val() }, function (data) {
        if (data == "AUTH")
            window.location.reload();
        else if (data == "NO") {
            ShowPending();
        } else if (data == "RETRY") {
        } else if (data == "EXCEEDED") {
            window.location = "/WeChat/Exceeded";
        } else {
            var obj = data;
            if (obj.type == 0)
                cooldown = new Date();
            if (obj.type != 1)
                ShowDrawn(obj.display);
            else
                ShowDrawn("点击打开", obj.display);
        }
    });
}

$('.txtCommand').keyup(function () {
    KeyUp();
});

function Close() {
    $('.pending').removeClass('showing');
    $('.drawn').removeClass('showing');
    $('.undrawn').removeClass('showing');
    $('.alpha').removeClass('active');
    $('.drawn-text').removeClass('active');
    lock = false;
}

function ShowPending() {
    $('.alpha').addClass('active');
    $('.pending').addClass('showing');
}

function ShowDrawn(txt, url) {
    $('.alpha').addClass('active');
    $('.drawn-text').addClass('active');
    $('.drawn').addClass('showing');
    $('.drawn-text').text(txt);
    if (!url)
        $('.drawn-text').attr('href', '#');
    else
        $('.drawn-text').attr('href', url);
}