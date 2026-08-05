import { useState, useEffect } from 'react';
import { createItem } from '../api/items';
import { createAuction } from '../api/auctions';
import { listCategories } from '../api/categories';
import { useAuth } from '../store/authstore';
import type { Category } from '../api/types';

export default function SellItemForm() {
  const { user } = useAuth();
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [categoryId, setCategoryId] = useState<number | null>(null);
  const [desc, setDesc] = useState('');
  const [ok, setOk] = useState<string | null>(null);
  const [categories, setCategories] = useState<Category[]>([]);
  const [withAuction, setWithAuction] = useState(false);

  const [address, setAddress] = useState('');
  const [lat, setLat] = useState<number | null>(null);
  const [lon, setLon] = useState<number | null>(null);
  const [geoStatus, setGeoStatus] = useState<string | null>(null);

  const [auctionStart, setAuctionStart] = useState('');
  const [auctionEnd, setAuctionEnd] = useState('');
  const [auctionStartingPrice, setAuctionStartingPrice] = useState('');

  useEffect(() => {
    listCategories().then(setCategories);
  }, []);

  async function geocodeAddress(q: string) {
    setGeoStatus('Geocoding address…');
    try {
      const url = new URL('https://nominatim.openstreetmap.org/search');
      url.searchParams.set('q', q);
      url.searchParams.set('format', 'json');
      url.searchParams.set('limit', '1');

      const res = await fetch(url.toString(), {
        headers: {
          'Accept': 'application/json',
        },
      });
      const data: Array<{ lat: string; lon: string }> = await res.json();
      if (data.length) {
        const { lat, lon } = data[0];
        setLat(Number(lat));
        setLon(Number(lon));
        setGeoStatus('Address located ✅');
        return { lat: Number(lat), lon: Number(lon) };
      } else {
        setGeoStatus('Could not find that address.');
        return null;
      }
    } catch (e) {
      setGeoStatus(`Geocoding failed. You can still submit without a map: ${e}`);
      return null;
    }
  }

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;

    let coords = (lat != null && lon != null) ? { lat, lon } : null;
    if (address && !coords) {
      coords = await geocodeAddress(address);
    }

    const it = await createItem({
      name,
      price: Number(price),
      categoryId: categoryId ?? 1,
      description: desc,
      sellerId: user.userId,
      isActive: true,
      address: address || undefined,
      latitude: coords?.lat,
      longitude: coords?.lon,
    });

    if (withAuction) {
      await createAuction({
        itemId: it.itemId,
        status: 0,
        startingAt: auctionStart || new Date().toISOString(),
        endingAt:
          auctionEnd || new Date(Date.now() + 7 * 24 * 3600 * 1000).toISOString(),
        startingPrice: Number(auctionStartingPrice) || 0,
      });
      setOk(`Created item #${it.itemId} with auction`);
    } else {
      setOk(`Created item #${it.itemId}`);
    }
  };

  return (
    <form onSubmit={submit} className="max-w-xl mx-auto p-6 space-y-3">
      <h1 className="text-2xl font-bold">Sell an item</h1>

      <input
        className="w-full border p-2 rounded"
        placeholder="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />

      <input
        className="w-full border p-2 rounded"
        placeholder="Buy Now Price"
        value={price}
        onChange={(e) => setPrice(e.target.value)}
      />

      <select
        className="w-full border p-2 rounded"
        value={categoryId ?? ''}
        onChange={(e) => setCategoryId(Number(e.target.value))}
      >
        <option value="" disabled>Select a category</option>
        {categories.map((c) => (
          <option key={c.categoryId} value={c.categoryId}>{c.name}</option>
        ))}
      </select>

      <textarea
        className="w-full border p-2 rounded"
        placeholder="Description"
        value={desc}
        onChange={(e) => setDesc(e.target.value)}
      />

      <input
        className="w-full border p-2 rounded"
        placeholder="Pickup address (street, city, country)"
        value={address}
        onChange={(e) => setAddress(e.target.value)}
      />
      {geoStatus && <div className="text-sm opacity-70">{geoStatus}</div>}

      <label className="flex items-center space-x-2">
        <input
          type="checkbox"
          checked={withAuction}
          onChange={(e) => setWithAuction(e.target.checked)}
        />
        <span>Create an auction</span>
      </label>

      {withAuction && (
        <div className="space-y-2">
          <label className="text-sm text-gray-500">Auction Start Time
            <input
              type="datetime-local"
              className="w-full border p-2 rounded"
              value={auctionStart}
              onChange={(e) => setAuctionStart(e.target.value)}
            />
          </label>
          <label className="text-sm text-gray-500">Auction End Time
            <input
              type="datetime-local"
              className="w-full border p-2 rounded"
              value={auctionEnd}
              onChange={(e) => setAuctionEnd(e.target.value)}
            />
          </label>
          <input
            className="w-full border p-2 rounded"
            placeholder="Auction Starting Price"
            value={auctionStartingPrice}
            onChange={(e) => setAuctionStartingPrice(e.target.value)}
          />
        </div>
      )}

      <button className="bg-black text-white px-4 py-2 rounded">Create</button>
      {ok && <div className="text-green-700">{ok}</div>}
    </form>
  );
}
