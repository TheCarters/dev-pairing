import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { CreateDevGroup, DevGroup, User } from '../models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DevGroupService {
  private api = inject(ApiService);

  getGroup(identifier: string): Observable<DevGroup> {
    return this.api.get<DevGroup>(`/groups/${identifier}`);
  }

  createGroup(data: CreateDevGroup): Observable<DevGroup> {
    return this.api.post<DevGroup>('/groups', data);
  }

  getMembers(groupId: number): Observable<User[]> {
    return this.api.get<User[]>(`/groups/${groupId}/members`);
  }
}
