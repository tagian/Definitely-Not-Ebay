import { api, ApiError } from "./client";
import type { Order } from "./types";

export const createOrder = (payload: Partial < Order > ) => api < Order > ('/api/Orders', {
   method: 'POST',
   body: JSON.stringify(payload)
});
// export const listMyOrders = () => api < Order[] > (`/api/Orders/mine`);

export async function listMyOrders() {
  try {
    return await api<Order[]>('/api/Orders/mine');
  } catch (e) {
    if (e instanceof ApiError && e.status === 404) return [];
    throw e;
  }
}
