var lock = false;

$(document).ready(function () {
    if (window.DeviceMotionEvent) {
        window.addEventListener('devicemotion', deviceMotionHandler, false);
    } else {
        alert('您好，你目前所用的设备好像不支持重力感应哦！');
    }
    var shakeThreshold = 3000;
    var lastUpdate = 0;
    var curShakeX = curShakeY = curShakeZ = lastShakeX = lastShakeY = lastShakeZ = 0;
    function deviceMotionHandler(event) {
        var acceleration = event.accelerationIncludingGravity;
        var curTime = new Date().getTime();
        if ((curTime - lastUpdate) > 100) {
            var diffTime = curTime - lastUpdate;
            lastUpdate = curTime;
            curShakeX = acceleration.x;
            curShakeY = acceleration.y;
            curShakeZ = acceleration.z;
            var speed = Math.abs(curShakeX + curShakeY + curShakeZ - lastShakeX - lastShakeY - lastShakeZ) / diffTime * 10000;
            if (speed > shakeThreshold) {
                Shake();
            }
            lastShakeX = curShakeX;
            lastShakeY = curShakeY;
            lastShakeZ = curShakeZ;
        }
    }

    $('.pending').click(function () { Close(); });
    $('.drawn').click(function () { Close(); });
    $('.undrawn').click(function () { Close(); });

    var font_size = 0.048 * $(window).height();
    $('.drawn-text').css('font-size', font_size + "px");
    $('.drawn-text').css('line-height', font_size + "px");
    $('.drawn-text').css('margin-top', -(font_size / 2.0) + "px");
});

function Shake()
{
    if (lock)
        return;
    lock = true;
    $('#shakeSound')[0].play();
    $('.top').addClass('shaking');
    $('.bottom').addClass('shaking');
    setTimeout(function () {
        $('.top').removeClass('shaking');
        $('.bottom').removeClass('shaking');
        setTimeout(function () {
            // TODO: Send request


            setTimeout(function () {
                lock = false;
            });
        }, 610);
    }, 610);
}

function Close()
{
    $('.pending').removeClass('showing');
    $('.drawn').removeClass('showing');
    $('.undrawn').removeClass('showing');
    $('.alpha').removeClass('active');
    $('.drawn-text').removeClass('active');
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
}