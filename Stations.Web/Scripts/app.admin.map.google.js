$(document).ready(function () {
    $.get("/stations/get", {}, function (data) {
        $("#tmplStation").tmpl(data).appendTo("#lstStations");
    });

    var lstStations = $("#lstStations");
    

    var origin = new google.maps.LatLng(52.438581, 30.974897);

    var map = new google.maps.Map(document.getElementById("map"), {
        mapTypeId: google.maps.MapTypeId.HYBRID,
        //maxZoom: 16,
        zoom: 8
    });

    map.setCenter(origin);

    var infowindow = new google.maps.InfoWindow();

    var service = new google.maps.places.PlacesService(map);

    var bounds = new google.maps.LatLngBounds();


    var needTypes = ["bus_station", "transit_station", "subway_station"];

    var markers = [];

    function clearMap() {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers = [];
        bounds = new google.maps.LatLngBounds();
    }


    lstStations.on("click", "li", function () {
        var elem = $(this);

        lstStations.find("li")
            .data("active", false)
            .removeClass("active");

        elem.data("active", true);
        elem.addClass("active");

        var name = elem.text().trim();
        name = name.replace(/\s*\([-\s,._a-zA-Zа-яА-ЯёЁ]+\)$/, "");
        name = name.replace(/-[-_,.a-zA-Zа-яА-ЯёЁ]{1,}/, "");

        function callback(results) {
            for (var i = 0; i < results.length; i++) {
                var result = results[i];
                console.log(result);
                
                bounds.extend(result.pos);

                var marker = new google.maps.Marker({
                    position: result.pos,
                    title: result.name,
                    html: result.name,
                    map: map
                });

                var content = '<strong style="font-size:1.2em">' + result.name + '</strong>' +
                    '<br/><strong>' + result.address + '</strong>' +
                    '<br/><strong>Latitude: </strong>' + result.pos.jb +
                    '<br/><strong>Longitude: </strong>' + result.pos.kb +
                    '<br/><strong>Type: </strong>' + result.types.join(", ");

                google.maps.event.addListener(marker, "click", function () {
                    infowindow.setContent(content);
                    infowindow.open(map, this);
                });

                markers.push(marker);
            }

            map.setCenter(bounds.getCenter());
            map.fitBounds(bounds);
        }

        clearMap();
        searchPlaces(name, callback);
        searchGeoCodes("Гомель " + name, callback);
    });

    var mapElem = $("#map");

    $(window).scroll(function () {
        if ($(window).scrollTop() > mapElem.offset().top
            || $(window).scrollTop() < mapElem.offset().top + mapElem.height()) {
            mapElem.css("top", $(window).scrollTop());
        }
    });
    
    function searchPlaces(name, callback) {
        var request = {
            location: origin,
            radius: 50000,
            types: needTypes,
            //name: name
            query: name
        };

        //service.search(request, callback);
        service.textSearch(request, searchCallback);

        var callbackResults = [];

        function searchCallback(results, status) {
            if (status == google.maps.places.PlacesServiceStatus.OK) {
                for (var i = 0; i < results.length; i++) {
                    var result = results[i];

                    var pos = result.geometry.location;

                    callbackResults.push({
                        name: result.name,
                        address: result.formatted_address,
                        pos: pos,
                        types: result.types
                    });
                }
            }
            
            if (callback) {
                callback(callbackResults);
            }
        }
    }

    function searchGeoCodes(address, callback) {
        var requestData = {
            address: address,
            language: "ru",
            sensor: false
        };

        var callbackResults = [];

        $.getJSON("http://maps.googleapis.com/maps/api/geocode/json", requestData, function (data) {
            if (data.status == "OK") {
                for (var i = 0; i < data.results.length; i++) {
                    var result = data.results[i];

                    var pos = new google.maps.LatLng(result.geometry.location.lat, result.geometry.location.lng);

                    callbackResults.push({
                        name: result.formatted_address,
                        address: result.formatted_address,
                        pos: pos,
                        types: result.types
                    });
                }
            }
            
            if (callback) {
                callback(callbackResults);
            }
        });
    }
});