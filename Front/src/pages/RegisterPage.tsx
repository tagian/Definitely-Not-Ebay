/* eslint-disable @typescript-eslint/no-explicit-any */
import { type FormEvent, useMemo, useState } from "react";
import { register} from "../api/auth";
import type { Role } from "../api/types";

export default function RegisterPage() {

  const [form, setForm] = useState({
    name: "",
    email: "",
    password: "",
    confirmPassword: "",
    phone: "",
    address: "",
    city: "",
    postalCode: "",
    country: "",
    role: "Buyer" as Role,
  });

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const passwordsMismatch = useMemo(
    () =>
      form.password.length > 0 &&
      form.confirmPassword.length > 0 &&
      form.password !== form.confirmPassword,
    [form.password, form.confirmPassword]
  );

  function set<K extends keyof typeof form>(key: K, val: (typeof form)[K]) {
    setForm((f) => ({ ...f, [key]: val }));
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (passwordsMismatch) {
      setError("Passwords don’t match.");
      return;
    }

    setSubmitting(true);
    try {
      const payload = {
        name: form.name.trim(),
        email: form.email.trim(),
        password: form.password,
        phone: form.phone.trim(),
        address: form.address.trim(),
        city: form.city.trim(),
        postalCode: form.postalCode.trim(),
        country: form.country.trim(),
        role: form.role,
      };


      const message = await register(payload); 

      setSuccess(
        typeof message === "string"
          ? message
          : "Account created successfully."
      );

    } catch (err: any) {
      const msg = String(err?.message ?? "Registration failed");

      if (msg.startsWith("409") || msg.includes(" 409 ") || /Conflict/i.test(msg)) {
        setError("An account with this email already exists.");
      } else {
        setError(msg);
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="max-w-sm mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Register</h1>

      <form onSubmit={onSubmit} className="space-y-3">
        <input
          className="w-full border p-2 rounded"
          required
          placeholder="name"
          value={form.name}
          onChange={(e) => set("name", e.target.value)}
        />
        <input
          className="w-full border p-2 rounded"
          required
          type="email"
          placeholder="email"
          value={form.email}
          onChange={(e) => set("email", e.target.value)}
        />

        <input
          className="w-full border p-2 rounded"
          required
          type="password"
          placeholder="password"
          value={form.password}
          onChange={(e) => set("password", e.target.value)}
        />
        <div>
          <input
            className={`w-full border p-2 rounded ${
              passwordsMismatch ? "border-red-500" : ""
            }`}
            type="password"
            required
            placeholder="retype password"
            value={form.confirmPassword}
            onChange={(e) => set("confirmPassword", e.target.value)}
          />
          {passwordsMismatch && (
            <div className="text-red-600 text-xs mt-1">
              Passwords do not match.
            </div>
          )}
        </div>

        <input
          className="w-full border p-2 rounded"
          placeholder="phone"
          required
          value={form.phone}
          onChange={(e) => set("phone", e.target.value)}
        />
        <input
          className="w-full border p-2 rounded"
          placeholder="address"
          required
          value={form.address}
          onChange={(e) => set("address", e.target.value)}
        />
        <div className="grid grid-cols-2 gap-3">
          <input
            className="w-full border p-2 rounded"
            placeholder="city"
            required
            value={form.city}
            onChange={(e) => set("city", e.target.value)}
          />
          <input
            className="w-full border p-2 rounded"
            placeholder="postal code"
            required
            value={form.postalCode}
            onChange={(e) => set("postalCode", e.target.value)}
          />
        </div>
        <input
          className="w-full border p-2 rounded"
          placeholder="country"
          required
          value={form.country}
          onChange={(e) => set("country", e.target.value)}
        />

        <div className="flex gap-4 items-center pt-2">
          <label className="flex items-center gap-2">
            <input
              
              type="radio"
              name="role"
              value="Buyer"
              checked={form.role === "Buyer"}
              onChange={() => set("role", "Buyer")}
            />
            <span>Buyer</span>
          </label>
          <label className="flex items-center gap-2">
            <input
              type="radio"
              name="role"
              value="Seller"
              checked={form.role === "Seller"}
              onChange={() => set("role", "Seller")}
            />
            <span>Seller</span>
          </label>
        </div>

        {error && <div className="text-red-600 text-sm">{error}</div>}
        {success && <div className="text-green-700 text-sm">{success}</div>}

        <button
          className="bg-black text-white px-4 py-2 rounded disabled:opacity-60"
          disabled={submitting || passwordsMismatch}
        >
          {submitting ? "Creating account..." : "Create account"}
        </button>

        <div className="text-sm text-gray-600">
          Already have an account?{" "}
          <a href="/login" className="underline">
            Sign in
          </a>
        </div>
      </form>
    </div>
  );
}
