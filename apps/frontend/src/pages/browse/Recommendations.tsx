/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../store/authstore';
import { getItem } from '../../api/endpoints/items';
import { getRecommendations } from '../../api/endpoints/recommendations';
import type { Item, UserRecommendationResponse } from '../../api/types';

type LoadState = 'idle' | 'loading' | 'done' | 'error';

export default function Recommendations() {
  const { status, user} = useAuth();
  const [fetchState, setFetchState] = useState<LoadState>('idle');
  const [error, setError] = useState<string | null>(null);
  const [top, setTop] = useState<number>(20);
  const [items, setItems] = useState<Item[]>([]);


  useEffect(() => {
    if (status !== 'authed' || !user) return;

    let cancelled = false;
    (async () => {
      setFetchState('loading');
      setError(null);
      try {
        const userId = (user as any).id ?? (user as any).userId;
        if (!userId) throw new Error('Missing user id');

        const recs: UserRecommendationResponse[] = await getRecommendations(userId, top);

        const ids = (recs ?? [])
          .map((r) => r.itemId ?? r.recommendedItemId)
          .filter((n): n is number => typeof n === 'number');

        const unique = Array.from(new Set(ids)).slice(0, top);

        const fetched = await Promise.all(
          unique.map(async (id) => {
            try { return await getItem(id); } catch { return null; }
          })
        );

        const valid = fetched.filter((x): x is Item => !!x);
        if (!cancelled) {
          setItems(valid);
          setFetchState('done');
        }
      } catch (e: any) {
        if (!cancelled) {
          setError(e?.message ?? 'Failed to load recommendations');
          setFetchState('error');
        }
      }
    })();

    return () => { cancelled = true; };
  }, [status, user, top]);

  const hasResults = items.length > 0;

  if (status === 'guest') {
    return (
      <div className="max-w-6xl mx-auto p-6">
        <h1 className="text-2xl font-bold mb-3">Recommendations</h1>
        <p className="text-sm mb-4">Sign in to see personalized picks.</p>
        <div className="flex gap-2">
          <Link to="/browse" className="rounded border px-3 py-2 text-sm hover:bg-gray-50">Browse all</Link>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-6">
      <div className="mb-4 flex items-center justify-between gap-3">
        <h1 className="text-2xl font-bold">Recommended for you</h1>
        <div className="flex gap-2">
          <Link to="/browse" className="rounded border px-3 py-2 text-sm hover:bg-gray-50">Browse all</Link>
        </div>
      </div>

      <div className="mb-4 flex items-center gap-2">
        <label className="text-sm">Show</label>
        <select
          className="rounded border px-2 py-2 text-sm"
          value={top}
          onChange={(e) => {
            const n = Number(e.target.value);
            if (!Number.isNaN(n)) setTop(n);
          }}
        >
          {[10, 20, 30, 50].map((n) => (
            <option key={n} value={n}>{n}</option>
          ))}
        </select>
        <span className="text-sm">items</span>
      </div>

      {fetchState === 'loading' && <div className="text-sm">Loading recommendations…</div>}
      {fetchState === 'error' && <div className="text-sm text-red-600">{error}</div>}
      {fetchState === 'done' && !hasResults && (
        <div className="text-sm text-gray-600">No recommendations yet.</div>
      )}

      {hasResults && (
        <div className="grid gap-4 grid-cols-[repeat(auto-fill,minmax(220px,1fr))]">
          {items.map((it) => (
            <Link
              key={it.itemId}
              to={`/items/${it.itemId}`}
              className="border rounded-2xl p-3 hover:shadow focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <img
                className="w-full h-40 object-cover rounded"
                src={it.thumbnailPath || 'https://placehold.co/800?text=Coming+Soon&font=roboto'}
                alt={it.name ?? 'item image'}
                loading="lazy"
              />
              <div className="mt-2 font-medium truncate">{it.name}</div>
              <div className="text-sm opacity-70">€{it.price.toFixed(2)}</div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
