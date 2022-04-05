/**
 * Saving dotnet object reference for service
 */
export const RequirePopper = async function (address) {    
    let dependence = await Required(address);
    if (!window.Popper) {
        window.Popper = dependence;
        define('@popperjs/core', ["/" + address]);
    }
}

export const RequireBootstrap = async function (address) {
    let dependence = await Required(address);
    if (!window.bootstrap) window.bootstrap = dependence;
}

export const RequirePerfectScrollbar = async function (address) {
    let dependence = await Required(address);
    if (!window.PerfectScrollbar) window.PerfectScrollbar = dependence;
}

export const RequireSmoothScrollbar = async function (address) {
    let dependence = await Required(address);
    if (!window.Scrollbar) window.Scrollbar = dependence;
}

export const Required = async function (address, title) {
    return await new Promise(resolve => {
        require([address], function (obj)
        {
            if (title) {
                let mod = new Function('mod', 'window.' + title + ' = mod;');
                mod(obj);
            }
            resolve(obj);
        });
    });
}

export const RequireSweetAlerts = async function (address) {
    //import { Swal } from '/_content/Sufficit.Blazor.UI.Material/assets/js/plugins/sweetalert.min.js';
    let dependence = await Required(address);
    if (!window.Swal) window.Swal = dependence;

    return dependence;
}


// Shim for allowing async function creation via new Function
const AsyncFunction = Object.getPrototypeOf(async function () { }).constructor;

export const ExecAsync = async function (method, value) {
    return await new AsyncFunction('reference', method)(value);
}
