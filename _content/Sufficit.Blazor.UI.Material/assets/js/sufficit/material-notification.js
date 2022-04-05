
// Notification function
function notify(el) {
    var body = document.querySelector('body');
    var alert = document.createElement('div');
    alert.classList.add('alert', 'position-absolute', 'top-0', 'border-0', 'text-white', 'w-50', 'end-0', 'start-0', 'mt-2', 'mx-auto', 'py-2');
    alert.classList.add('alert-' + el.getAttribute('data-type'));
    alert.style.transform = 'translate3d(0px, 0px, 0px)'
    alert.style.opacity = '0';
    alert.style.transition = '.35s ease';
    alert.style.zIndex = '9999';
    setTimeout(function () {
        alert.style.transform = 'translate3d(0px, 20px, 0px)';
        alert.style.setProperty("opacity", "1", "important");
    }, 100);

    alert.innerHTML = '<div class="d-flex mb-1">' +
        '<div class="alert-icon me-1">' +
        '<i class="' + el.getAttribute('data-icon') + ' mt-1"></i>' +
        '</div>' +
        '<span class="alert-text"><strong>' + el.getAttribute('data-title') + '</strong></span>' +
        '</div>' +
        '<span class="text-sm">' + el.getAttribute('data-content') + '</span>';

    body.appendChild(alert);
    setTimeout(function () {
        alert.style.transform = 'translate3d(0px, 0px, 0px)'
        alert.style.setProperty("opacity", "0", "important");
    }, 4000);
    setTimeout(function () {
        el.parentElement.querySelector('.alert').remove();
    }, 4500);
}
