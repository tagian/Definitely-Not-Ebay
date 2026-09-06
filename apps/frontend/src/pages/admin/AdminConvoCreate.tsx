/* eslint-disable @typescript-eslint/no-explicit-any */
//devHelper

import { useState } from 'react';
import { useAuth } from '../../store/authstore';
import { createConversation} from '../../api/endpoints/conversation';
import { sendMessage } from '../../api/endpoints/messages';

export default function AdminCreateTool() {
  const { user } = useAuth();
  const [sellerId, setSellerId] = useState('');
  const [buyerId, setBuyerId] = useState('');

  const [firstMessage, setFirstMessage] = useState('Hello!');
  const [busy, setBusy] = useState(false);
  const [ok, setOk] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);

  if (!user || user.role !== 'Admin') return null;

  async function handleCreate() {
    setOk(null); setErr(null);
    const sId = Number(sellerId); const bId = Number(buyerId);
    if (!sId || !bId || !firstMessage.trim()) {
      setErr('Please fill all fields with valid numeric IDs and a non-empty message.');
      return;
    }
    setBusy(true);
    try {
      const created = await createConversation(sId, bId);
      if (!created) throw new Error('No conversation returned');
      await sendMessage({ senderId: user!.userId!, receipientId: sId, content: firstMessage, sentAt: new Date().toISOString(), conversationId: created.conversationId});
      await sendMessage({ senderId: user!.userId!, receipientId: bId, content: firstMessage, sentAt: new Date().toISOString(), conversationId: created.conversationId});
      setOk(`Conversation #${created.conversationId} created and first message sent.`);
      setSellerId(''); setBuyerId(''); setFirstMessage('Hello!');
    } catch (e: any) {
      setErr(e?.message ?? 'Failed to create conversation');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="border rounded-2xl p-4 mb-4">
      <div className="font-semibold mb-2">Admin: Create Conversation & Send First Message</div>
      <div className="grid grid-cols-2 gap-3">
        <label className="text-sm">Seller ID
          <input className="mt-1 w-full border rounded-lg p-2" value={sellerId} onChange={e=>setSellerId(e.target.value)} placeholder="e.g. 12" />
        </label>
        <label className="text-sm">Buyer ID
          <input className="mt-1 w-full border rounded-lg p-2" value={buyerId} onChange={e=>setBuyerId(e.target.value)} placeholder="e.g. 34" />
        </label>
        <label className="col-span-2 text-sm">First message
          <textarea className="mt-1 w-full border rounded-lg p-2" value={firstMessage} onChange={e=>setFirstMessage(e.target.value)} />
        </label>
      </div>
      <div className="mt-3 flex items-center gap-3">
        <button disabled={busy} onClick={() => void handleCreate()} className="px-4 py-2 rounded-xl bg-black text-white disabled:opacity-50">Create & Send</button>
        {ok && <div className="text-green-600 text-sm">{ok}</div>}
        {err && <div className="text-red-600 text-sm">{err}</div>}
      </div>
    </div>
  );
}