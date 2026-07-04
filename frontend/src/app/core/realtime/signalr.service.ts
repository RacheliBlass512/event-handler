import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../auth/auth.service';
import { HUB_URL } from '../config/app-config';

/**
 * Connection handshake is real (skeleton-plan.md §13.4 expects a live SignalR connection).
 * Message handling is stubbed — NotificationService on the backend doesn't push real payloads
 * yet, so these are registration points, not working feature code.
 */
@Injectable({ providedIn: 'root' })
export class SignalrService {
  private connection: signalR.HubConnection | null = null;

  constructor(private readonly authService: AuthService) {}

  connect(): signalR.HubConnection {
    if (this.connection) {
      return this.connection;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, { accessTokenFactory: () => this.authService.getToken() ?? '' })
      .withAutomaticReconnect()
      .build();

    void this.connection.start();
    return this.connection;
  }

  disconnect(): void {
    void this.connection?.stop();
    this.connection = null;
  }

  // TODO: real payload types once NotificationService pushes EventUpdated/Alert for real.
  onEventUpdated(callback: (payload: unknown) => void): void {
    this.connection?.on('EventUpdated', callback);
  }

  onAlert(callback: (payload: unknown) => void): void {
    this.connection?.on('Alert', callback);
  }
}
