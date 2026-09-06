/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useMemo, useState, type FormEvent } from "react";
import { me, updateMe } from "../../api/endpoints/auth";
import { useAuth } from "../../store/authstore";
import type { User} from "../../api/types";


export default function ProfilePage() {
  const { token, user, setAuth } = useAuth();
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const [form, setForm] = useState({
    name: "",
    email: "",
    phone: "",
    address: "",
    city: "",
    postalCode: "",
    country: ""
  });

  //load current profile
  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const profile = await me(); 
        if (!mounted) return;
        setForm({
          name: profile.name ?? "",
          email: profile.email ?? "",
          phone: profile.phone ?? "",
          address: profile.address ?? "",
          city: profile.city ?? "",
          postalCode: profile.postalCode ?? "",
          country: profile.country ?? ""
        });
      } catch (err: any) {
        if (!mounted) return;
        setError(err?.message ?? "Failed to load profile");
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => { mounted = false; };
  }, []);

  function set<K extends keyof typeof form>(key: K, val: (typeof form)[K]) {
    setSuccess(null); // clear stale success on edits
    setForm((f) => ({ ...f, [key]: val }));
  }

  const canSubmit = useMemo(() => {
    return !!form.name.trim() &&
           !!form.email.trim() &&
           !!form.phone.trim() &&
           !!form.address.trim() &&
           !!form.city.trim() &&
           !!form.postalCode.trim() &&
           !!form.country.trim()
  }, [form]);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!canSubmit) {
      setError("Please fill all required fields.");
      return;
    }

    setSubmitting(true);
    try {
      const payload = {
        userId: user?.userId,
        name: form.name.trim(),
        email: form.email.trim(),
        phone: form.phone.trim(),
        address: form.address.trim(),
        city: form.city.trim(),
        postalCode: form.postalCode.trim(),
        country: form.country.trim(),
      };

      const res = await updateMe(payload); // POST /api/Users/me

      // If server returns updated user JSON, use it; otherwise merge locally
      const updatedUser = (res as User) ?? { ...(user as User), ...payload };

      // Keep the same token, just refresh the user in the store
      setAuth(token, updatedUser);

      setSuccess("Profile updated successfully.");
    } catch (err: any) {
      const msg = String(err?.message ?? "Update failed");
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) {
    return (
      <div className="max-w-sm mx-auto p-6 text-sm text-gray-600">
        Loading profile…
      </div>
    );
  }

  return (
    <div className="max-w-sm mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">My Profile</h1>

      <form onSubmit={onSubmit} className="space-y-3">
        <input
          className="w-full border p-2 rounded"
          placeholder="name"
          value={form.name}
          onChange={(e) => set("name", e.target.value)}
          required
        />
        <input
          className="w-full border p-2 rounded"
          type="email"
          placeholder="email"
          value={form.email}
          onChange={(e) => set("email", e.target.value)}
          required
        />
        <input
          className="w-full border p-2 rounded"
          placeholder="phone"
          value={form.phone}
          onChange={(e) => set("phone", e.target.value)}
          required
        />
        <input
          className="w-full border p-2 rounded"
          placeholder="address"
          value={form.address}
          onChange={(e) => set("address", e.target.value)}
          required
        />
        <div className="grid grid-cols-2 gap-3">
          <input
            className="w-full border p-2 rounded"
            placeholder="city"
            value={form.city}
            onChange={(e) => set("city", e.target.value)}
            required
          />
          <input
            className="w-full border p-2 rounded"
            placeholder="postal code"
            value={form.postalCode}
            onChange={(e) => set("postalCode", e.target.value)}
            required
          />
        </div>
        <input
          className="w-full border p-2 rounded"
          placeholder="country"
          value={form.country}
          onChange={(e) => set("country", e.target.value)}
          required
        />


        {error && <div className="text-red-600 text-sm">{error}</div>}
        {success && <div className="text-green-700 text-sm">{success}</div>}

        <button
          className="bg-black text-white px-4 py-2 rounded disabled:opacity-60"
          disabled={!canSubmit || submitting}
        >
          {submitting ? "Saving…" : "Save changes"}
        </button>
      </form>
    </div>
  );
}
