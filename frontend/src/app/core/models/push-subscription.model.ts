// Mirrors backend/src/EventHandler.Server/Api/Dtos/PushSubscriptionDto.cs.

export interface PushSubscriptionDto {
  endpoint: string;
  p256dh: string;
  auth: string;
}
