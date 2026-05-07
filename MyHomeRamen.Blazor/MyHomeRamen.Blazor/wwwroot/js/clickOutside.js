const registry = new Map();

export function registerClickOutside(element, dotNetRef) {
    const handler = (event) => {
        if (!element.contains(event.target)) {
            dotNetRef.invokeMethodAsync('CloseMenu');
        }
    };

    registry.set(element, handler);
    document.addEventListener('mousedown', handler);
}

export function unregisterClickOutside(element) {
    const handler = registry.get(element);
    if (handler) {
        document.removeEventListener('mousedown', handler);
        registry.delete(element);
    }
}
