var lock = false;
var cooldown = null;
var limiting = 1;

$(document).ready(function () {
    var size = $(window).width() * 0.3125;
    $('.button').css('margin-left', -(size / 2) + 'px');
    $('.button').css('width', size + 'px');
    $('.button').css('height', size + 'px');
    $('.circle').css('width', (size * 0.95) + 'px');
    $('.circle').css('height', (size * 0.95) + 'px');
    $('.circle').css('margin-left', -((size * 0.95) / 2) + 'px');
    $('.circle').css('margin-top', ((size * 0.05) / 2) + 'px');

    $('.pending').click(function () { Close(); });
    $('.drawn').click(function () { Close(); });
    $('.undrawn').click(function () { Close(); });

    var font_size = 0.048 * $(window).height();
    $('.drawn-text').css('font-size', font_size + "px");
    $('.drawn-text').css('line-height', font_size + "px");
    $('.drawn-text').css('margin-top', -(font_size / 2.0) + "px");

    $('.button').click(function () { Shoop(); });
});

function Shoop()
{
    if (lock)
        return;
    lock = true;

    $('#shoopSound')[0].play();
    $($('.circle')[0]).addClass('transition');
    $($('.circle')[0]).addClass('shooping');
    setTimeout(function () {
        $($('.circle')[0]).removeClass('transition');
        $($('.circle')[0]).removeClass('shooping');
        $($('.circle')[1]).addClass('transition');
        $($('.circle')[1]).addClass('shooping');
        setTimeout(function () {
            $($('.circle')[1]).removeClass('transition');
            $($('.circle')[1]).removeClass('shooping');
            $($('.circle')[2]).addClass('transition');
            $($('.circle')[2]).addClass('shooping');
            setTimeout(function () {
                $($('.circle')[2]).removeClass('transition');
                $($('.circle')[2]).removeClass('shooping');
                if (cooldown && (((new Date()).getTime() - cooldown.getTime()) / 1000.0) <= 15) {
                    ShowUndrawn();
                    return;
                }
                if (Math.random() >= limiting) {
                    ShowUndrawn();
                    return;
                }
                ShowLoading();
                $.post('/WeChat/Drawn/' + Merchant, {}, function (data) {
                    HideLoading();
                    if (data == "AUTH")
                        window.location.reload();
                    else if (data == "NO") {
                        ShowPending();
                    } else if (data == "RETRY") {
                        ShowUndrawn();
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
            }, 600);
        }, 600);
    }, 600);
}

function Close()
{
    $('.pending').removeClass('showing');
    $('.drawn').removeClass('showing');
    $('.undrawn').removeClass('showing');
    $('.alpha').removeClass('active');
    $('.drawn-text').removeClass('active');
    lock = false;
}

function ShowPending()
{
    $('.alpha').addClass('active');
    $('.pending').addClass('showing');
}

function ShowUndrawn() {
    $('.alpha').addClass('active');
    $('.undrawn').addClass('showing');
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

function ShowLoading() {
    $('.loading').show();
}

function HideLoading() {
    $('.loading').hide();
}