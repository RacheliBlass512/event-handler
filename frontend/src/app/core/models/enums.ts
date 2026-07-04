// Mirrors backend/src/EventHandler.Domain/Enums/*.cs. Server serializes enums as strings
// (see Program.cs JsonStringEnumConverter), so these values must match the C# names exactly.

export enum EventStatus {
  New = 'New',
  Assigned = 'Assigned',
  InProgress = 'InProgress',
  Resolved = 'Resolved',
  Closed = 'Closed',
  Canceled = 'Canceled',
}

export enum UserRole {
  Dispatcher = 'Dispatcher',
  Technician = 'Technician',
}

export enum Priority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
  Critical = 'Critical',
}

export enum EventChangeKind {
  StatusChanged = 'StatusChanged',
  Assigned = 'Assigned',
  Transferred = 'Transferred',
  NoteAdded = 'NoteAdded',
}
