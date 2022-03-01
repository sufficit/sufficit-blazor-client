export function ToggleClass(elementSelector, cssClass, active) {
    let element = document.querySelector(elementSelector);
    if (typeof element !== 'undefined' && element) {

        let contains = element.classList.contains(cssClass);
        if (typeof active !== 'undefined') {
            if (active && !contains) {
                element.classList.add(cssClass);
            } else if (!active && contains) {
                element.classList.remove(cssClass);
            }
        } else {
            if (!contains) {
                element.classList.add(cssClass);
            } else {
                element.classList.remove(cssClass);
            }
        }        
    }
}
