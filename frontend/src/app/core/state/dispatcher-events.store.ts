import { effect, Injectable, signal } from '@angular/core';
import { EventsApi } from '../api/events.api';
import { AuthService } from '../auth/auth.service';
import { EventDto } from '../models';
import { SignalrService } from '../realtime/signalr.service';

/**
 * Owns the dispatcher's events list: seeds from the API once, then folds live
 * `EventCreated` pushes in (newest-first). Connection lifecycle is auth-driven —
 * a signed-in user opens the SignalR connection and seeds; sign-out tears it down.
 *
 * ponytail: the connection opens when this (dispatcher) store is first injected. If
 * technician realtime is added, lift the lifecycle into a root ConnectionManager so it
 * fires for every authenticated user regardless of which view is open.
 */
@Injectable({ providedIn: 'root' })
export class DispatcherEventsStore {
  private readonly _events = signal<EventDto[]>([]);
  readonly events = this._events.asReadonly();
  readonly error = signal<string | null>(null);

  constructor(
    private readonly eventsApi: EventsApi,
    private readonly signalr: SignalrService,
    private readonly authService: AuthService,
  ) {
    this.signalr.eventCreated$.subscribe((dto) => this.merge(dto));

    effect(() => {
      if (this.authService.currentUser()) {
        this.signalr.connect();
        this.load();
      } else {
        this.signalr.disconnect();
        this._events.set([]);
      }
    });
  }

  load(): void {
    this.eventsApi.list().subscribe({
      next: (events) => {
        this._events.set(events);
        this.error.set(null);
      },
      error: (err) => {
        console.error('Failed to load events', err);
        this.error.set('Failed to load events.');
      },
    });
  }

  /** Prepend the pushed event, deduped by id (a push can race the seed load). */
  private merge(dto: EventDto): void {
    this._events.update((events) =>
      events.some((e) => e.id === dto.id) ? events : [dto, ...events],
    );
  }
}
