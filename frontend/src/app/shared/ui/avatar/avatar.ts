import { Component, computed, input } from '@angular/core';

const HUES = [0, 25, 45, 90, 145, 190, 220, 260, 290, 330];

/** Technician/user avatar — initials only, no photo upload (ui-component-library-plan.md). */
@Component({
  selector: 'app-avatar',
  templateUrl: './avatar.html',
  styleUrl: './avatar.scss',
})
export class Avatar {
  readonly name = input<string>('');
  readonly size = input<'sm' | 'md' | 'lg'>('md');

  readonly initials = computed(() => initialsOf(this.name()));
  readonly hue = computed(() => HUES[hashCode(this.name()) % HUES.length]);
}

function initialsOf(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  const first = parts[0][0];
  const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
  return (first + last).toUpperCase();
}

function hashCode(value: string): number {
  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = (hash * 31 + value.charCodeAt(i)) | 0;
  }
  return Math.abs(hash);
}
