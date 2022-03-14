export const ResizeListener = async function (element) {
    window.addEventListener("resize", GetListener.bind(this, element));
    return GetResizeEventArgs();
}

const GetListener = async function (element) {
    let data = GetResizeEventArgs();
    element.invokeMethodAsync("BrowserResize", data);
}

const GetResizeEventArgs = function () {
    return {
        height: window.innerHeight,
        width: window.innerWidth
    };
}