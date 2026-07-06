import { Component, inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { Tone } from '../tone';

/** Payload for any in-app notification. Kept generic so every notification type reuses one component. */
export interface SnackbarData {
  title: string;
  message?: string;
  /** Material Symbols icon name. */
  icon?: string;
  /** Accent colour, shared with the badge palette. */
  tone?: Tone;
  /** Small muted label, e.g. "Just now". */
  timestamp?: string;
}

/** The one in-app notification card, rendered inside MatSnackBar. Opened via SnackbarService. */
@Component({
  selector: 'app-snackbar',
  templateUrl: './snackbar.html',
  styleUrl: './snackbar.scss',
})
export class Snackbar {
  readonly data = inject<SnackbarData>(MAT_SNACK_BAR_DATA);
  private readonly ref = inject(MatSnackBarRef);

  readonly tone = this.data.tone ?? 'primary';
  readonly icon = this.data.icon ?? 'notifications';

  dismiss(): void {
    this.ref.dismiss();
  }
}
