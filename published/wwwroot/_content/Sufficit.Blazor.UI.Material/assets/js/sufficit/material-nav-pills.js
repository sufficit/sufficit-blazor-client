// Tabs navigation
export function NavMaterial(element) {
    let first_li = element.querySelector('li:first-child .nav-link');

    let tab = first_li.cloneNode();
    tab.innerHTML = "-";

    let moving_div = document.createElement('div');
    moving_div.classList.add('moving-tab', 'position-absolute', 'nav-link');    
    moving_div.style.padding = '0px';
    moving_div.style.width = element.querySelector('li:nth-child(1)').offsetWidth + 'px';
    moving_div.style.transform = 'translate3d(0px, 0px, 0px)';
    moving_div.style.transition = '.5s ease';
    moving_div.appendChild(tab);

    element.appendChild(moving_div);

    forEachUList(element);
    //element.addEventListener('mouseover', onTabMouseOver.bind(this, element));

    // Tabs navigation resize
    onWindowResize(element);
    window.addEventListener('resize', onWindowResize.bind(this, element));

    DropFlexRowsOnMobile(element);
}

// Function to remove flex row on mobile devices
function DropFlexRowsOnMobile(element) {
    if (window.innerWidth < 991) {
        if (element.classList.contains('flex-row')) {
            element.classList.remove('flex-row');
            element.classList.add('flex-column', 'on-resize');
        }
    }
}

function forEachUList(NavMaterialElement) {
    let ListItems = NavMaterialElement.querySelectorAll('li');
    ListItems.forEach(function (SubItem) {
        let NavLink = SubItem.querySelector('.nav-link');
        if (NavLink) {
            NavLink.addEventListener('click', onNavLinkClick.bind(this, NavMaterialElement, SubItem));
        }
    });
}

function onNavLinkClick(NavMaterialElement, ListItem) {
    let nodes = Array.from(ListItem.closest('ul').children); // get array
    let index = nodes.indexOf(ListItem) + 1;    

    let moving_div = NavMaterialElement.querySelector('.moving-tab');
    let sum = 0;
    if (NavMaterialElement.classList.contains('flex-column')) {
        for (var j = 1; j <= nodes.indexOf(ListItem); j++) {
            sum += NavMaterialElement.querySelector('li:nth-child(' + j + ')').offsetHeight;
        }
        moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';
        moving_div.style.height = NavMaterialElement.querySelector('li:nth-child(' + j + ')').offsetHeight;
    } else {
        for (var j = 1; j <= nodes.indexOf(ListItem); j++) {
            sum += NavMaterialElement.querySelector('li:nth-child(' + j + ')').offsetWidth;
        }
        moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
        moving_div.style.width = NavMaterialElement.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
    }
}

function onWindowResize(item) {
    item.querySelector('.moving-tab').remove();
    let moving_div = document.createElement('div');
    let tab = item.querySelector(".nav-link.active").cloneNode();
    tab.innerHTML = "-";

    moving_div.classList.add('moving-tab', 'position-absolute', 'nav-link');
    moving_div.appendChild(tab);

    item.appendChild(moving_div);

    moving_div.style.padding = '0px';
    moving_div.style.transition = '.5s ease';

    let li = item.querySelector(".nav-link.active").parentElement;

    if (li) {
        let nodes = Array.from(li.closest('ul').children); // get array
        let index = nodes.indexOf(li) + 1;

        let sum = 0;
        if (item.classList.contains('flex-column')) {
            for (var j = 1; j <= nodes.indexOf(li); j++) {
                sum += item.querySelector('li:nth-child(' + j + ')').offsetHeight;
            }
            moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';
            moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
            moving_div.style.height = item.querySelector('li:nth-child(' + j + ')').offsetHeight;
        } else {
            for (var j = 1; j <= nodes.indexOf(li); j++) {
                sum += item.querySelector('li:nth-child(' + j + ')').offsetWidth;
            }
            moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
            moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
        }
    }

    if (window.innerWidth < 991) {
        if (!item.classList.contains('flex-column')) {
            item.classList.remove('flex-row');
            item.classList.add('flex-column', 'on-resize');
            li = item.querySelector(".nav-link.active").parentElement;
            let nodes = Array.from(li.closest('ul').children); // get array
            let index = nodes.indexOf(li) + 1;
            let sum = 0;
            for (var j = 1; j <= nodes.indexOf(li); j++) {
                sum += item.querySelector('li:nth-child(' + j + ')').offsetHeight;
            }
            moving_div = document.querySelector('.moving-tab');
            moving_div.style.width = item.querySelector('li:nth-child(1)').offsetWidth + 'px';
            moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';
        }
    } else {
        if (item.classList.contains('on-resize')) {
            item.classList.remove('flex-column', 'on-resize');
            item.classList.add('flex-row');
            li = item.querySelector(".nav-link.active").parentElement;
            let nodes = Array.from(li.closest('ul').children); // get array
            let index = nodes.indexOf(li) + 1;
            let sum = 0;
            for (var j = 1; j <= nodes.indexOf(li); j++) {
                sum += item.querySelector('li:nth-child(' + j + ')').offsetWidth;
            }
            moving_div = document.querySelector('.moving-tab');
            moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
            moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
        }
    }
}



