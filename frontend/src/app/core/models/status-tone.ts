import { Tone } from '../../shared/ui/tone';
import { EventStatus, Priority } from './enums';

const EVENT_STATUS_TONES: Record<EventStatus, Tone> = {
  [EventStatus.New]: 'info',
  [EventStatus.Assigned]: 'warning',
  [EventStatus.InProgress]: 'primary',
  [EventStatus.Resolved]: 'success',
  [EventStatus.Closed]: 'neutral',
  [EventStatus.Canceled]: 'danger',
};

const PRIORITY_TONES: Record<Priority, Tone> = {
  [Priority.Low]: 'success',
  [Priority.Normal]: 'warning',
  [Priority.High]: 'primary',
  [Priority.Critical]: 'danger',
};

export function eventStatusTone(status: EventStatus): Tone {
  return EVENT_STATUS_TONES[status];
}

export function priorityTone(priority: Priority): Tone {
  return PRIORITY_TONES[priority];
}
