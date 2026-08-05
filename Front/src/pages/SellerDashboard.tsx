/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Item, Auction, Order } from '../api/types';
import { listMyItems, updateItem } from '../api/items';
import { listMyAuctions, updateAuction } from '../api/auctions';
import { listMyOrders } from '../api/orders';

const AuctionStatus: Record<number, string> = {
  0: "Pending",
  1: "Active",
  2: "Completed",
  3: "Cancelled",
};

export default function SellerDashboard() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [items, setItems] = useState<Item[]>([]);
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [orders, setOrders] = useState<Order[]>([]);

  useEffect(() => {
  (async () => {
    try {
      const [myItems, myAuctions, myOrders] = await Promise.all([
        listMyItems().catch((e: any) => {
          if (e.status === 404) return []; 
          throw e;
        }),
        listMyAuctions().catch((e: any) => {
          if (e.status === 404) return [];
          throw e;
        }),
        listMyOrders().catch((e: any) => {
          if (e.status === 404) return [];
          throw e;
        }),
      ]);
      setItems(myItems);
      setAuctions(myAuctions);
      setOrders(myOrders);
    } catch (e: any) {
      setError(e.message || 'Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  })();
}, []);


  const stats = useMemo(() => {
    const activeItems = items.filter(i => (i as any).isActive).length;
    const liveAuctions = auctions.filter(a => (a as any).status === 1).length; 
    return {
      items: items.length,
      activeItems,
      auctions: auctions.length,
      liveAuctions,
      orders: orders.length,

    };
  }, [items, auctions, orders]);

  if (loading) return <div className="p-6">Loading…</div>;
  if (error) return <div className="p-6 text-red-600">{error}</div>;

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Seller Dashboard</h1>
        <Link to="/sell" className="inline-flex items-center gap-2 rounded border px-4 py-2 hover:shadow">
          <span className="font-medium">Sell new</span>
        </Link>
      </div>

      
      <div className="grid gap-4 grid-cols-[repeat(auto-fill,minmax(220px,1fr))]">
        <StatCard title="My Items" value={stats.items} />
        <StatCard title="Active Items" value={stats.activeItems} />
        <StatCard title="My Auctions" value={stats.auctions} />
        <StatCard title="Live Auctions" value={stats.liveAuctions} />
        <StatCard title="My Orders" value={stats.orders} />
      </div>

      
      <div className="grid gap-4 sm:grid-cols-2">
        <QuickLink to="/sell" label="Sell new" />
        <QuickLink to="/seller/orders" label="View all Orders" />
      </div>

      
      <Section title="My Items" action={<Link to="/seller/items" className="text-sm underline">View all</Link>}>
        <ItemsTable items={items.slice(0, 5)} onQuickEdit={handleQuickItemEdit} />
      </Section>

      <Section title="My Auctions" action={<Link to="/seller/auctions" className="text-sm underline">View all</Link>}>
        <AuctionsTable auctions={auctions.slice(0, 5)} onQuickEdit={handleQuickAuctionEdit} />
      </Section>

      <Section title="My Orders" action={<Link to="/seller/orders" className="text-sm underline">View all</Link>}>
        <OrdersTable orders={orders.slice(0, 5)} />
      </Section>
    </div>
  );

  // async function handleQuickItemEdit(itemId: number, patch: Partial<Item>) {
  //   try {
  //     await updateItem(itemId, patch); 
  //     setItems(cur => cur.map(i => (i.itemId === itemId ? { ...i, ...patch } : i)));
  //   } catch (e: any) {
  //     alert(e.message || 'Failed to update item');
  //   }
  // }

  async function handleQuickItemEdit(itemId: number, patch: Partial<Item>) {
  try {
    const current = items.find(i => i.itemId === itemId);
    if (!current) {
      alert('Item not found in state');
      return;
    }
    const fullItem: Item = { ...current, ...patch }; 
    await updateItem(itemId, fullItem as unknown as Item);
    setItems(cur => cur.map(i => (i.itemId === itemId ? fullItem : i)));
  } catch (e: any) {
    alert(e.message || 'Failed to update item');
  }
}


  // async function handleQuickAuctionEdit(auctionId: number, patch: Partial<Auction>) {
  //   try {
  //     await updateAuction(auctionId, patch);
  //     setAuctions(cur => cur.map(a => (a.auctionId === auctionId ? { ...a, ...patch } : a)));
  //   } catch (e: any) {
  //     alert(e.message || 'Failed to update auction');
  //   }
  // }

  async function handleQuickAuctionEdit(auctionId: number, patch: Partial<Auction>) {
  try {
    const current = auctions.find(a => a.auctionId === auctionId);
    if (!current) {
      alert('Auction not found in state');
      return;
    }
    const fullAuction: Auction = { ...current, ...patch };
    await updateAuction(auctionId, fullAuction as unknown as Auction);

    setAuctions(cur => cur.map(a => (a.auctionId === auctionId ? fullAuction : a)));
  } catch (e: any) {
    alert(e.message || 'Failed to update auction');
  }
}

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

function Section({ title, children, action }: { title: string; children: any; action?: any }) {
  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">{title}</h2>
        {action}
      </div>
      <div className="border rounded overflow-hidden">{children}</div>
    </div>
  );
}


function ItemsTable({ items, onQuickEdit }: { items: Item[]; onQuickEdit: (id: number, patch: Partial<Item>) => void }) {
  if (!items.length) return <EmptyState message="No items yet" cta={<Link to="/sell" className="underline">Sell new</Link>} />;
  return (
    <table className="w-full text-sm">
      <thead className="bg-gray-50">
        <tr>
          <Th>Name</Th>
          <Th>Status</Th>
          <Th>Price</Th>
          <Th>Actions</Th>
        </tr>
      </thead>
      <tbody>
        {items.map((i) => (
          <tr key={i.itemId} className="border-t">
            <Td>{(i as any).name || i.itemId}</Td>
            <Td>{(i as any).isActive ? 'Active' : 'Draft'}</Td>
            <Td>{(i as any).price != null ? `$${(i as any).price}` : '—'}</Td>
            <Td>
              <div className="flex gap-2">
                <Link to={`/items/${i.itemId}`} className="underline">View</Link>
                <Link to={`/items/${i.itemId}/edit`} className="underline">Edit</Link>
                <button
                  className="underline"
                  onClick={() => onQuickEdit(i.itemId, { isActive: !(i as any).isActive } as Partial<Item>)}
                >
                  {(i as any).isActive ? 'Deactivate' : 'Activate'}
                </button>
              </div>
            </Td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function AuctionsTable({ auctions, onQuickEdit }: { auctions: Auction[]; onQuickEdit: (id: number, patch: Partial<Auction>) => void }) {
  if (!auctions.length) return <EmptyState message="No auctions yet" cta={<Link to="/sell" className="underline">Start one</Link>} />;
  return (
    <table className="w-full text-sm">
      <thead className="bg-gray-50">
        <tr>
          <Th>Title</Th>
          <Th>Status</Th>
          <Th>Ends</Th>
          <Th>Actions</Th>
        </tr>
      </thead>
      <tbody>
        {auctions.map((a) => (
          <tr key={a.auctionId} className="border-t">
            <Td>{(a as any).title || a.auctionId}</Td>
            <Td>{AuctionStatus[(a as any).status] ?? '—'}</Td>
            <Td>{(a as any).endAt ? new Date((a as any).endAt).toLocaleString() : '—'}</Td>
            <Td>
              <div className="flex gap-2">
                <Link to={`/auctions/${a.auctionId}`} className="underline">View</Link>
                <Link to={`/auctions/${a.auctionId}/edit`} className="underline">Edit</Link>
                <button
                    className="underline"
                    onClick={() => onQuickEdit(
                        a.auctionId,
                        { status: (a as any).status === 1 ? 0 : 1 } as Partial<Auction>
                    )}
                >
                  {(a as any).status === 1 ? 'Set Pending' : 'Activate'}
                </button>
              </div>
            </Td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function OrdersTable({ orders }: { orders: Order[] }) {
  if (!orders.length) return <EmptyState message="No orders yet" cta={<Link to="/seller/orders" className="underline">Browse orders</Link>} />;
  return (
    <table className="w-full text-sm">
      <thead className="bg-gray-50">
        <tr>
          <Th>Order #</Th>
          <Th>Buyer</Th>
          <Th>Total</Th>
          <Th>Placed</Th>
          <Th>BuyNow</Th>
        </tr>
      </thead>
      <tbody>
        {orders.map((o) => (
          <tr key={o.orderId} className="border-t">
            <Td>
              <Link to={`/orders/${o.orderId}`} className="underline">{o.orderId}</Link>
            </Td>
            <Td>{(o as any).buyerId || '—'}</Td>
            <Td>{(o as any).total != null ? `$${(o as any).total}` : '—'}</Td>
            <Td>{(o as any).dateCreated ? new Date((o as any).dateCreated).toLocaleString() : '—'}</Td>
            <Td>{(o as any).buyNow ? 'Yes' : 'No'}</Td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function Th({ children }: { children: any }) {
  return <th className="text-left p-3 text-xs uppercase tracking-wide opacity-70">{children}</th>;
}
function Td({ children }: { children: any }) {
  return <td className="p-3 align-middle">{children}</td>;
}

function EmptyState({ message, cta }: { message: string; cta?: any }) {
  return (
    <div className="p-6 text-center">
      <div className="mb-2">{message}</div>
      {cta}
    </div>
  );
}
