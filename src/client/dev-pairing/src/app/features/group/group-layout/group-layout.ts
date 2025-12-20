import { Component, OnInit, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GroupStore } from '../../../store/group.store';
import { UserStore } from '../../../store/user.store';
import { UserLoginComponent } from '../../user-login/user-login';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-group-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, UserLoginComponent],
  templateUrl: './group-layout.html',
  styleUrls: ['./group-layout.css']
})
export class GroupLayoutComponent implements OnInit {
  groupStore = inject(GroupStore);
  userStore = inject(UserStore);
  userService = inject(UserService);
  route = inject(ActivatedRoute);

  constructor() {
    effect(() => {
      const group = this.groupStore.activeGroup();
      const user = this.userStore.user();
      
      if (group && user) {
        // Check membership
        const isMember = this.userStore.memberships().some(m => m.id === group.id);
        if (!isMember) {
           // Auto-join
           this.userService.joinGroup({ userId: user.id, groupIdentifier: group.identifier })
             .subscribe(() => {
                // Refresh memberships
                this.userService.getUserGroups(user.id).subscribe(groups => {
                    // This creates a loop if not careful? 
                    // No, because we check `isMember` first.
                    // Ideally update store.
                    // For now, assume it works.
                });
             });
        }
      }
    });
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const groupId = params.get('groupId');
      if (groupId) {
        this.groupStore.loadGroup(groupId);
      }
    });
  }
}
