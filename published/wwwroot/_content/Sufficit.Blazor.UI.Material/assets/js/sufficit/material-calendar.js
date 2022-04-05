
//
// Widget Calendar
//

if (document.querySelector('[data-toggle="widget-calendar"]')) {
    var calendarEl = document.querySelector('[data-toggle="widget-calendar"]');
    var today = new Date();
    var mYear = today.getFullYear();
    var weekday = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var mDay = weekday[today.getDay()];

    var m = today.getMonth();
    var d = today.getDate();
    document.getElementsByClassName('widget-calendar-year')[0].innerHTML = mYear;
    document.getElementsByClassName('widget-calendar-day')[0].innerHTML = mDay;

    var calendar = new FullCalendar.Calendar(calendarEl, {
        contentHeight: 'auto',
        initialView: 'dayGridMonth',
        selectable: true,
        initialDate: '2020-12-01',
        editable: true,
        headerToolbar: false,
        events: [{
            title: 'Call with Dave',
            start: '2020-11-18',
            end: '2020-11-18',
            className: 'bg-gradient-danger'
        },

        {
            title: 'Lunch meeting',
            start: '2020-11-21',
            end: '2020-11-22',
            className: 'bg-gradient-warning'
        },

        {
            title: 'All day conference',
            start: '2020-11-29',
            end: '2020-11-29',
            className: 'bg-gradient-success'
        },

        {
            title: 'Meeting with Mary',
            start: '2020-12-01',
            end: '2020-12-01',
            className: 'bg-gradient-info'
        },

        {
            title: 'Winter Hackaton',
            start: '2020-12-03',
            end: '2020-12-03',
            className: 'bg-gradient-danger'
        },

        {
            title: 'Digital event',
            start: '2020-12-07',
            end: '2020-12-09',
            className: 'bg-gradient-warning'
        },

        {
            title: 'Marketing event',
            start: '2020-12-10',
            end: '2020-12-10',
            className: 'bg-gradient-primary'
        },

        {
            title: 'Dinner with Family',
            start: '2020-12-19',
            end: '2020-12-19',
            className: 'bg-gradient-danger'
        },

        {
            title: 'Black Friday',
            start: '2020-12-23',
            end: '2020-12-23',
            className: 'bg-gradient-info'
        },

        {
            title: 'Cyber Week',
            start: '2020-12-02',
            end: '2020-12-02',
            className: 'bg-gradient-warning'
        },

        ]
    });
    calendar.render();
}