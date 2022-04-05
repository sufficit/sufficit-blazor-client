var material = {
    initFullCalendar: function () {
        document.addEventListener('DOMContentLoaded', function () {
            var calendarEl = document.getElementById('fullCalendar');
            var today = new Date();
            var y = today.getFullYear();
            var m = today.getMonth();
            var d = today.getDate();
            var calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                selectable: true,
                headerToolbar: {
                    left: 'title',
                    center: 'dayGridMonth,timeGridWeek,timeGridDay',
                    right: 'prev,next today'
                },
                select: function (info) {
                    // on select we show the Sweet Alert modal with an input
                    Swal.fire({
                        title: 'Create an Event',
                        html: '<div class="form-group">' +
                            '<input class="form-control text-default" placeholder="Event Title" id="input-field">' +
                            '</div>',
                        showCancelButton: true,
                        customClass: {
                            confirmButton: 'btn btn-primary',
                            cancelButton: 'btn btn-danger'
                        },
                        buttonsStyling: false
                    }).then(function (result) {
                        var eventData;
                        var event_title = document.getElementById('input-field').value;
                        if (event_title) {
                            eventData = {
                                title: event_title,
                                start: info.startStr,
                                end: info.endStr
                            };
                            calendar.addEvent(eventData);
                        }
                    });
                },
                editable: true,
                // color classes: [ event-blue | event-azure | event-green | event-orange | event-red ]
                events: [{
                    title: 'All Day Event',
                    start: new Date(y, m, 1),
                    className: 'event-default'
                },
                {
                    id: 999,
                    title: 'Repeating Event',
                    start: new Date(y, m, d - 4, 6, 0),
                    allDay: false,
                    className: 'event-rose'
                },
                {
                    id: 999,
                    title: 'Repeating Event',
                    start: new Date(y, m, d + 3, 6, 0),
                    allDay: false,
                    className: 'event-rose'
                },
                {
                    title: 'Meeting',
                    start: new Date(y, m, d - 1, 10, 30),
                    allDay: false,
                    className: 'event-green'
                },
                {
                    title: 'Lunch',
                    start: new Date(y, m, d + 7, 12, 0),
                    end: new Date(y, m, d + 7, 14, 0),
                    allDay: false,
                    className: 'event-red'
                },
                {
                    title: 'Md-pro Launch',
                    start: new Date(y, m, d - 2, 12, 0),
                    allDay: true,
                    className: 'event-azure'
                },
                {
                    title: 'Birthday Party',
                    start: new Date(y, m, d + 1, 19, 0),
                    end: new Date(y, m, d + 1, 22, 30),
                    allDay: false,
                    className: 'event-azure'
                },
                {
                    title: 'Click for Creative Tim',
                    start: new Date(y, m, 21),
                    end: new Date(y, m, 22),
                    url: 'http://www.creative-tim.com/',
                    className: 'event-orange'
                },
                {
                    title: 'Click for Google',
                    start: new Date(y, m, 23),
                    end: new Date(y, m, 23),
                    url: 'http://www.creative-tim.com/',
                    className: 'event-orange'
                }
                ]
            });
            calendar.render();
        });
    },
    datatableSimple: function () {
        var columnDefs = [{
            field: 'athlete',
            minWidth: 150,
            sortable: true,
            filter: true
        },
        {
            field: 'age',
            maxWidth: 90,
            sortable: true,
            filter: true
        },
        {
            field: 'country',
            minWidth: 150,
            sortable: true,
            filter: true
        },
        {
            field: 'year',
            maxWidth: 90,
            sortable: true,
            filter: true
        },
        {
            field: 'date',
            minWidth: 150,
            sortable: true,
            filter: true
        },
        {
            field: 'sport',
            minWidth: 150,
            sortable: true,
            filter: true
        },
        {
            field: 'gold'
        },
        {
            field: 'silver'
        },
        {
            field: 'bronze'
        },
        {
            field: 'total'
        },
        ];

        // specify the data
        var rowData = [{
            "athlete": "Ronald Valencia",
            "age": 23,
            "country": "United States",
            "year": 2008,
            "date": "24/08/2008",
            "sport": "Swimming",
            "gold": 8,
            "silver": 0,
            "bronze": 0,
            "total": 8
        },
        {
            "athlete": "Lorand Frentz",
            "age": 19,
            "country": "United States",
            "year": 2004,
            "date": "29/08/2004",
            "sport": "Swimming",
            "gold": 6,
            "silver": 0,
            "bronze": 2,
            "total": 8
        },
        {
            "athlete": "Michael Phelps",
            "age": 27,
            "country": "United States",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 4,
            "silver": 2,
            "bronze": 0,
            "total": 6
        },
        {
            "athlete": "Natalie Coughlin",
            "age": 25,
            "country": "United States",
            "year": 2008,
            "date": "24/08/2008",
            "sport": "Swimming",
            "gold": 1,
            "silver": 2,
            "bronze": 3,
            "total": 6
        },
        {
            "athlete": "Aleksey Nemov",
            "age": 24,
            "country": "Russia",
            "year": 2000,
            "date": "01/10/2000",
            "sport": "Gymnastics",
            "gold": 2,
            "silver": 1,
            "bronze": 3,
            "total": 6
        },
        {
            "athlete": "Alicia Coutts",
            "age": 24,
            "country": "Australia",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 1,
            "silver": 3,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Missy Franklin",
            "age": 17,
            "country": "United States",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 4,
            "silver": 0,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Ryan Lochte",
            "age": 27,
            "country": "United States",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 2,
            "silver": 2,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Allison Schmitt",
            "age": 22,
            "country": "United States",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 3,
            "silver": 1,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Natalie Coughlin",
            "age": 21,
            "country": "United States",
            "year": 2004,
            "date": "29/08/2004",
            "sport": "Swimming",
            "gold": 2,
            "silver": 2,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Ian Thorpe",
            "age": 17,
            "country": "Australia",
            "year": 2000,
            "date": "01/10/2000",
            "sport": "Swimming",
            "gold": 3,
            "silver": 2,
            "bronze": 0,
            "total": 5
        },
        {
            "athlete": "Dara Torres",
            "age": 33,
            "country": "United States",
            "year": 2000,
            "date": "01/10/2000",
            "sport": "Swimming",
            "gold": 2,
            "silver": 0,
            "bronze": 3,
            "total": 5
        },
        {
            "athlete": "Cindy Klassen",
            "age": 26,
            "country": "Canada",
            "year": 2006,
            "date": "26/02/2006",
            "sport": "Speed Skating",
            "gold": 1,
            "silver": 2,
            "bronze": 2,
            "total": 5
        },
        {
            "athlete": "Nastia Liukin",
            "age": 18,
            "country": "United States",
            "year": 2008,
            "date": "24/08/2008",
            "sport": "Gymnastics",
            "gold": 1,
            "silver": 3,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Marit Bjørgen",
            "age": 29,
            "country": "Norway",
            "year": 2010,
            "date": "28/02/2010",
            "sport": "Cross Country Skiing",
            "gold": 3,
            "silver": 1,
            "bronze": 1,
            "total": 5
        },
        {
            "athlete": "Sun Yang",
            "age": 20,
            "country": "China",
            "year": 2012,
            "date": "12/08/2012",
            "sport": "Swimming",
            "gold": 2,
            "silver": 1,
            "bronze": 1,
            "total": 4
        }
        ];

        // let the grid know which columns and what data to use
        var gridOptions = {
            columnDefs: columnDefs,
            rowSelection: 'multiple',
            rowMultiSelectWithClick: true,
            rowData: rowData
        };

        // setup the grid after the page has finished loading
        document.addEventListener('DOMContentLoaded', function () {
            var gridDiv = document.querySelector('#datatableSimple');
            new agGrid.Grid(gridDiv, gridOptions);
        });
    },
    initVectorMap: function () {
        am4core.ready(function () {

            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end

            // Create map instance
            var chart = am4core.create("chartdiv", am4maps.MapChart);

            // Set map definition
            chart.geodata = am4geodata_worldLow;

            // Set projection
            chart.projection = new am4maps.projections.Miller();

            // Create map polygon series
            var polygonSeries = chart.series.push(new am4maps.MapPolygonSeries());

            // Exclude Antartica
            polygonSeries.exclude = ["AQ"];

            // Make map load polygon (like country names) data from GeoJSON
            polygonSeries.useGeodata = true;

            // Configure series
            var polygonTemplate = polygonSeries.mapPolygons.template;
            polygonTemplate.tooltipText = "{name}";
            polygonTemplate.polygon.fillOpacity = 0.6;


            // Create hover state and set alternative fill color
            var hs = polygonTemplate.states.create("hover");
            hs.properties.fill = chart.colors.getIndex(0);

            // Add image series
            var imageSeries = chart.series.push(new am4maps.MapImageSeries());
            imageSeries.mapImages.template.propertyFields.longitude = "longitude";
            imageSeries.mapImages.template.propertyFields.latitude = "latitude";
            imageSeries.mapImages.template.tooltipText = "{title}";
            imageSeries.mapImages.template.propertyFields.url = "url";

            var circle = imageSeries.mapImages.template.createChild(am4core.Circle);
            circle.radius = 3;
            circle.propertyFields.fill = "color";

            var circle2 = imageSeries.mapImages.template.createChild(am4core.Circle);
            circle2.radius = 3;
            circle2.propertyFields.fill = "color";


            circle2.events.on("inited", function (event) {
                animateBullet(event.target);
            })


            function animateBullet(circle) {
                var animation = circle.animate([{
                    property: "scale",
                    from: 1,
                    to: 5
                }, {
                    property: "opacity",
                    from: 1,
                    to: 0
                }], 1000, am4core.ease.circleOut);
                animation.events.on("animationended", function (event) {
                    animateBullet(event.target.object);
                })
            }

            var colorSet = new am4core.ColorSet();

            imageSeries.data = [{
                "title": "Brussels",
                "latitude": 50.8371,
                "longitude": 4.3676,
                "color": colorSet.next()
            }, {
                "title": "Copenhagen",
                "latitude": 55.6763,
                "longitude": 12.5681,
                "color": colorSet.next()
            }, {
                "title": "Paris",
                "latitude": 48.8567,
                "longitude": 2.3510,
                "color": colorSet.next()
            }, {
                "title": "Reykjavik",
                "latitude": 64.1353,
                "longitude": -21.8952,
                "color": colorSet.next()
            }, {
                "title": "Moscow",
                "latitude": 55.7558,
                "longitude": 37.6176,
                "color": colorSet.next()
            }, {
                "title": "Madrid",
                "latitude": 40.4167,
                "longitude": -3.7033,
                "color": colorSet.next()
            }, {
                "title": "London",
                "latitude": 51.5002,
                "longitude": -0.1262,
                "url": "http://www.google.co.uk",
                "color": colorSet.next()
            }, {
                "title": "Peking",
                "latitude": 39.9056,
                "longitude": 116.3958,
                "color": colorSet.next()
            }, {
                "title": "New Delhi",
                "latitude": 28.6353,
                "longitude": 77.2250,
                "color": colorSet.next()
            }, {
                "title": "Tokyo",
                "latitude": 35.6785,
                "longitude": 139.6823,
                "url": "http://www.google.co.jp",
                "color": colorSet.next()
            }, {
                "title": "Ankara",
                "latitude": 39.9439,
                "longitude": 32.8560,
                "color": colorSet.next()
            }, {
                "title": "Buenos Aires",
                "latitude": -34.6118,
                "longitude": -58.4173,
                "color": colorSet.next()
            }, {
                "title": "Brasilia",
                "latitude": -15.7801,
                "longitude": -47.9292,
                "color": colorSet.next()
            }, {
                "title": "Ottawa",
                "latitude": 45.4235,
                "longitude": -75.6979,
                "color": colorSet.next()
            }, {
                "title": "Washington",
                "latitude": 38.8921,
                "longitude": -77.0241,
                "color": colorSet.next()
            }, {
                "title": "Kinshasa",
                "latitude": -4.3369,
                "longitude": 15.3271,
                "color": colorSet.next()
            }, {
                "title": "Cairo",
                "latitude": 30.0571,
                "longitude": 31.2272,
                "color": colorSet.next()
            }, {
                "title": "Pretoria",
                "latitude": -25.7463,
                "longitude": 28.1876,
                "color": colorSet.next()
            }];
        }); // end am4core.ready()
    },

    // Sweet Alerts
    showSwal: function (type) {
        if (type == 'basic') {
            const swalBasic = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-info'
                }
            });
            swalBasic.fire({
                title: 'Sweet!'
            })

        } else if (type == 'title-and-text') {
            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                }
            });
            swalWithBootstrapButtons.fire({
                title: 'Sweet!',
                text: 'Modal with a custom image.',
                imageUrl: 'https://unsplash.it/400/200',
                imageWidth: 400,
                imageAlt: 'Custom image',
            })

        } else if (type == 'success-message') {

            Swal.fire(
                'Good job!',
                'You clicked the button!',
                'success'
            )

        } else if (type == 'warning-message-and-confirmation') {
            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                },
                buttonsStyling: false
            })

            swalWithBootstrapButtons.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'No, cancel!',
                reverseButtons: true
            }).then((result) => {
                console.debug('result: ', result);
                if (result.value) {
                    swalWithBootstrapButtons.fire(
                        'Deleted!',
                        'Your file has been deleted.',
                        'success'
                    )
                } else if (
                    /* Read more about handling dismissals below */
                    result.dismiss === Swal.DismissReason.cancel
                ) {
                    swalWithBootstrapButtons.fire(
                        'Cancelled',
                        'Your imaginary file is safe :)',
                        'error'
                    )
                }
            })
        } else if (type == 'warning-message-and-cancel') {
            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                },
                buttonsStyling: false
            })
            swalWithBootstrapButtons.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                console.debug('result: ', result);
                if (result.isConfirmed) {
                    Swal.fire(
                        'Deleted!',
                        'Your file has been deleted.',
                        'success'
                    )
                }
            })
        } else if (type == 'custom-html') {
            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                },
                buttonsStyling: false
            })
            swalWithBootstrapButtons.fire({
                title: '<strong>HTML <u>example</u></strong>',
                icon: 'info',
                html: 'You can use <b>bold text</b>, ' +
                    '<a href="//sweetalert2.github.io">links</a> ' +
                    'and other HTML tags',
                showCloseButton: true,
                showCancelButton: true,
                focusConfirm: false,
                confirmButtonText: '<i class="fa fa-thumbs-up"></i> Great!',
                confirmButtonAriaLabel: 'Thumbs up, great!',
                cancelButtonText: '<i class="fa fa-thumbs-down"></i>',
                cancelButtonAriaLabel: 'Thumbs down'
            })
        } else if (type == 'rtl-language') {
            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                },
                buttonsStyling: false
            })
            swalWithBootstrapButtons.fire({
                title: 'هل تريد الاستمرار؟',
                icon: 'question',
                iconHtml: '؟',
                confirmButtonText: 'نعم',
                cancelButtonText: 'لا',
                showCancelButton: true,
                showCloseButton: true
            })
        } else if (type == 'auto-close') {
            let timerInterval
            Swal.fire({
                title: 'Auto close alert!',
                html: 'I will close in <b></b> milliseconds.',
                timer: 2000,
                timerProgressBar: true,
                didOpen: () => {
                    Swal.showLoading()
                    timerInterval = setInterval(() => {
                        const content = Swal.getHtmlContainer()
                        if (content) {
                            const b = content.querySelector('b')
                            if (b) {
                                b.textContent = Swal.getTimerLeft()
                            }
                        }
                    }, 100)
                },
                willClose: () => {
                    clearInterval(timerInterval)
                }
            }).then((result) => {
                console.debug('result: ', result);
                /* Read more about handling dismissals below */
                if (result.dismiss === Swal.DismissReason.timer) { }
            })

        } else if (type == 'input-field') {

            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: 'btn bg-gradient-success',
                    cancelButton: 'btn bg-gradient-danger'
                },
                buttonsStyling: false
            })
            swalWithBootstrapButtons.fire({
                title: 'Submit your Github username',
                input: 'text',
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                confirmButtonText: 'Look up',
                showLoaderOnConfirm: true,
                preConfirm: (login) => {
                    return fetch(`//api.github.com/users/${login}`)
                        .then(response => {
                            if (!response.ok) {
                                throw new Error(response.statusText)
                            }
                            return response.json()
                        })
                        .catch(error => {
                            Swal.showValidationMessage(
                                `Request failed: ${error}`
                            )
                        })
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: `${result.value.login}'s avatar`,
                        imageUrl: result.value.avatar_url
                    })
                }
            })
        }
    }
}