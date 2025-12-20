import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserStore } from '../../../store/user.store';
import { FormsModule } from '@angular/forms';
import { DevGroupService } from '../../../core/services/dev-group.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-no-group',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './no-group.html',
  styleUrls: ['./no-group.css']
})
export class NoGroupComponent {
  userStore = inject(UserStore);
  groupService = inject(DevGroupService);
  router = inject(Router);

  newGroupName = '';

  createGroup() {
    if (!this.newGroupName) return;
    this.groupService.createGroup({ name: this.newGroupName }).subscribe(group => {
      this.router.navigate(['/g', group.identifier]);
    });
  }
}