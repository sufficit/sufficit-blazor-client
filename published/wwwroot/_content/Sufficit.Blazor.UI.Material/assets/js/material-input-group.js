/**
 * Used for load events of an input or button
 * @param {any} element Element reference from Razor
 */
export function InputMaterial(element) {

    let classes = element.classList;
    if (classes !== undefined && classes != null && classes.contains('.input-group')) {
        setAttributes(element, {
            "onfocus": "focused(this)",
            "onfocusout": "defocused(this)"
        });
    }

    // Material Design Input function
    var inputs = element.querySelectorAll('input');

    for (let indexInput = 0; indexInput < inputs.length; indexInput++) {
        inputs[indexInput].addEventListener('focus', function (e) {
            this.parentElement.classList.add('is-focused');
        }, false);

        inputs[indexInput].onkeyup = function (e) {
            if (this.value != "") {
                this.parentElement.classList.add('is-filled');
            } else {
                this.parentElement.classList.remove('is-filled');
            }
        };

        inputs[indexInput].addEventListener('focusout', function (e) {
            if (this.value != "") {
                this.parentElement.classList.add('is-filled');
            }
            this.parentElement.classList.remove('is-focused');
        }, false);
    }

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

