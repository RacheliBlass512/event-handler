import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/app-config';
import {
  AssignRequestDto,
  EventDto,
  EventHistoryDto,
  NoteDto,
  StatusChangeRequestDto,
  TransferRequestDto,
} from '../models';

@Injectable({ providedIn: 'root' })
export class EventsApi {
  private readonly baseUrl = `${API_BASE_URL}/api/events`;

  constructor(private readonly http: HttpClient) {}

  list(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(this.baseUrl);
  }

  listAvailable(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.baseUrl}/available`);
  }

  getById(id: string): Observable<EventDto> {
    return this.http.get<EventDto>(`${this.baseUrl}/${id}`);
  }

  getHistory(id: string): Observable<EventHistoryDto[]> {
    return this.http.get<EventHistoryDto[]>(`${this.baseUrl}/${id}/history`);
  }

  assign(id: string, request: AssignRequestDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/assign`, request);
  }

  transfer(id: string, request: TransferRequestDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/transfer`, request);
  }

  changeStatus(id: string, request: StatusChangeRequestDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/status`, request);
  }

  close(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/close`, {});
  }

  addNote(id: string, request: NoteDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/notes`, request);
  }
}
