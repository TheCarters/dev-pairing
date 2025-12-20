import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { DevGroup, User } from '../core/models/models';
import { inject } from '@angular/core';
import { UserService } from '../core/services/user.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

interface UserState {
  user: User | null;
  memberships: DevGroup[];
  isAuthenticated: boolean;
  isLoading: boolean;
}

const initialState: UserState = {
  user: null,
  memberships: [],
  isAuthenticated: false,
  isLoading: false,
};

export const UserStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, userService = inject(UserService)) => ({
    loadUserFromStorage: () => {
      const stored = localStorage.getItem('dev_pairing_user');
      if (stored) {
        const user = JSON.parse(stored) as User;
        patchState(store, { user, isAuthenticated: true });
        // Refresh memberships
        userService.getUserGroups(user.id).subscribe(groups => {
           patchState(store, { memberships: groups });
        });
      }
    },
    login: rxMethod<{ email: string; firstName: string; joinGroupId?: string }>(
      pipe(
        tap(() => patchState(store, { isLoading: true })),
        switchMap(({ email, firstName, joinGroupId }) =>
          userService.createOrFindUser({ email, firstName, joinGroupId }).pipe(
            tap((user) => {
              localStorage.setItem('dev_pairing_user', JSON.stringify(user));
              patchState(store, { user, isAuthenticated: true, isLoading: false });
              // Load memberships
              userService.getUserGroups(user.id).subscribe(groups => {
                 patchState(store, { memberships: groups });
              });
            })
          )
        )
      )
    ),
    logout: () => {
        localStorage.removeItem('dev_pairing_user');
        patchState(store, initialState);
    }
  }))
);
