import { Component, OnInit, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserStore } from '../../store/user.store';
import { PreferencesStore } from '../../store/preferences.store';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-preferences',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './preferences.html',
  styleUrls: ['./preferences.css']
})
export class PreferencesComponent {
  userStore = inject(UserStore);
  preferencesStore = inject(PreferencesStore);
  notificationService = inject(NotificationService);

  notifyOnSlotJoin = true;
  notifyBeforeSessionStart = true;

  get permissionState() {
    return this.notificationService.permissionState;
  }

  requestPermission() {
    this.notificationService.requestPermission();
  }

  constructor() {
    effect(() => {
      const user = this.userStore.user();
      if (user) {
        this.preferencesStore.loadPreferences(user.id);
      }
    });

    effect(() => {
      const prefs = this.preferencesStore.preferences();
      if (prefs) {
        this.notifyOnSlotJoin = prefs.notifyOnSlotJoin;
        this.notifyBeforeSessionStart = prefs.notifyBeforeSessionStart;
      }
    });
  }

  save() {
    const user = this.userStore.user();
    if (user) {
      if (this.notifyOnSlotJoin && this.permissionState !== 'granted') {
        this.notificationService.requestPermission();
      }

      this.preferencesStore.updatePreferences({
        userId: user.id,
        data: {
          notifyOnSlotJoin: this.notifyOnSlotJoin,
          notifyBeforeSessionStart: this.notifyBeforeSessionStart
        }
      });
      alert('Preferences saved');
    }
  }
}
