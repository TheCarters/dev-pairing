import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../store/user.store';

@Component({
  selector: 'app-user-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-login.html',
  styleUrls: ['./user-login.css']
})
export class UserLoginComponent {
  userStore = inject(UserStore);

  @Input() joinGroupId?: string;

  firstName = '';
  email = '';

  login() {
    if (this.firstName && this.email) {
      this.userStore.login({ 
        firstName: this.firstName, 
        email: this.email, 
        joinGroupId: this.joinGroupId 
      });
    }
  }
}
