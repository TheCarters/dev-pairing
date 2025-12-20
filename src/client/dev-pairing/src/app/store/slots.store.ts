import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { CreateSlot, PairingSlot } from '../core/models/models';
import { inject } from '@angular/core';
import { SlotService } from '../core/services/slot.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

interface SlotsState {
  slots: PairingSlot[];
  isLoading: boolean;
}

const initialState: SlotsState = {
  slots: [],
  isLoading: false,
};

export const SlotsStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, slotService = inject(SlotService)) => ({
    loadSlots: rxMethod<{ groupId: number; start: string; end: string }>(
      pipe(
        tap(() => patchState(store, { isLoading: true })),
        switchMap(({ groupId, start, end }) =>
          slotService.getSlots(groupId, start, end).pipe(
            tap((slots) => {
              patchState(store, { slots, isLoading: false });
            })
          )
        )
      )
    ),
    addSlot: rxMethod<CreateSlot>(
        pipe(
            switchMap(data => slotService.createSlot(data).pipe(
                tap(newSlot => {
                    patchState(store, { slots: [...store.slots(), newSlot] });
                })
            ))
        )
    ),
    deleteSlot: rxMethod<number>(
        pipe(
            switchMap(id => slotService.deleteSlot(id).pipe(
                tap(() => {
                    patchState(store, { slots: store.slots().filter(s => s.id !== id) });
                })
            ))
        )
    )
  }))
);
