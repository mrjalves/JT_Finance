self.addEventListener('notificationclick', event => {
    event.notification.close();

    const payload = event.notification.data;
    const clickUrl = payload && payload.url ? payload.url : '/';

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then(windowClients => {
            // Verifica se a aplicação já está aberta numa aba
            for (let i = 0; i < windowClients.length; i++) {
                const client = windowClients[i];
                if (client.url.includes(self.registration.scope) && 'focus' in client) {
                    // Já existe uma janela aberta, posta a mensagem em vez de recarregar
                    client.focus();
                    client.postMessage({ type: 'NAVIGATE', url: clickUrl });
                    return;
                }
            }
            
            // Se nenhuma janela estava aberta, abre uma nova
            if (clients.openWindow) {
                return clients.openWindow(clickUrl);
            }
        })
    );
});
