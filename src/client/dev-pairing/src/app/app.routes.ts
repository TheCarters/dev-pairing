import { Routes } from '@angular/router';
import { NoGroupComponent } from './features/group/no-group/no-group';
import { PreferencesComponent } from './features/preferences/preferences';
import { GroupLayoutComponent } from './features/group/group-layout/group-layout';
import { CalendarComponent } from './features/calendar/calendar';
import { SlotDetailComponent } from './features/slot-detail/slot-detail';
import { AdminComponent } from './features/admin/admin';

export const routes: Routes = [
  { path: '', component: NoGroupComponent },
  { path: 'preferences', component: PreferencesComponent },
  { path: 'admin', component: AdminComponent },
  { 
    path: 'g/:groupId', 
    component: GroupLayoutComponent,
    children: [
        { path: '', component: CalendarComponent },
        { path: 'slot/:slotId', component: SlotDetailComponent }
    ]
  }
];