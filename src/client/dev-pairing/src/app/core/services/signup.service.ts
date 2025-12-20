import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { CreateSignup, PairingSignup } from '../models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignupService {
  private api = inject(ApiService);

  createSignup(data: CreateSignup): Observable<PairingSignup> {
    return this.api.post<PairingSignup>('/signups', data);
  }

  deleteSignup(id: number): Observable<void> {
    return this.api.delete<void>(`/signups/${id}`);
  }

  getUserSignups(userId: number): Observable<PairingSignup[]> {
    return this.api.get<PairingSignup[]>(`/signups/user/${userId}`);
  }
}
