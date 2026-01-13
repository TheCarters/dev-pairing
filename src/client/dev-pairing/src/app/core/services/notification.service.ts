import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private eventSource: EventSource | null = null;
  private currentUserId: number | null = null;

  init(userId: number) {
    if (this.currentUserId === userId) return;
    this.currentUserId = userId;

    if (this.eventSource) {
      this.eventSource.close();
    }

    if (!('Notification' in window)) {
      console.log('This browser does not support desktop notification');
      return;
    }

    if (Notification.permission === 'default') {
      Notification.requestPermission();
    }

    const topic = `dev-pairing-user-${userId}`;
    const url = `https://ntfy.sh/${topic}/sse`;
    
    this.eventSource = new EventSource(url);
    
    this.eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        if (data.event === 'message') {
           this.showNotification(data.title || 'Dev Pairing', data.message);
        }
      } catch (e) {
        console.error('Error parsing notification data', e);
      }
    };

    this.eventSource.onerror = (err) => {
      console.error('EventSource failed:', err);
    };
  }

  requestPermission() {
    if ('Notification' in window) {
      Notification.requestPermission();
    }
  }

  get permissionState(): NotificationPermission {
      if (!('Notification' in window)) return 'denied';
      return Notification.permission;
  }

  private showNotification(title: string, body: string) {
    if (Notification.permission === 'granted') {
      new Notification(title, { body });
    }
  }
}
