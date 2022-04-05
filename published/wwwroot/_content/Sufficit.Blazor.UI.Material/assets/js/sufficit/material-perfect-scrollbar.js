export const LoadPerfectScrollbar = function () {
    let isWindows = navigator.platform.indexOf('Win') > -1 ? true : false;

    if (isWindows) {
        // if we are on windows OS we activate the perfectScrollbar function
        if (document.getElementsByClassName('main-content')[0]) {
            let mainpanel = document.querySelector('.main-content');
            var ps = new PerfectScrollbar(mainpanel);
        };

        if (document.getElementsByClassName('sidenav')[0]) {
            let sidebar = document.querySelector('.sidenav');
            var ps1 = new PerfectScrollbar(sidebar);
        };
    
        if (document.getElementsByClassName('navbar-collapse')[0]) {
            let fixedplugin = document.querySelector('.navbar-collapse');
            var ps2 = new PerfectScrollbar(fixedplugin);
        };
    
        if (document.getElementsByClassName('fixed-plugin')[0]) {
            let fixedplugin = document.querySelector('.fixed-plugin');
            var ps3 = new PerfectScrollbar(fixedplugin);
        };
    };
}