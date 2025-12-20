import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SlotService } from '../../core/services/slot.service';
import { SignupService } from '../../core/services/signup.service';
import { PairingSlot, PairingSignup } from '../../core/models/models';
import { UserStore } from '../../store/user.store';
import { GroupStore } from '../../store/group.store';
import { SlotsStore } from '../../store/slots.store';

@Component({
  selector: 'app-slot-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './slot-detail.html',
  styleUrls: ['./slot-detail.css']
})
export class SlotDetailComponent implements OnInit {
  route = inject(ActivatedRoute);
  router = inject(Router);
  slotService = inject(SlotService);
  signupService = inject(SignupService);
  userStore = inject(UserStore);
  groupStore = inject(GroupStore);
  slotsStore = inject(SlotsStore); // To update list if modified

  slot = signal<PairingSlot | null>(null);
  isLoading = signal(false);

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('slotId');
      if (id) {
        this.loadSlot(+id);
      }
    });
  }

  loadSlot(id: number) {
    this.isLoading.set(true);
    this.slotService.getSlot(id).subscribe({
      next: (s) => {
        this.slot.set(s);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        // Handle error (e.g. not found)
      }
    });
  }

  isOwner(): boolean {
    return this.slot()?.owner.id === this.userStore.user()?.id;
  }

  isSignedUp(): boolean {
    const userId = this.userStore.user()?.id;
    return !!this.slot()?.signups?.some(s => s.user.id === userId);
  }

  getMySignupId(): number | undefined {
    const userId = this.userStore.user()?.id;
    return this.slot()?.signups?.find(s => s.user.id === userId)?.id;
  }

  join() {
    const s = this.slot();
    const u = this.userStore.user();
    if (s && u) {
      this.signupService.createSignup({ slotId: s.id, userId: u.id }).subscribe(() => {
        this.loadSlot(s.id);
        // Also refresh store list if needed, or just let it be stale until reload
      });
    }
  }

  leave() {
    const id = this.getMySignupId();
    if (id) {
      this.signupService.deleteSignup(id).subscribe(() => {
         const s = this.slot();
         if(s) this.loadSlot(s.id);
      });
    }
  }

  deleteSlot() {
    const s = this.slot();
    if (s && confirm('Are you sure you want to delete this slot?')) {
      this.slotService.deleteSlot(s.id).subscribe(() => {
        this.router.navigate(['../'], { relativeTo: this.route });
      });
    }
  }

  close() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
