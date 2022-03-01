/**
 * Saving dotnet object reference for service
 * @param {any} dotNetObjectRef
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

export const Required = async function (address) {
    return await new Promise(resolve => {
        require([address], function (obj) { resolve(obj); });
    });
}