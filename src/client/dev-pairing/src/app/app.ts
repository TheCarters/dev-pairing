import { Component, signal, inject, effect, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserStore } from './store/user.store';
import { SignupsStore } from './store/signups.store';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  userStore = inject(UserStore);
  signupsStore = inject(SignupsStore);
  notificationService = inject(NotificationService);

  constructor() {
    effect(() => {
      const user = this.userStore.user();
      if (user) {
        this.signupsStore.loadSignups(user.id);
        this.notificationService.init(user.id);
      }
    });
  }

  ngOnInit() {
    this.userStore.loadUserFromStorage();
  }
}