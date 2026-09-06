
import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAdminStats, exportAdminStats } from '../../api/endpoints/adminstats';
import type { AdminStats } from '../../api/types';

export default function AdminDashboard() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState<AdminStats | null>(null);

  const [sellerId, setSellerId] = useState<string>(''); 
  const [format, setFormat] = useState<'json' | 'xml'>('json');
  const [start, setStart] = useState<string>(''); 
  const [end, setEnd] = useState<string>('');
  const [exporting, setExporting] = useState(false);
  const [exportError, setExportError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const data = await getAdminStats();
        setStats(data);
      } catch (e : unknown) {
        if (e instanceof Error) {
          setError(e.message);
        } else {
          setError('Failed to load dashboard');
        }
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const cards = useMemo(() => {
    if (!stats) return [];
    return [
      { title: 'Items', value: stats.itemCount },
      { title: 'Active Items', value: stats.activeItemCount },
      { title: 'Users', value: stats.usersCount },
      { title: 'Sellers', value: stats.sellersCount },
      { title: 'Buyers', value: stats.buyersCount },
      { title: 'Categories', value: stats.categoriesCount },
      { title: 'Buy-Now Orders', value: stats.buyNowOrdersCount },
      { title: 'Auction Orders', value: stats.auctionOrdersCount },
      { title: 'Active Auctions', value: stats.activeAuctionCount },
      { title: 'Completed Auctions', value: stats.completedAuctionCount },
    ];
  }, [stats]);

  const handleExport = async () => {
    try {
      setExportError(null);
      setExporting(true);

      const toISO = (s: string) => (s ? new Date(s).toISOString() : undefined);

      const payload = await exportAdminStats({
        format,
        sellerId: sellerId ? Number(sellerId) : undefined,
        start: toISO(start),
        end: toISO(end),
      });

      const data = format === 'json' && typeof payload === 'object'? JSON.stringify(payload, null, 2): (payload as unknown as string);
      const blob = new Blob([data], { type: format === 'json' ? 'application/json' : 'application/xml' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `admin-stats.${format}`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } catch (e : unknown) {
      if (e instanceof Error) {
        setExportError(e.message);
      } else {
        setExportError('Export failed');
      }
    } finally {
      setExporting(false);
    }
  };

  if (loading) return <div className="p-6">Loading…</div>;
  if (error) return <div className="p-6 text-red-600">{error}</div>;

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-2xl font-bold">Admin Dashboard</h1>

      <div className="grid gap-4 grid-cols-[repeat(auto-fill,minmax(220px,1fr))]">
        {cards.map((c) => (
          <StatCard key={c.title} title={c.title} value={c.value} />
        ))}
      </div>

      <section className="border rounded p-4">
        <h2 className="text-lg font-semibold mb-3">Export Stats</h2>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div>
            <label className="block text-xs font-medium mb-1">Seller ID (optional)</label>
            <input
              type="number"
              inputMode="numeric"
              value={sellerId}
              onChange={(e) => setSellerId(e.target.value)}
              className="w-full rounded border px-3 py-2 text-sm"
              placeholder="e.g. 42"
            />
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">Start (optional)</label>
            <input
              type="datetime-local"
              value={start}
              onChange={(e) => setStart(e.target.value)}
              className="w-full rounded border px-3 py-2 text-sm"
            />
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">End (optional)</label>
            <input
              type="datetime-local"
              value={end}
              onChange={(e) => setEnd(e.target.value)}
              className="w-full rounded border px-3 py-2 text-sm"
            />
          </div>

          <div>
            <label className="block text-xs font-medium mb-1">Format</label>
            <select
              className="w-full rounded border px-2 py-2 text-sm"
              value={format}
              onChange={(e) => setFormat(e.target.value as 'json' | 'xml')}
            >
              <option value="json">JSON</option>
              <option value="xml">XML</option>
            </select>
          </div>
        </div>

        <div className="mt-3 flex items-center gap-3">
          <button
            onClick={handleExport}
            disabled={exporting}
            className="rounded border px-3 py-2 text-sm hover:bg-gray-50 disabled:opacity-50"
          >
            {exporting ? 'Exporting…' : 'Export'}
          </button>
          {exportError && <div className="text-sm text-red-600">{exportError}</div>}
        </div>
      </section>

      <div className="grid gap-4 sm:grid-cols-2">
        <QuickLink to="/admin/users" label="Manage Users" />
        <QuickLink to="/admin/categories" label="Manage Categories" />
      </div>
    </div>
  );
}

function StatCard({ title, value }: { title: string; value: number }) {
  return (
    <div className="border rounded p-4">
      <div className="text-sm opacity-70">{title}</div>
      <div className="text-3xl font-semibold">{value}</div>
    </div>
  );
}

function QuickLink({ to, label }: { to: string; label: string }) {
  return (
    <Link to={to} className="border rounded p-4 hover:shadow">
      <div className="text-lg font-medium">{label}</div>
      <div className="text-sm opacity-70">Go to {label}</div>
    </Link>
  );
}
