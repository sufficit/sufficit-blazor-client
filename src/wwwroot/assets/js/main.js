﻿console.debug("SUFFICIT: Loading from RequireJs");
let rootPath = "";
requirejs.config({
    baseUrl: rootPath + '/assets/js',
    paths: {
        'sufficit/blazor-before': 'sufficit-blazor-before',
        'framework': rootPath + '/_framework/blazor.webassembly',
        'content/authentication': rootPath + '/_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService'
    },
    shim: {
        'content/authentication': { init: onAuthenticationLoaded },
        'framework': {
            deps: ['content/authentication'], init: onBlazorFrameworkLoaded
        }
    }
});

require(['sufficit/blazor-before', 'framework']);

/** Carregando sistema de autenticação */
function onAuthenticationLoaded() {
    console.debug('SUFFICIT: Authentication system loaded');
}

/** Carregando framework principal, Blazor */
function onBlazorFrameworkLoaded() {
    console.debug("SUFFICIT: Blazor Framework loaded");
}

async function Reload() {
    await caches.delete("blazor-resources-/");

    // Recarrega a página atual sem usar o cache
    document.location.reload(true);
}