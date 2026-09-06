import {api} from '../client';
import type {Bid} from '../types';
export const getBid = (id: number) => api < Bid > (`/api/Bids/${id}`);
export const getBids = (auctionId: number) => api < Bid[] > (`/api/Bids/Auction/${auctionId}`);

export const createBid = (payload: {bidderId: number;hit: number;auctionId: number;}) => api < void > ('/api/Bids', {method: 'POST',body: JSON.stringify(payload)});