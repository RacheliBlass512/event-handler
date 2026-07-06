import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { EventsApi } from '../api/events.api';
import { AuthService } from '../auth/auth.service';
import { EventDto, EventStatus, Priority } from '../models';
import { AuthState } from '../auth/auth.models';
import { SignalrService } from '../realtime/signalr.service';
import { DispatcherEventsStore } from './dispatcher-events.store';

function event(id: string, overrides: Partial<EventDto> = {}): EventDto {
  return {
    id,
    title: `Event ${id}`,
    description: '',
    sourceName: 'test',
    sourceEventId: id,
    location: '',
    status: EventStatus.New,
    priority: Priority.Normal,
    assignedTechnicianId: null,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    ...overrides,
  };
}

describe('DispatcherEventsStore', () => {
  const currentUser = signal<AuthState | null>({ token: 't' } as AuthState);
  const eventCreated$ = new Subject<EventDto>();
  let seed: EventDto[];

  function makeStore(): DispatcherEventsStore {
    seed = [event('a'), event('b')];
    TestBed.configureTestingModule({
      providers: [
        DispatcherEventsStore,
        { provide: EventsApi, useValue: { list: () => of(seed) } },
        { provide: SignalrService, useValue: { eventCreated$, connect: () => {}, disconnect: () => {} } },
        { provide: AuthService, useValue: { currentUser } },
      ],
    });
    const store = TestBed.inject(DispatcherEventsStore);
    TestBed.tick(); // flush the auth-driven effect (connect + seed load)
    return store;
  }

  it('seeds events from the API on an authenticated user', () => {
    const store = makeStore();
    expect(store.events().map((e) => e.id)).toEqual(['a', 'b']);
  });

  it('prepends a live push and dedups by id', () => {
    const store = makeStore();

    eventCreated$.next(event('c'));
    expect(store.events().map((e) => e.id)).toEqual(['c', 'a', 'b']);

    eventCreated$.next(event('c')); // duplicate id — ignored
    expect(store.events().map((e) => e.id)).toEqual(['c', 'a', 'b']);
  });
});
