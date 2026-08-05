import {api, ApiError} from './client';
import type {Auction} from './types';

export const getAuction = (id: number) => api < Auction > (`/api/Auctions/${id}`);
export const getActiveAuction = (itemId: number) => api < Auction > (`/api/Auctions/active/${itemId}`);
// export const listMyAuctions = () => api < Auction[] > (`/api/Auctions/seller/mine`);

export const createAuction = (payload: Partial < Auction > ) => api < boolean > ('/api/Auctions', {
   method: 'POST',
   body: JSON.stringify(payload)
});
export const updateAuction = (id: number, payload: Partial < Auction > ) => api < void > (`/api/Auctions/${id}`, {
   method: 'PUT',
   body: JSON.stringify(payload)
});


export async function listMyAuctions() {
  try {
    return await api<Auction[]>('/api/Auctions/seller/mine');
  } catch (e) {
    if (e instanceof ApiError && e.status === 404) return [];
    throw e;
  }
}