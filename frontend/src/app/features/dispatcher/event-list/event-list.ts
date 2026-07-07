import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DispatcherEventsStore } from '../../../core/state/dispatcher-events.store';
import { SignalrService } from '../../../core/realtime/signalr.service';
import { SnackbarService } from '../../../core/notifications/snackbar.service';
import { EventStatusGlyphPipe } from '../../../shared/pipes/event-status-glyph.pipe';
import { EventStatusLabelPipe } from '../../../shared/pipes/event-status-label.pipe';
import { EventStatusTonePipe } from '../../../shared/pipes/event-status-tone.pipe';
import { PriorityTonePipe } from '../../../shared/pipes/priority-tone.pipe';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';
import { Badge, EmptyState } from '../../../shared/ui';

@Component({
  selector: 'app-event-list',
  imports: [
    Badge,
    EmptyState,
    EventStatusGlyphPipe,
    EventStatusLabelPipe,
    EventStatusTonePipe,
    PriorityTonePipe,
    TimeAgoPipe,
  ],
  templateUrl: './event-list.html',
  styleUrl: './event-list.scss',
})
export class EventList {
  private readonly store = inject(DispatcherEventsStore);
  private readonly signalr = inject(SignalrService);
  private readonly snackbar = inject(SnackbarService);

  readonly events = this.store.events;
  readonly error = this.store.error;

  constructor() {
    // ponytail: subscribed on the events screen (only dispatcher view in scope);
    // lift to Shell if realtime goes app-wide.
    this.signalr.eventCreated$.pipe(takeUntilDestroyed()).subscribe((dto) =>
      this.snackbar.show({
        icon: 'notifications',
        tone: 'primary',
        title: `New Event: ${dto.title}`,
        message: dto.description,
        timestamp: 'Just now',
      }),
    );
  }
}
