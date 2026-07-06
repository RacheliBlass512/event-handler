import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { vi } from 'vitest';
import { EventDto } from '../../../core/models';
import { SnackbarService } from '../../../core/notifications/snackbar.service';
import { SignalrService } from '../../../core/realtime/signalr.service';
import { DispatcherEventsStore } from '../../../core/state/dispatcher-events.store';
import { EventList } from './event-list';

describe('EventList', () => {
  let component: EventList;
  let fixture: ComponentFixture<EventList>;
  const eventCreated$ = new Subject<EventDto>();
  const show = vi.fn();

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventList],
      providers: [
        { provide: DispatcherEventsStore, useValue: { events: signal([]), error: signal(null) } },
        { provide: SignalrService, useValue: { eventCreated$ } },
        { provide: SnackbarService, useValue: { show } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(EventList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('raises a snackbar when a live event arrives', () => {
    eventCreated$.next({ title: 'Boiler', description: 'hot' } as EventDto);
    expect(show).toHaveBeenCalledWith(
      expect.objectContaining({ title: 'New Event: Boiler', message: 'hot' }),
    );
  });
});
