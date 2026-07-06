import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Snackbar, SnackbarData } from '../../shared/ui';

/**
 * The single entry point for in-app notifications. Every notification type calls `show()`;
 * it owns the MatSnackBar config (position, duration, panel) so call sites stay one-liners.
 * ponytail: thin wrapper — the config seam for all notification types, not a framework.
 */
@Injectable({ providedIn: 'root' })
export class SnackbarService {
  private readonly snackBar = inject(MatSnackBar);

  show(data: SnackbarData): void {
    this.snackBar.openFromComponent(Snackbar, {
      data,
      duration: 6000,
      panelClass: 'app-snackbar-panel',
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }
}
