﻿var lock = false;
var cooldown = null;
var limiting = 1;

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

function Shake() {
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
            if ((cooldown && (((new Date()).getTime() - cooldown.getTime()) / 1000.0) <= 15) || ($.cookie('x-LastDrawn') && parseInt((new Date).getTime() - $.cookie('x-LastDrawn') < 15000))) {
                ShowUndrawn();
                return;
            }
            if (Math.random() >= limiting) {
                ShowUndrawn();
                return;
            }
            ShowLoading();
            $.ajax({
                url: '/WeChat/Drawn/' + Merchant, data: {}, timeout: 10000, dataType: 'text', type: 'POST',
                success: function (data) {
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
                        var obj = JSON.parse(data);
                        console.log(obj);
                        if (obj.type == 0)
                            cooldown = new Date();
                        if (obj.type != 1)
                            ShowDrawn(obj.display);
                        else
                            ShowDrawn("点击打开", obj.display);
                    }
                },
                error: function () {
                    HideLoading();
                    ShowUndrawn();
                }
            });
        }, 610);
    }, 610);
}

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