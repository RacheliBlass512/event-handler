// Backend base URL — the Server runs on a different port than `ng serve` (skeleton-plan.md §13.4
// integration check). Hardcoded for the skeleton; move to Angular environment files when real
// deployment configuration is needed.
export const API_BASE_URL = 'http://localhost:5027';
export const HUB_URL = `${API_BASE_URL}/hubs/events`;
