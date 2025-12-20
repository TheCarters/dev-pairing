import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { CreateSlot, PairingSlot, UpdateSlot } from '../models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SlotService {
  private api = inject(ApiService);

  getSlots(groupId: number, start: string, end: string): Observable<PairingSlot[]> {
    return this.api.get<PairingSlot[]>('/slots', { groupId, start, end });
  }

  getSlot(id: number): Observable<PairingSlot> {
    return this.api.get<PairingSlot>(`/slots/${id}`);
  }

  createSlot(data: CreateSlot): Observable<PairingSlot> {
    return this.api.post<PairingSlot>('/slots', data);
  }

  updateSlot(id: number, data: UpdateSlot): Observable<PairingSlot> {
    return this.api.put<PairingSlot>(`/slots/${id}`, data);
  }

  deleteSlot(id: number): Observable<void> {
    return this.api.delete<void>(`/slots/${id}`);
  }
}
