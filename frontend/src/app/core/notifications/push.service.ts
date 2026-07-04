import { Injectable } from '@angular/core';
import { PushSubscriptionDto } from '../models';

/**
 * Mode B (browser-closed alerts) — thin skeleton stub, per this session's decision. Service
 * worker registration is real (trivial browser API call); the actual subscribe flow and VAPID
 * key wiring are not implemented yet.
 */
@Injectable({ providedIn: 'root' })
export class PushService {
  async registerServiceWorker(): Promise<ServiceWorkerRegistration | null> {
    if (!('serviceWorker' in navigator)) {
      return null;
    }

    return navigator.serviceWorker.register('/sw.js');
  }

  async subscribe(): Promise<PushSubscriptionDto | null> {
    // TODO: PushManager.subscribe() with the VAPID public key from config, then POST to
    // /api/push/subscribe.
    return null;
  }

  async unsubscribe(): Promise<void> {
    // TODO: PushManager unsubscribe + DELETE /api/push/subscribe.
  }
}
