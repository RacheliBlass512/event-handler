import { Tone } from '../../shared/ui/tone';
import { EventStatus, Priority } from './enums';

// Tones and glyphs match the reference demo's badges 1:1 (Canceled reuses Closed's
// gray/✕ — the demo has no Canceled status).
const EVENT_STATUS_TONES: Record<EventStatus, Tone> = {
  [EventStatus.New]: 'blue',
  [EventStatus.Assigned]: 'amber',
  [EventStatus.InProgress]: 'orange',
  [EventStatus.Resolved]: 'green',
  [EventStatus.Closed]: 'gray',
  [EventStatus.Canceled]: 'gray',
};

const EVENT_STATUS_GLYPHS: Record<EventStatus, string> = {
  [EventStatus.New]: '◉',
  [EventStatus.Assigned]: '◎',
  [EventStatus.InProgress]: '▶',
  [EventStatus.Resolved]: '✓',
  [EventStatus.Closed]: '✕',
  [EventStatus.Canceled]: '✕',
};

const PRIORITY_TONES: Record<Priority, Tone> = {
  [Priority.Low]: 'green',
  [Priority.Normal]: 'yellow',
  [Priority.High]: 'orange',
  [Priority.Critical]: 'red',
};

export function eventStatusTone(status: EventStatus): Tone {
  return EVENT_STATUS_TONES[status];
}

export function eventStatusGlyph(status: EventStatus): string {
  return EVENT_STATUS_GLYPHS[status];
}

export function priorityTone(priority: Priority): Tone {
  return PRIORITY_TONES[priority];
}
