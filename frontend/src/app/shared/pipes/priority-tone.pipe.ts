import { Pipe, PipeTransform } from '@angular/core';
import { Priority, priorityTone } from '../../core/models';
import { Tone } from '../ui/tone';

@Pipe({ name: 'priorityTone' })
export class PriorityTonePipe implements PipeTransform {
  transform(value: Priority): Tone {
    return priorityTone(value);
  }
}
