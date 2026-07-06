import { Pipe, PipeTransform, signal } from '@angular/core';

const RTF = new Intl.RelativeTimeFormat(undefined, { numeric: 'auto' });

// One shared clock for all pipe instances; a signal write schedules change
// detection, so the strings re-render without any per-component wiring.
const now = signal(Date.now());
setInterval(() => now.set(Date.now()), 30_000);

// Largest-fitting unit, seconds up to days.
const UNITS: [Intl.RelativeTimeFormatUnit, number][] = [
  ['day', 86_400],
  ['hour', 3_600],
  ['minute', 60],
  ['second', 1],
];

/** ISO timestamp -> "2 minutes ago", re-ticking every 30s. Impure so the
 * memoized result is recomputed against the shared `now` clock. */
@Pipe({ name: 'timeAgo', pure: false })
export class TimeAgoPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    const diffSeconds = Math.round((Date.parse(value) - now()) / 1000);
    const abs = Math.abs(diffSeconds);
    const [unit, size] = UNITS.find(([, s]) => abs >= s) ?? UNITS[UNITS.length - 1];
    return RTF.format(Math.round(diffSeconds / size), unit);
  }
}
