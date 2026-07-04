import { Pipe, PipeTransform } from '@angular/core';
import { EventStatus } from '../../core/models';

const LABELS: Record<EventStatus, string> = {
  [EventStatus.New]: 'New',
  [EventStatus.Assigned]: 'Assigned',
  [EventStatus.InProgress]: 'In Progress',
  [EventStatus.Resolved]: 'Resolved',
  [EventStatus.Closed]: 'Closed',
  [EventStatus.Canceled]: 'Canceled',
};

/** Reused by both dispatcher and technician event-list/event-detail screens. */
@Pipe({ name: 'eventStatusLabel' })
export class EventStatusLabelPipe implements PipeTransform {
  transform(value: EventStatus): string {
    return LABELS[value] ?? value;
  }
}
