import { api } from '../client';
import type { AdminStats } from '../types';

export function getAdminStats() {
  return api<AdminStats>('/api/admin/stats');
}

export function exportAdminStats(opts: {
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
