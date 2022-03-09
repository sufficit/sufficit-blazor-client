/**
 * Used for load events of an input or button
 * @param {any} element Element reference from Razor
 */
export function InputMaterial(element) {

    // Ripple Effect
    var ripples = element.querySelectorAll('.btn');

    for (let indexBtn = 0; indexBtn < ripples.length; indexBtn++) {
        ripples[indexBtn].addEventListener('click', function (e) {
            var targetEl = e.target;
            var rippleDiv = targetEl.querySelector('.ripple');

            rippleDiv = document.createElement('span');
            rippleDiv.classList.add('ripple');
            rippleDiv.style.width = rippleDiv.style.height = Math.max(targetEl.offsetWidth, targetEl.offsetHeight) + 'px';
            targetEl.appendChild(rippleDiv);

            rippleDiv.style.left = (e.offsetX - rippleDiv.offsetWidth / 2) + 'px';
            rippleDiv.style.top = (e.offsetY - rippleDiv.offsetHeight / 2) + 'px';
            rippleDiv.classList.add('ripple');
            setTimeout(function () {
                rippleDiv.parentElement.removeChild(rippleDiv);
            }, 600);
        }, false);
    }
}

