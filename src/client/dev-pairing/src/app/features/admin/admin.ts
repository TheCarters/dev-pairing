import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styles: [`
    :host {
      display: block;
      padding: 2rem;
    }
  `]
})
export class AdminComponent {
  api = inject(ApiService);
  
  topic = '';
  message = 'Testing';

  sendNotification() {
    if (!this.topic) {
        alert('Please enter a topic (e.g. from a slot)');
        return;
    }

    // We don't have a direct "send to topic" endpoint exposed for generic use in the plan,
    // but the backend NtfyService exists. 
    // Ideally we'd have an admin endpoint.
    // For now, I'll assume we can create a temporary endpoint or use an existing one hackily?
    // No, let's create a proper admin endpoint in the backend for this test.
    
    this.api.post('/admin/notify', { topic: this.topic, message: this.message }).subscribe({
        next: () => alert('Sent!'),
        error: (err: any) => alert('Error: ' + err.message)
    });
  }
}
