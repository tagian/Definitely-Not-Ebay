import { api } from '../client';
import type { UserRecommendationResponse } from '../types'

export function getRecommendations(userId: number, top = 30) {
  const qs = top ? `?top=${encodeURIComponent(String(top))}` : '';
  return api<UserRecommendationResponse[]>(`/api/Recommendations/${userId}${qs}`);
}
