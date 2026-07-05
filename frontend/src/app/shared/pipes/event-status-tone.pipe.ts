import { Pipe, PipeTransform } from '@angular/core';
import { EventStatus, eventStatusTone } from '../../core/models';
import { Tone } from '../ui/tone';

@Pipe({ name: 'eventStatusTone' })
export class EventStatusTonePipe implements PipeTransform {
  transform(value: EventStatus): Tone {
    return eventStatusTone(value);
  }
}
