async function LoadBlazorScriptMethod(onload) {
    const scriptId = 'SufficitBlazorLoader';
    const scriptUrl = '_framework/blazor.webassembly.js?version=net7.0';

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

async function StartBlazor() {
    let loadedCount = 0;
    const resourcesToLoad = [];
    Blazor.start({
        loadBootResource:
            function (type, filename, defaultUri, integrity) {
                if (type == "dotnetjs")
                    return defaultUri;

                const fetchResources = fetch(defaultUri, {
                    cache: 'no-cache',
                    integrity: integrity
                });

                resourcesToLoad.push(fetchResources);

                fetchResources.then((r) => {
                    loadedCount += 1;
                    if (filename == "blazor.boot.json")
                        return;

                    const totalCount = resourcesToLoad.length;
                    const percentLoaded = 10 + parseInt((loadedCount * 90.0) / totalCount);

                    const progressbar = document.getElementById('splash-progress');
                    const elProgressStatus = document.getElementById('splash-status');
                    const elProgressMsg = document.getElementById('splash-message');

                    if (percentLoaded < 100) {
                        progressbar.style.width = percentLoaded + '%';
                        elProgressStatus.innerHTML = parseInt(percentLoaded, 10) + ' %';
                        elProgressMsg.innerText = 'baixando arquivos para o funcionamento do aplicativo !';
                    } else {
                        elProgressMsg.innerText = 'soluções em tecnologia da informação';
                        elProgressStatus.innerHTML = 'sufficit';
                    }

                    progressbar.style.width = percentLoaded + '%';
                });

                return fetchResources;
            }
    });
}

LoadBlazorScriptMethod(StartBlazor);