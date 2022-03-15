const className = 'g-sidenav-pinned';
const sidenavSelector = '#sidenav-main';

export function ToggleSidenav() {
    let sidenav = document.querySelector(sidenavSelector);
    if (typeof sidenav !== 'undefined' && sidenav) {
        let body = document.querySelector('body');
        if (typeof body !== 'undefined' && body) {
            if (body.classList.contains(className)) {
                body.classList.remove(className);
                setTimeout(function () {
                    sidenav.classList.remove('bg-white');
                }, 100);
                sidenav.classList.remove('bg-transparent');

            } else {
                body.classList.add(className);
                sidenav.classList.remove('bg-transparent');

                let iconSidenav = document.getElementById('iconSidenav');
                if (iconSidenav)
                    iconSidenav.classList.remove('d-none');
            }
        }
    }
}