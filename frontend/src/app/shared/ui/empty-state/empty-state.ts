import { Component, input } from '@angular/core';

/** "Nothing here" placeholder, reused across list views (ui-component-library-plan.md). */
@Component({
  selector: 'app-empty-state',
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  readonly icon = input<string>('inbox');
  readonly title = input<string>('');
  readonly message = input<string>('');
}
