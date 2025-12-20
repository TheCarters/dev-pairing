import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { UpdatePreferences, UserPreferences } from '../core/models/models';
import { inject } from '@angular/core';
import { PreferencesService } from '../core/services/preferences.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

interface PreferencesState {
  preferences: UserPreferences | null;
  isLoading: boolean;
}

const initialState: PreferencesState = {
  preferences: null,
  isLoading: false,
};

export const PreferencesStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, preferencesService = inject(PreferencesService)) => ({
    loadPreferences: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { isLoading: true })),
        switchMap((userId) =>
          preferencesService.getPreferences(userId).pipe(
            tap((preferences) => {
              patchState(store, { preferences, isLoading: false });
            })
          )
        )
      )
    ),
    updatePreferences: rxMethod<{ userId: number; data: UpdatePreferences }>(
      pipe(
        switchMap(({ userId, data }) =>
          preferencesService.updatePreferences(userId, data).pipe(
            tap((preferences) => {
              patchState(store, { preferences });
            })
          )
        )
      )
    )
  }))
);
