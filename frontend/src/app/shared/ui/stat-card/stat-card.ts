import { Component, input } from '@angular/core';
import { Tone } from '../tone';

/** Dashboard summary tile — icon + big number + label (ui-component-library-plan.md). */
@Component({
  selector: 'app-stat-card',
  templateUrl: './stat-card.html',
  styleUrl: './stat-card.scss',
})
export class StatCard {
  readonly icon = input<string>('');
  readonly value = input<string | number>('');
  readonly label = input<string>('');
  readonly tone = input<Tone>('neutral');
}
