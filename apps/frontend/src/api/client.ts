export const API_BASE = (import.meta.env.VITE_API_BASE ?? '') as string;

export class ApiError extends Error {
  status: number;
  statusText: string;
  bodyText: string;
  constructor(res: Response, bodyText: string) {
    super(`${res.status} ${res.statusText}: ${bodyText}`);
    this.name = 'ApiError';
    this.status = res.status;
    this.statusText = res.statusText;
    this.bodyText = bodyText;
  }
}

export async function api<T> (path: string, opts: RequestInit = {}): Promise<T> {
   const token = localStorage.getItem('token');
   const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...(token ? {Authorization: `Bearer ${token}`} : {}),
      ...(opts.headers || {})
   };

   const res = await fetch(`${API_BASE}${path}`, {...opts, headers});

   //Ungraceful logout on 401
//    if (res.status === 401) {
//       try { useAuth.getState().logout(); } catch {}
//   }

   if (!res.ok) {
    const text = await res.text();
    throw new ApiError(res, text);
  }

   if (res.status === 204) return undefined as unknown as T;
  
   const ct = res.headers.get('content-type') || '';

  if (ct.includes('application/json')) {
    return res.json() as T;
  }

  const text = await res.text();

  return text as unknown as T;
}