import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { PairingSignup } from '../core/models/models';
import { inject } from '@angular/core';
import { SignupService } from '../core/services/signup.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

interface SignupsState {
  signups: PairingSignup[];
  isLoading: boolean;
}

const initialState: SignupsState = {
  signups: [],
  isLoading: false,
};

export const SignupsStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, signupService = inject(SignupService)) => ({
    loadSignups: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { isLoading: true })),
        switchMap((userId) =>
          signupService.getUserSignups(userId).pipe(
            tap((signups) => {
              patchState(store, { signups, isLoading: false });
            })
          )
        )
      )
    )
  }))
);
