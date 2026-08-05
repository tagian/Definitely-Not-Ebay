import { api } from './client';

export type AdminStatsResponse = {
  itemCount: number;
  activeItemCount: number;
  sellersCount: number;
  buyersCount: number;
  usersCount: number;
  categoriesCount: number;
  buyNowOrdersCount: number;
  auctionOrdersCount: number;
  activeAuctionCount: number;
  completedAuctionCount: number;
};

export async function getAdminStats() {
  return api<AdminStatsResponse>('/api/admin/stats');
}

export async function exportAdminStats(opts: {
  format: 'json' | 'xml';
  sellerId?: number;
  start?: string; 
  end?: string;   
}) {
  const params = new URLSearchParams();
  if (opts.format) params.set('format', opts.format);
  if (opts.sellerId != null) params.set('sellerId', String(opts.sellerId));
  if (opts.start) params.set('start', opts.start);
  if (opts.end) params.set('end', opts.end);
  const qs = params.toString() ? `?${params.toString()}` : '';
  return api<string>(`/api/admin/stats/export${qs}`);
}
