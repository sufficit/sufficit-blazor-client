const ChatWootMessages = [];
const DotNetObjectReferences = [];
window.addEventListener('message', (event) => {
    ChatWootMessages.push(event.data);
    for (let Reference of DotNetObjectReferences) {
        for (let ChatWootMessage of ChatWootMessages) {
            Reference.invokeMethodAsync('OnMessageEvent', ChatWootMessage);
        }
    }
});

window.addMessageEventListener = function (Reference) {
    DotNetObjectReferences.push(Reference);
    for (let ChatWootMessage of ChatWootMessages) {
        Reference.invokeMethodAsync('OnMessageEvent', ChatWootMessage);
    };
}