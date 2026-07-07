import { Component, input } from '@angular/core';
import { Tone } from '../tone';

/** The one pill/dot implementation for status & priority labels (ui-component-library-plan.md). */
@Component({
  selector: 'app-badge',
  templateUrl: './badge.html',
  styleUrl: './badge.scss',
})
export class Badge {
  readonly tone = input<Tone>('gray');
  readonly variant = input<'soft' | 'dot'>('soft');
  /** Small leading character, e.g. the demo's status glyphs ◉ ◎ ▶ ✓ ✕. */
  readonly glyph = input<string>('');
}
