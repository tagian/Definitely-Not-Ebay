/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { listCategories } from "../../api/endpoints/categories";
import type { Category } from "../../api/types";

const BROWSE_ITEMS_PATH = "/browse";

type Card = { id: number | string; name: string; thumb?: string | null };

export default function BrowseByCategory() {
  const [cards, setCards] = useState<Card[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const cats: Category[] = await listCategories();
        if (cancelled) return;
        const mapped: Card[] = cats
          .map((c: any) => ({
            id: (c as any).categoryId ?? (c as any).id,
            name: c.name ?? "Category",
            thumb: (c as any).thumbnailPath ?? null,
          }))
          .sort((a, b) => a.name.localeCompare(b.name));
        setCards(mapped);
      } catch (e: any) {
        if (!cancelled) setError(e?.message ?? "Failed to load categories");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto p-6">
        <div className="mb-4 h-7 w-56 rounded bg-gray-100 animate-pulse" />
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
          {Array.from({ length: 10 }).map((_, i) => (
            <div key={i} className="overflow-hidden rounded-2xl border">
              <div className="aspect-square bg-gray-100 animate-pulse" />
              <div className="p-2">
                <div className="h-4 w-24 rounded bg-gray-100 animate-pulse" />
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-3xl mx-auto p-6 text-sm text-red-600">{error}</div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Browse by Category</h1>

      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
        {cards.map((c) => (
          <Link
            key={c.id}
            to={`${BROWSE_ITEMS_PATH}?categoryId=${encodeURIComponent(
              String(c.id)
            )}`}
            className="group block rounded-2xl overflow-hidden border hover:shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            aria-label={`Open ${c.name}`}
          >
            <div className="aspect-square bg-gray-50">
              {c.thumb ? (
                <img
                  src={String(c.thumb)}
                  alt={c.name}
                  className="h-full w-full object-cover"
                  loading="lazy"
                />
              ) : (
                <div className="h-full w-full flex items-center justify-center text-gray-400 text-xs">
                  No image
                </div>
              )}
            </div>
            <div className="p-2 text-sm font-medium truncate">{c.name}</div>
          </Link>
        ))}
      </div>
    </div>
  );
}
