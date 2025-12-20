import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { DevGroup } from '../core/models/models';
import { inject } from '@angular/core';
import { DevGroupService } from '../core/services/dev-group.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

interface GroupState {
  activeGroup: DevGroup | null;
  isLoading: boolean;
}

const initialState: GroupState = {
  activeGroup: null,
  isLoading: false,
};

export const GroupStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, groupService = inject(DevGroupService)) => ({
    loadGroup: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { isLoading: true })),
        switchMap((identifier) =>
          groupService.getGroup(identifier).pipe(
            tap((group) => {
              patchState(store, { activeGroup: group, isLoading: false });
            })
          )
        )
      )
    )
  }))
);
