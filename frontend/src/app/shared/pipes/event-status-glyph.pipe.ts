import { Pipe, PipeTransform } from '@angular/core';
import { EventStatus, eventStatusGlyph } from '../../core/models';

@Pipe({ name: 'eventStatusGlyph' })
export class EventStatusGlyphPipe implements PipeTransform {
  transform(value: EventStatus): string {
    return eventStatusGlyph(value);
  }
}
