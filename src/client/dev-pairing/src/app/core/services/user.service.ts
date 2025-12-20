import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { CreateUser, DevGroup, JoinGroup, User } from '../models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private api = inject(ApiService);

  createOrFindUser(data: CreateUser): Observable<User> {
    return this.api.post<User>('/users', data);
  }

  getUser(id: number): Observable<User> {
    return this.api.get<User>(`/users/${id}`);
  }

  getUserByEmail(email: string): Observable<User> {
    return this.api.get<User>(`/users/email/${email}`);
  }

  getUserGroups(userId: number): Observable<DevGroup[]> {
    return this.api.get<DevGroup[]>(`/users/${userId}/groups`);
  }

  joinGroup(data: JoinGroup): Observable<void> {
    return this.api.post<void>('/memberships', data);
  }
}
