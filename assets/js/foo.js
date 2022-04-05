define({ load: LoadBlazorReq });

async function LoadBlazorReq(name, req, onload, config) {
    let deps = config.shim.framework.deps
    req([deps], function () {
        LoadBlazorScript(config.shim.framework.init);
        req(name);
    });
    return true;
}

let scriptId = 'sufficitBlazorLoader';
let scriptUrl = '/_framework/blazor.webassembly.js';

async function LoadBlazorScript(onload) {
    var node = document.getElementById(scriptId);
    if (!node) {
        node = document.createElement('script');
        node.id = scriptId;
        node.type = 'text/javascript';
        node.charset = 'utf-8';
        node.async = true;
        node.setAttribute('src', scriptUrl);
        node.setAttribute('autostart', 'false');
        node.onload = onload;
        document.head.appendChild(node);
    } else { onload(); }
}