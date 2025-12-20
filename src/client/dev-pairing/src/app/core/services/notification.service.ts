import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private eventSources: Map<string, EventSource> = new Map();

  constructor() {
    this.requestPermission();
  }

  requestPermission() {
    if ('Notification' in window) {
      Notification.requestPermission();
    }
  }

  subscribeToTopic(topic: string) {
    if (this.eventSources.has(topic)) return;

    const url = `https://ntfy.sh/${topic}/sse`;
    const eventSource = new EventSource(url);

    eventSource.onmessage = (event) => {
      const data = JSON.parse(event.data);
      this.showNotification(data.title || 'Dev Pairing', data.message);
    };

    eventSource.onerror = (err) => {
      console.error('Ntfy SSE error:', err);
      // Optional: reconnect logic or just let EventSource retry
    };

    this.eventSources.set(topic, eventSource);
  }

  unsubscribeFromTopic(topic: string) {
    const es = this.eventSources.get(topic);
    if (es) {
      es.close();
      this.eventSources.delete(topic);
    }
  }

  private showNotification(title: string, body: string) {
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification(title, { body });
    }
  }
}