// Tabs navigation

var total = document.querySelectorAll('.nav-pills');
function initNavs() {
    total.forEach(function (item, i) {
        var moving_div = document.createElement('div');
        var first_li = item.querySelector('li:first-child .nav-link');
        var tab = first_li.cloneNode();
        tab.innerHTML = "-";

        moving_div.classList.add('moving-tab', 'position-absolute', 'nav-link');
        moving_div.appendChild(tab);
        item.appendChild(moving_div);

        var list_length = item.getElementsByTagName("li").length;

        moving_div.style.padding = '0px';
        moving_div.style.width = item.querySelector('li:nth-child(1)').offsetWidth + 'px';
        moving_div.style.transform = 'translate3d(0px, 0px, 0px)';
        moving_div.style.transition = '.5s ease';

        item.onmouseover = function (event) {
            let target = getEventTarget(event);
            let li = target.closest('li'); // get reference
            if (li) {
                let nodes = Array.from(li.closest('ul').children); // get array
                let index = nodes.indexOf(li) + 1;
                item.querySelector('li:nth-child(' + index + ') .nav-link').onclick = function () {
                    moving_div = item.querySelector('.moving-tab');
                    let sum = 0;
                    if (item.classList.contains('flex-column')) {
                        for (var j = 1; j <= nodes.indexOf(li); j++) {
                            sum += item.querySelector('li:nth-child(' + j + ')').offsetHeight;
                        }
                        moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';
                        moving_div.style.height = item.querySelector('li:nth-child(' + j + ')').offsetHeight;
                    } else {
                        for (var j = 1; j <= nodes.indexOf(li); j++) {
                            sum += item.querySelector('li:nth-child(' + j + ')').offsetWidth;
                        }
                        moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
                        moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
                    }
                }
            }
        }
    });
}

setTimeout(function () {
    initNavs();
}, 100);

// Tabs navigation resize
window.addEventListener('resize', function (event) {
    total.forEach(function (item, i) {
        item.querySelector('.moving-tab').remove();
        var moving_div = document.createElement('div');
        var tab = item.querySelector(".nav-link.active").cloneNode();
        tab.innerHTML = "-";

        moving_div.classList.add('moving-tab', 'position-absolute', 'nav-link');
        moving_div.appendChild(tab);

        item.appendChild(moving_div);

        moving_div.style.padding = '0px';
        moving_div.style.transition = '.5s ease';

        let li = item.querySelector(".nav-link.active").parentElement;

        if (li) {
            let nodes = Array.from(li.closest('ul').children); // get array
            let index = nodes.indexOf(li) + 1;

            let sum = 0;
            if (item.classList.contains('flex-column')) {
                for (var j = 1; j <= nodes.indexOf(li); j++) {
                    sum += item.querySelector('li:nth-child(' + j + ')').offsetHeight;
                }
                moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';
                moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
                moving_div.style.height = item.querySelector('li:nth-child(' + j + ')').offsetHeight;
            } else {
                for (var j = 1; j <= nodes.indexOf(li); j++) {
                    sum += item.querySelector('li:nth-child(' + j + ')').offsetWidth;
                }
                moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
                moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';

            }
        }
    });

    if (window.innerWidth < 991) {
        total.forEach(function (item, i) {
            if (!item.classList.contains('flex-column')) {
                item.classList.remove('flex-row');
                item.classList.add('flex-column', 'on-resize');
                let li = item.querySelector(".nav-link.active").parentElement;
                let nodes = Array.from(li.closest('ul').children); // get array
                let index = nodes.indexOf(li) + 1;
                let sum = 0;
                for (var j = 1; j <= nodes.indexOf(li); j++) {
                    sum += item.querySelector('li:nth-child(' + j + ')').offsetHeight;
                }
                var moving_div = document.querySelector('.moving-tab');
                moving_div.style.width = item.querySelector('li:nth-child(1)').offsetWidth + 'px';
                moving_div.style.transform = 'translate3d(0px,' + sum + 'px, 0px)';

            }
        });
    } else {
        total.forEach(function (item, i) {
            if (item.classList.contains('on-resize')) {
                item.classList.remove('flex-column', 'on-resize');
                item.classList.add('flex-row');
                let li = item.querySelector(".nav-link.active").parentElement;
                let nodes = Array.from(li.closest('ul').children); // get array
                let index = nodes.indexOf(li) + 1;
                let sum = 0;
                for (var j = 1; j <= nodes.indexOf(li); j++) {
                    sum += item.querySelector('li:nth-child(' + j + ')').offsetWidth;
                }
                var moving_div = document.querySelector('.moving-tab');
                moving_div.style.transform = 'translate3d(' + sum + 'px, 0px, 0px)';
                moving_div.style.width = item.querySelector('li:nth-child(' + index + ')').offsetWidth + 'px';
            }
        })
    }
});

//Function to remove flex row on mobile devices
if (window.innerWidth < 991) {
    total.forEach(function (item, i) {
        if (item.classList.contains('flex-row')) {
            item.classList.remove('flex-row');
            item.classList.add('flex-column', 'on-resize');
        }
    });
}

function getEventTarget(e) {
    e = e || window.event;
    return e.target || e.srcElement;
}

//End tabs navigation