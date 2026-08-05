import { api } from './client';

export interface UserRecommendationResponse {
  itemId?: number;
  recommendedItemId?: number;
  score?: number;
}

export function getRecommendations(userId: number, top = 30) {
  const qs = top ? `?top=${encodeURIComponent(String(top))}` : '';
  return api<UserRecommendationResponse[]>(`/api/Recommendations/${userId}${qs}`);
}
