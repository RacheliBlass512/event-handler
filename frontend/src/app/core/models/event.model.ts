// Mirrors backend/src/EventHandler.Server/Api/Dtos/EventDtos.cs.

import { EventChangeKind, EventStatus, Priority } from './enums';

export interface EventDto {
  id: string;
  title: string;
  description: string;
  sourceName: string;
  sourceEventId: string;
  location: string;
  status: EventStatus;
  priority: Priority;
  assignedTechnicianId: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface EventHistoryDto {
  kind: EventChangeKind;
  fromStatus: EventStatus | null;
  toStatus: EventStatus | null;
  fromAssigneeId: string | null;
  toAssigneeId: string | null;
  changedByDisplayName: string;
  changedAt: string;
  note: string | null;
}

export interface AssignRequestDto {
  technicianId: string;
}

export interface TransferRequestDto {
  toTechnicianId: string;
}

export interface StatusChangeRequestDto {
  to: EventStatus;
  note: string | null;
}

export interface NoteDto {
  note: string;
}
