import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { UpdatePreferences, UserPreferences } from '../models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PreferencesService {
  private api = inject(ApiService);

  getPreferences(userId: number): Observable<UserPreferences> {
    return this.api.get<UserPreferences>(`/users/${userId}/preferences`);
  }

  updatePreferences(userId: number, data: UpdatePreferences): Observable<UserPreferences> {
    return this.api.put<UserPreferences>(`/users/${userId}/preferences`, data);
  }
}
