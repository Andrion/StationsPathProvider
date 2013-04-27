var map = null;
var mapObjects = null;
var allObjects = [];
var selectedObject = null;

var viewAllObjects = false;

var cityName = "Гомель";

var apiKey = "AMHTe1EBAAAAOEenHgQAz4QIRivO2iC7Wc4jbm8M698lUHUAAAAAAAAAAADaI3VteFcmN6TORqr7iEA95JNpxw==";

function init() {
    mapObjects = new ymaps.GeoObjectCollection();

    ymaps.geocode("Гомель", { results: 1 }).then(function (result) {
        var gomelObj = result.geoObjects.get(0);
        
        map = new ymaps.Map("map", {
            //center: [52.438581, 30.974897],
            center: gomelObj.geometry.getCoordinates(),
            zoom: 12,
            //type: "yandex#publicMap",
            type: "yandex#publicMapHybrid",
            behaviors: ["default", "scrollZoom"]
        });

        map.controls
            .add("zoomControl")
            //.add("typeSelector")
            .add("smallZoomControl", { right: 5, top: 75 })
            .add("mapTools");

        var btnAllObjects = new ymaps.control.Button("Все объекты");
        btnAllObjects.events
            .add("select", function () {
                viewAllObjects = true;
                changeMapObjects(true);
            }).add("deselect", function () {
                viewAllObjects = false;
                changeMapObjects(false);
            });

        map.controls.add(btnAllObjects, { right: 5, top: 5 });
    });
}
ymaps.ready(init);

$(function () {
    $.get("/stations/get", {}, function (data) {
        $("#tmplStation").tmpl(data).appendTo("#lstStations");
    });

    var lstStations = $("#lstStations");

    lstStations.on("click", "li", function() {
        var elem = $(this);

        lstStations.find("li")
            .data("active", false)
            .removeClass("active");

        elem.data("active", true);
        elem.addClass("active");

        var id = elem.data("id");
        var lat = elem.data("lat");
        var lng = elem.data("lng");
        
        var name = filterName(elem.text().trim());

        searchObjects(name, id, lat, lng);
    });

    var mapElem = $("#map");
    mapElem.css("position", "relative");

    mapElem.on("click", ".setStation", function () {
        var elem = $(this);

        var id = elem.data("id");
        var lat = elem.data("lat");
        var lng = elem.data("lng");

        setStation(id, lat, lng, function () {
            var li = lstStations.find("[data-id=" + id + "]");
            li.data("lat", lat);
            li.data("аlng", lng);
            var im = li.find("i");
            im.removeClass();
            im.addClass("icon-ok");
        });
    });
});

function filterName(name) {
    name = name.replace(/\s*\([-\s,._a-zA-Zа-яА-ЯёЁ]+\)$/, "");
    name = name.replace(/-[-_,.a-zA-Zа-яА-ЯёЁ]{1,}/, "");
    console.log("search: ", name);
    return name;
}

function searchObjects(name, id, lat, lng) {
    var req = {
        text: cityName + " " + name,
        results: 20,
        format: "json",
        key: apiKey
    };

    //ymaps.geocode(cityName + " " + name, { provider: "yandex#publicMap", kind: "stop" }).then(function (res) {
    //    console.log(res);
    //});
    //return;
    $.ajax({
        url: "http://psearch-maps.yandex.ru/1.x/",
        type: "GET",
        data: req,
        dataType: "jsonp",
        success: function (data) {
            allObjects = [];
            
            var results = data.response.GeoObjectCollection.featureMember;

            for (var i = 0; i < results.length; i++) {
                var obj = results[i].GeoObject;
                
                var prop = obj.metaDataProperty.PSearchObjectMetaData;
                var coords = obj.Point.pos.split(" ");

                var mapObj = setMarker(name, id, coords[1], coords[0], prop.kind, lat, lng);

                allObjects.push(mapObj);
            }

            changeMapObjects(viewAllObjects);
        }
    });
}

function setMarker(title, id, lat, lng, kind, sLat, sLng) {
    var preset = null;
    if (lat == sLat && lng == sLng) {
        preset = "twirl#greenIcon";
    } else {
        preset = kind == "stop" ? "twirl#blueIcon" : "twirl#yellowIcon";
    }

    var mapObj = new ymaps.Placemark([lat, lng], {
        balloonContentHeader: title,
        balloonContentBody: '<div class="text-center">' +
            '<a href="javascript:;" class="btn btn-primary setStation" data-id="' + id + '" data-lat="' + lat + '" data-lng="' + lng + '">SET</a></div>',
        balloonContentFooter: 'Lat: ' + lat + '<br />Lng: ' + lng,
        id: id
    }, {
        preset: preset,
        kind: kind,
        lat: lat,
        lng: lng
    });

    mapObj.events.add("click", function (e) {
        selectedObject = e.originalEvent.target;
    });

    return mapObj;
}

function setStation(id, lat, lng, callback) {
    var req = {
        id: id,
        lat: lat.toString().replace(".", ","),
        lng: lng.toString().replace(".", ",")
    };

    $.post("/stations/set", req, function (data) {
        if (data.success) {
            if (selectedObject != null) {
                selectedObject.options.set("preset", "twirl#greenIcon");
                selectedObject.balloon.close();
                selectedObject = null;
            }
            
            if (callback) {
                callback();
            }
        } else {
            alert("error");
        }
    }).error(function () {
        alert("error");
    });
}

function changeMapObjects(viewAll) {
    mapObjects.removeAll();
    map.geoObjects.remove(mapObjects);
    mapObjects = new ymaps.GeoObjectCollection();

    for (var i = 0; i < allObjects.length; i++) {
        var obj = allObjects[i];
        
        if (obj.options.get("kind") == "stop") {
            mapObjects.add(obj);
        }
        else if (viewAll) {
            mapObjects.add(obj);
        }
    };
    
    map.geoObjects.add(mapObjects);

    if (mapObjects.getLength() > 0) {
        map.setBounds(mapObjects.getBounds());
    }
}