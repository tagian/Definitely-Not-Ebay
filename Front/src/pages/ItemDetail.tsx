import { useEffect, useMemo, useState } from 'react';
import { useParams} from 'react-router-dom';
import { getItem} from '../api/items';
import { getActiveAuction } from '../api/auctions';
import { createBid, getBids } from '../api/bids';
import type { Item, Auction, Bid } from '../api/types';
import { useAuth } from '../store/authstore';
import { createOrder } from '../api/orders';

export default function ItemDetail() {
    const {id} = useParams();
    const itemId = Number(id);
    const [item, setItem] = useState < Item | null > (null);
    const [auction, setAuction] = useState < Auction | null > (null);
    const [bid, setBid] = useState('');
    const [bids, setBids] = useState<Bid[]>([]);
    const [status, setStatus] = useState < string > ('');
    const {user} = useAuth();

    const [viewLat, setViewLat] = useState<number | null>(null);
    const [viewLon, setViewLon] = useState<number | null>(null);
    
    useEffect(() => {
    (async () => {
      const it = await getItem(itemId);
      setItem(it as Item);

      if (it?.latitude != null && it?.longitude != null) {
        setViewLat(it.latitude as number);
        setViewLon(it.longitude as number);
      } else if (it?.address) {
        const url = new URL('https://nominatim.openstreetmap.org/search');
        url.searchParams.set('q', it.address);
        url.searchParams.set('format', 'json');
        url.searchParams.set('limit', '1');
        try {
          const res = await fetch(url.toString(), { headers: { 'Accept': 'application/json' } });
          const data: Array<{ lat: string; lon: string }> = await res.json();
          if (data.length) {
            setViewLat(Number(data[0].lat));
            setViewLon(Number(data[0].lon));
          }
        } catch { /* ignore */ }
      }
    })();
    }, [itemId]);
    
    const highestBid = useMemo(() => {
        if (!bids?.length) return null;
        return bids.reduce((max, b) => (b.hit! > max! ? b.hit : max), bids[0].hit);
    }, [bids]);

    const placeBid = async () => {
        if (!auction || !user) return;
        const value = Number(bid);
        
        if (!Number.isFinite(value) || value <= 0) {
            setStatus('Enter a valid positive bid amount.');
            return;
        }
    
        if (highestBid != null && value <= highestBid) {
            setStatus(`Your bid must be greater than the current highest (€${highestBid.toFixed(2)}).`);
            return;
        }


        await createBid({ bidderId: user.userId, hit: value, auctionId: auction.auctionId });
        setBid('');
        setStatus('Bid placed.');
    };



    useEffect(() => {
    (async () => {
        try {
            const a = await getActiveAuction(itemId); // returns Auction | null
            setAuction(a ?? null);
        } catch {
            setAuction(null);
        }
    })();
}, [itemId]);



    const buyNow = async () => {
        if (!item || !user) return;
        await createOrder({buyerId: user.userId, sellerId: item.sellerId, itemId: item.itemId, BuyNow: true});
        setStatus('Purchased. Conversation opened with seller.');
    };

    useEffect(() => {
    if (!auction?.auctionId) return;
    const poll = async () => {
        try {
            const a = await getActiveAuction(itemId);
            if (a) setAuction(a);
            const list = await getBids(auction.auctionId);
            setBids(list?.sort((x: Bid, y: Bid) => (y.hit! - x.hit!)) ?? []);
        } catch {
            // ignore
        }
    };
    const timer = window.setInterval(poll, 10_000); 
    poll();
    return () => {
        if (timer) clearInterval(timer);
    };
}, [auction?.auctionId, itemId]);

    if (!item) return <div className="p-6">Loading…</div>;

    const canBuy = user?.role === 'Buyer';
    const canBid = user?.role === 'Buyer' && !!auction;


    const mapIframe = (() => {
    if (viewLat == null || viewLon == null) return null;
    const lat = viewLat;
    const lon = viewLon;
    const dx = 0.005, dy = 0.003; 
    const bbox = `${lon - dx},${lat - dy},${lon + dx},${lat + dy}`;
    const src = `https://www.openstreetmap.org/export/embed.html?bbox=${bbox}&layer=mapnik&marker=${lat},${lon}`;
    const link = `https://www.openstreetmap.org/?mlat=${lat}&mlon=${lon}#map=15/${lat}/${lon}`;
    return (
      <div className="mt-4">
        <div className="text-sm opacity-70">Pickup location {item.address ? `(${item.address})` : ''}</div>
        <div className="rounded overflow-hidden border">
          <iframe
            title="item-location"
            width="100%"
            height="300"
            src={src}
          />
        </div>
        <a className="text-sm underline opacity-80" href={link} target="_blank" rel="noreferrer">
          View on OpenStreetMap
        </a>
      </div>
    );
  })();


    return (
        <div className="max-w-3xl mx-auto p-6 space-y-4">
                <img className="w-full rounded" src={item.thumbnailPath || 'https://placehold.co/800?text=Coming+Soon&font=roboto'} />
                <h1 className="text-3xl font-bold">{item.name}</h1>
                <p className="opacity-80">{item.description}</p>


                <div className="flex gap-3 items-end">
                    <div>
                    <div className="text-sm opacity-70">Buy now</div>
                    <div className="text-2xl font-semibold">€{item.price.toFixed(2)}</div>
                </div>
                {canBuy && (
                    <button onClick={buyNow} className="bg-black text-white px-4 py-2 rounded">
                    Buy now
                  </button>
                )}
                </div>

                  <div className="border rounded p-4">
        {!auction ? (
          <div className="text-sm opacity-80">
            No auction is currently linked to this product. Check back soon or make a buy‑now offer.
          </div>
        ) : (
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm opacity-70">Auction ends</div>
                <div className="font-medium">
                  {new Date(auction.endingAt).toLocaleString()}
                </div>
              </div>
              <div className="text-right">
                <div className="text-sm opacity-70">Current highest</div>
                <div className="text-xl font-semibold">
                  {highestBid != null ? `€${highestBid.toFixed(2)}` : '—'}
                </div>
              </div>
            </div>
            {canBid && (
            <div className="flex gap-2">
                <input
                  className="border p-2 rounded flex-1"
                  placeholder="Your bid (EUR)"
                  value={bid}
                  onChange={(e) => setBid(e.target.value)}
                  inputMode="decimal"
                />
                <button onClick={placeBid} className="border px-3 py-2 rounded">
                  Place bid
                </button>
              </div>
            )}
            <div>
              <div className="text-sm opacity-70 mb-1">Placed bids</div>
              {bids.length === 0 ? (
                <div className="text-sm opacity-80">No bids yet — be the first!</div>
              ) : (
                <ul className="divide-y border rounded">
                  {bids.map((b) => (
                    <li key={b.bidId} className="flex items-center justify-between p-2">
                      <span className="opacity-80 text-sm">Bidder #{b.bidderId}</span>
                      <span className="font-medium">€{b.hit!.toFixed(2)}</span>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        )}
      </div>

            {mapIframe}


            {status && <div className="text-green-700">{status}</div>}
        </div>
    );
}
