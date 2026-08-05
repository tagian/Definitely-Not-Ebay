import { useEffect, useMemo, useState } from 'react';
import { listItems, type ListItemsParams } from '../api/items';
import type { Item } from '../api/types';
import { Link, useSearchParams } from 'react-router-dom'; // ← added


const ORDER_FIELDS = [
  { label: 'Newest', value: 'createdAt' },
  { label: 'Price', value: 'price' },
];

export default function BrowseItems() {
  const [items, setItems] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [search, setSearch] = useState('');
  const [orderBy, setOrderBy] = useState<string>('createdAt');
  const [orderDir, setOrderDir] = useState<'asc' | 'desc'>('desc');
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(20);

  const [sp, setSp] = useSearchParams();
  const categoryIdParam = sp.get('categoryId');
  const categoryId = categoryIdParam ? Number(categoryIdParam) : undefined;


  const [debouncedSearch, setDebouncedSearch] = useState(search);
  useEffect(() => {
    const t = setTimeout(() => setDebouncedSearch(search), 350);
    return () => clearTimeout(t);
  }, [search]);

  const params: ListItemsParams = useMemo(
    () => ({
      search: debouncedSearch || undefined,
      orderBy,
      orderDir,
      page,
      pageSize,
      categoryId
    }),
    [debouncedSearch, orderBy, orderDir, page, pageSize, categoryId]
  );

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    listItems<Item[]>(params)
      .then((res) => {
        if (cancelled) return;
        setItems(Array.isArray(res) ? res : []);
      })
      .catch((e) => {
        if (cancelled) return;
        setError(e?.message ?? 'Failed to load items');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [params]);

  const hasNext = items.length === pageSize;
  const canPrev = page > 1;

  return (
    <div className="max-w-6xl mx-auto p-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between mb-4">
        <div className="flex-1 flex gap-2">
          <div className="flex-1">
            <label className="block text-xs font-medium mb-1">Search</label>
            <input
              value={search}
              onChange={(e) => {
                setPage(1); // reset page on new search
                setSearch(e.target.value);
              }}
              placeholder="Search items…"
              className="w-full rounded border px-3 py-2 text-sm"
            />
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">Order by</label>
            <select
              value={orderBy}
              onChange={(e) => {
                setPage(1);
                setOrderBy(e.target.value);
              }}
              className="rounded border px-2 py-2 text-sm"
            >
              {ORDER_FIELDS.map((o) => (
                <option key={o.value} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">Direction</label>
            <select
              value={orderDir}
              onChange={(e) => {
                setPage(1);
                setOrderDir(e.target.value as 'asc' | 'desc');
              }}
              className="rounded border px-2 py-2 text-sm"
            >
              <option value="desc">Desc</option>
              <option value="asc">Asc</option>
            </select>
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">Page size</label>
            <select
              value={pageSize}
              onChange={(e) => {
                setPage(1);
                setPageSize(Number(e.target.value));
              }}
              className="rounded border px-2 py-2 text-sm"
            >
              {[10, 20, 30, 50].map((n) => (
                <option key={n} value={n}>
                  {n}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="flex gap-2">
          <button
            className="rounded border px-3 py-2 text-sm disabled:opacity-50"
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={!canPrev || loading}
          >
            ← Prev
          </button>
          <div className="px-2 py-2 text-sm">Page {page}</div>
          <button
            className="rounded border px-3 py-2 text-sm disabled:opacity-50"
            onClick={() => setPage((p) => p + 1)}
            disabled={!hasNext || loading}
          >
            Next →
          </button>
        </div>
      </div>

      {categoryId && (
        <div className="mb-4 text-sm">
          Filtering by category <strong>#{categoryId}</strong>{' '}
          <button
            onClick={() => {
              sp.delete('categoryId');
              setSp(sp);
            }}
            className="ml-2 px-2 py-1 border rounded hover:bg-gray-50"
          >
            Clear
          </button>
        </div>
      )}

      {loading && <div className="p-3 text-sm">Loading…</div>}
      {error && <div className="p-3 text-sm text-red-600">{error}</div>}

      {!loading && !error && items.length === 0 && (
        <div className="p-3 text-sm text-gray-600">No items found.</div>
      )}

      <div className="grid gap-4 grid-cols-[repeat(auto-fill,minmax(220px,1fr))]">
        {items.map((it) => (
          <Link
            key={it.itemId}
            to={`/items/${it.itemId}`}
            className="border rounded-2xl p-3 hover:shadow focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            <img
              className="w-full h-40 object-cover rounded"
              src={
                it.thumbnailPath ||
                'https://placehold.co/800?text=Coming+Soon&font=roboto'
              }
              alt={it.name ?? 'item image'}
              loading="lazy"
            />
            <div className="mt-2 font-medium truncate">{it.name}</div>
            <div className="text-sm opacity-70">€{it.price.toFixed(2)}</div>
          </Link>
        ))}
      </div>
    </div>
  );
}
