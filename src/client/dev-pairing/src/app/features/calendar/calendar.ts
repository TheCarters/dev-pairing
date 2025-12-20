import { Component, inject, signal, effect, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventClickArg, DateSelectArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { SlotsStore } from '../../store/slots.store';
import { GroupStore } from '../../store/group.store';
import { UserStore } from '../../store/user.store';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule, FormsModule],
  templateUrl: './calendar.html',
  styleUrls: ['./calendar.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CalendarComponent {
  slotsStore = inject(SlotsStore);
  groupStore = inject(GroupStore);
  userStore = inject(UserStore);
  router = inject(Router);

  showCreateModal = signal(false);
  newSlotTitle = signal('');
  newSlotStart = signal('');
  newSlotEnd = signal('');

  calendarEvents = signal<any[]>([]);

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'timeGridWeek',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    selectable: true,
    selectMirror: true,
    dayMaxEvents: true,
    select: this.handleDateSelect.bind(this),
    eventClick: this.handleEventClick.bind(this),
    datesSet: this.handleDatesSet.bind(this)
  };

  handleDatesSet(arg: any) {
    const group = this.groupStore.activeGroup();
    if (group) {
        this.slotsStore.loadSlots({ 
            groupId: group.id, 
            start: arg.startStr, 
            end: arg.endStr 
        });
    }
  }

  constructor() {
    effect(() => {
      const slots = this.slotsStore.slots();
      this.calendarEvents.set(slots.map(s => {
        const signups = s.signups || [];
        return {
          id: s.id.toString(),
          title: s.title + (signups.length > 0 ? ` (${signups.length})` : ''),
          start: s.startTime,
          end: s.endTime,
          backgroundColor: signups.some(su => su.user.id === this.userStore.user()?.id) ? '#10b981' : '#3b82f6'
        };
      }));
    });
  }

  handleDateSelect(selectInfo: DateSelectArg) {
    this.newSlotStart.set(selectInfo.startStr);
    this.newSlotEnd.set(selectInfo.endStr);
    this.newSlotTitle.set('');
    this.showCreateModal.set(true);
    const calendarApi = selectInfo.view.calendar;
    calendarApi.unselect();
  }

  handleEventClick(clickInfo: EventClickArg) {
    this.router.navigate(['g', this.groupStore.activeGroup()?.identifier, 'slot', clickInfo.event.id]);
  }

  createSlot() {
    const group = this.groupStore.activeGroup();
    const user = this.userStore.user();
    const title = this.newSlotTitle();
    
    if (group && user && title) {
      this.slotsStore.addSlot({
        devGroupId: group.id,
        ownerId: user.id,
        startTime: this.newSlotStart(),
        endTime: this.newSlotEnd(),
        title: title
      });
      this.showCreateModal.set(false);
    }
  }

  // Need to load slots when view changes.
  // For simplicity, I'll load on init for a fixed range or just rely on store logic?
  // Store `loadSlots` needs range.
  // I should hook into `datesSet` callback of FullCalendar.
  // Adding `datesSet` to options.
}
