
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

var video = document.getElementById('idVideo');
var canvas = document.getElementById('idCanvas');
var context = canvas.getContext('2d');
var bCameraFirstTime = true;
var currLong = Infinity;
var currLat = Infinity;


function doResize() {
    var cameraScreenSize = 1.0;
    var cookie = readCookie("CameraScreenSize");
    if (cookie != null) {
        cameraScreenSize = Number(cookie) / 100.0;
    }

    var w = Math.floor($('#idCameraWrapper').width());
    var ws = Math.floor($('#idCameraWrapper').width() * cameraScreenSize);
    $('#idVideo').width(ws);
    $('#idCanvas').width(ws);
    $('#idInfo').width(w);

}

function stateInit() {
    $("#idButOpenCamera").show();
    $("#idButTakePhoto").hide();
    $("#idButReportIt").hide();
    $('#idVideo').hide();
    $('#idCanvas').hide();
    $('#idInfo').hide();
    $('#idRspMsg').hide();
};

function stateCapturing() {
    $("#idButOpenCamera").hide();
    $("#idButTakePhoto").show();
    $("#idButReportIt").hide();
    $('#idVideo').show();
    $('#idCanvas').hide();
    $('#idInfo').hide();
    $('#idRspMsg').hide();
};

function stateImageCaptured() {
    $("#idButOpenCamera").show();
    $("#idButTakePhoto").hide();
    $("#idButReportIt").show();
    $('#idVideo').hide();
    $('#idCanvas').show();
    $('#idInfo').show();
};

function updateRspMsg(msg, color) {
    $('#idRspMsg').html(msg);
    $('#idRspMsg').css("color", color);
    $('#idRspMsg').show();
}

function doResponse(response) {
    if (response.status == 200) {
        updateRspMsg("Item reported successfully", "green");
    }
    else {
        updateRspMsg("Error: " + response.responseText, "red");
    }
}

function doReportIt() {
    var vJPGQuality = 0.75;
    var cookie = readCookie("JPGQuality");
    if (cookie != null) {
        vJPGQuality = Number(cookie);
    }
    var vThingid = 0;
    var cookie = readCookie("Thingid");
    if (cookie != null) {
        vThingid = Number(cookie);
    }

    var dataURL = canvas.toDataURL("image/jpeg", vJPGQuality);

    if (currLong === Infinity) {
        updateRspMsg("Geo location updating - please wait / retry", "red");
        return;
    }

    if ($('#idName').val() == "") {
        updateRspMsg("'Name' field is mandatory", "red");
        $('#idName').focus();
        return;
    }

    updateRspMsg("Processing - please wait", "green")

    $.ajax({
        type: "POST",
        url: "/ReportIt",
        data: JSON.stringify({
            thingid: parseInt(vThingid),
            image: dataURL,
            name: $('#idName').val(),
            text: $('#idText').val(),
            latitude: currLat,
            longitude: currLong
        }),
        dataType: "json",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json; charset=utf-8",
            "XSRF-TOKEN": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        complete: function (data) {
            doResponse(data);
        }
    });

}

function doTakePhoto() {
    video.pause();
    canvas.width = video.clientWidth;
    canvas.height = video.clientHeight;
    context.drawImage(video, 0, 0, video.clientWidth, video.clientHeight);
    getGeoLocation();
    stateImageCaptured()
}

function doCameraFirstTime() {
    var vCameraSelection = "default";
    var cookie = readCookie("CameraSelection");
    if (cookie != null) {
        vCameraSelection = cookie;
    }

    var media = {
        video: true,
        audio: false,
    };

    if (vCameraSelection != "default") {
        media.video = { facingMode: vCameraSelection };
    }

    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia(media)
            .then(function (stream) {
                video.srcObject = stream;
                video.play();
                stateCapturing();
                getGeoLocation();
                bCameraFirstTime = false;
                return true;
            })
            .catch(function (err) {
                alert("Camera failure: " + err.name);
                stateInit();
                return false;
            });
    }
    else {
        alert("Camera not supported");
        stateInit();
        return false;
    }
}

function doOpenCamera() {
    if (bCameraFirstTime) {
        doCameraFirstTime();
    }
    else {
        video.play();
        stateCapturing();
    }
}

function setGeoLocation(position) {
    currLat = Number(position.coords.latitude.toFixed(6));
    currLong = Number(position.coords.longitude.toFixed(6));
    $('#idGeoLocation').html(currLat + ", " + currLong);
}

function getGeoLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(setGeoLocation);
    } else {
        alert("Geolocation is not supported by this browser");
    }
}

function doIndexReady() {
    $("#idButTakePhoto").click(function () {
        doTakePhoto();
    });

    $("#idButReportIt").click(function () {
        doReportIt();
    });

    $("#idButOpenCamera").click(function () {
        doOpenCamera();
    });

    $(window).resize(function () {
        doResize();
    });

    doResize();
    stateInit();
    getGeoLocation();
}



