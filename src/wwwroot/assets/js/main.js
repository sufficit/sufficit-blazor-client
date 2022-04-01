console.debug("SUFFICIT: Loading from RequireJs");
let rootPath = "";
requirejs.config({
    baseUrl: rootPath + '/assets/js',
    paths: {
        'sufficit/blazor-before': 'sufficit-blazor-before',
        'content/authentication': rootPath + '/_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService'
    },
    shim: {
        'content/authentication': { init: onAuthenticationLoaded },
        'sufficit/blazor-before': {
            deps: ['content/authentication'], init: () => LoadBlazorScript(onBlazorFrameworkLoaded)
        }
    }
});

require(['sufficit/blazor-before']);

function ToggleVisibility(element, visible) {
    if (visible) {
        if (element.classList.contains("d-none"))
            element.classList.remove("d-none");

        if (!element.classList.contains("d-block"))
            element.classList.add("d-block");
    } else {
        if (element.classList.contains("d-block"))
            element.classList.remove("d-block");

        if (!element.classList.contains("d-none"))
            element.classList.add("d-none");
    }
}

/** Carregando sistema de autenticação */
function onAuthenticationLoaded() {
    console.debug('SUFFICIT: Authentication system loaded');
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
    } else { await onload(); }
}

/** Carregando framework principal, Blazor */
function onBlazorFrameworkLoaded() {
    console.debug("SUFFICIT: Blazor Framework loaded");
    var i = 0;
    var allResourcesBeingLoaded = [];
    Blazor.start({
        loadBootResource: function (type, name, defaultUri, integrity) {
            if (type == "dotnetjs")
                return defaultUri;

            var f = fetch(defaultUri, {
                cache: 'no-cache',
                integrity: integrity
            });

            allResourcesBeingLoaded.push(f);
            f.then((r) => {
                i++;
                if (i > 1) {
                    let l = allResourcesBeingLoaded.length;
                    let percent = 100 * i / l;

                    if (percent < 100) {
                        // console.debug("percent: " + percent + " %");
                        let elProgressBar = document.getElementById("progressbar");
                        elProgressBar.style.width = percent + "%";
                    }

                    let elProgressStatus = document.getElementById("progressstatus");
                    elProgressStatus.innerHTML = parseInt(percent, 10);

                    let elProgressMsg = document.getElementById("progressmsg");
                    if (percent < 100) {
                        elProgressMsg.innerText = "Baixando novos arquivos para o funcionamento do aplicativo !";
                    } else {
                        elProgressMsg.innerText = "Pronto ! Já baixamos tudo o que era necessário. Inicializando ...";
                    }

                    let elProgress = document.getElementById("progress");
                    let elPSuccess = document.getElementById("pssuccess");

                    if (percent == 100) {
                        ToggleVisibility(elProgress, false);
                        ToggleVisibility(elPSuccess, true);
                    } else {
                        ToggleVisibility(elProgress, true);
                        ToggleVisibility(elPSuccess, false);
                    }
                }
            });
            return f;
        }
    });
}