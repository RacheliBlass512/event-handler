import { Component, input } from '@angular/core';
import { Tone } from '../tone';

/** The one pill/dot implementation for status & priority labels (ui-component-library-plan.md). */
@Component({
  selector: 'app-badge',
  templateUrl: './badge.html',
  styleUrl: './badge.scss',
})
export class Badge {
  readonly tone = input<Tone>('neutral');
  readonly variant = input<'soft' | 'dot'>('soft');
}
