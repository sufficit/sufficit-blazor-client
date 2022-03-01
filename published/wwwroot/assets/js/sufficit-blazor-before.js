console.debug('SUFFICIT: Loading blazor before');

function interceptConsoleEvents(dotNetObjectRef) {

    var exLog = console.debug;
    console.debug = function (msg) {
        exLog.apply(console, arguments);

        // Calls a method by name with the [JSInokable] attribute (above)
        dotNetObjectRef.invokeMethodAsync('onConsoleEvent', `dbug: ${msg}`)
    }

    var exLogInfo = console.log;
    console.log = function (msg) {
        exLogInfo.apply(console, arguments);

        // Calls a method by name with the [JSInokable] attribute (above)
        dotNetObjectRef.invokeMethodAsync('onConsoleEvent', `info: ${msg}`)
    }
    /*
    var exLogError = console.error;
    console.error = function (msg) {
        exLogError.apply(console, arguments);

        // Calls a method by name with the [JSInokable] attribute (above)
        dotNetObjectRef.invokeMethodAsync('onConsoleEvent', `error: ${msg}`)
    }
    */
    /*
    document.addEventListener('hello', (event) => {

        // Calls a method by name with the [JSInokable] attribute (above)
        dotNetObjectRef.invokeMethodAsync('onConsoleEvent')
    });
    */
}