console.debug("SUFFICIT: Loading from RequireJs");    
requirejs.config({
    baseUrl: '/assets/js',
    paths: {
        'sufficit/blazor-before': 'sufficit-blazor-before',
        'framework': '/_framework/blazor.webassembly',        
        'content/mudblazor': '/_content/MudBlazor/MudBlazor.min',
        'content/authentication': '/_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService'
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