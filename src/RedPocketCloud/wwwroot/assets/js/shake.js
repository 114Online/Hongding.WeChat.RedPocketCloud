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