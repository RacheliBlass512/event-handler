import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { HUB_URL } from '../config/app-config';
import { EventDto } from '../models';

/**
 * Real SignalR connection to the server's notification hub. Handles the JWT handshake,
 * auto-reconnect, and surfaces live pushes as RxJS streams (occurrences over time).
 */
@Injectable({ providedIn: 'root' })
export class SignalrService {
  private connection: signalR.HubConnection | null = null;
  private readonly eventCreated = new Subject<EventDto>();

  readonly eventCreated$: Observable<EventDto> = this.eventCreated.asObservable();

  constructor(private readonly authService: AuthService) {}

  connect(): signalR.HubConnection {
    if (this.connection) {
      return this.connection;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, { accessTokenFactory: () => this.authService.getToken() ?? '' })
      .withAutomaticReconnect()
      .build();

    // Register before start() so the handler exists when the first push arrives.
    this.connection.on('EventCreated', (dto: EventDto) => this.eventCreated.next(dto));

    void this.connection.start();
    return this.connection;
  }

  disconnect(): void {
    void this.connection?.stop();
    this.connection = null;
  }
}
