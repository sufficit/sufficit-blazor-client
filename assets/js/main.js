console.debug("SUFFICIT: Loading from RequireJs");
let rootPath = "/sufficit-blazor-client";
requirejs.config({
    baseUrl: rootPath + '/assets/js',
    paths: {
        'sufficit/blazor-before': 'sufficit-blazor-before',
        'framework': rootPath + '/_framework/blazor.webassembly',
        'content/mudblazor': rootPath + '/_content/MudBlazor/MudBlazor.min',
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