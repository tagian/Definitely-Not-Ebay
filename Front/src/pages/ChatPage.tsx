import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useAuth } from '../store/authstore';
import { getMessagesByConversation, sendMessage } from '../api/messages';
import type { Conversation, Message, User } from '../api/types';
import { Link } from 'react-router-dom';
import { getMyConversations } from '../api/conversation';

const POLL_CONVERSATIONS_MS = 10_000; // 10s
const POLL_MESSAGES_MS = 4_000;       // 4s

// Generic polling hook
function usePolling(callback: () => void | Promise<void>, intervalMs: number, enabled: boolean) {
  useEffect(() => {
    if (!enabled) return;
    let cancelled = false;
    let timer: number | undefined;

    const tick = async () => {
      await callback();
      if (!cancelled) timer = window.setTimeout(tick, intervalMs);
    };

    tick();
    return () => {
      cancelled = true;
      if (timer) window.clearTimeout(timer);
    };
  }, [callback, intervalMs, enabled]);
}

function EmptyState() {
  return (
    <div className="flex flex-col items-center justify-center h-full text-center text-gray-500">
      <div className="text-2xl">No conversation selected</div>
      <p className="mt-2">Pick one from the left to read & reply.</p>
    </div>
  );
}

function SidebarConversationItem({ c, me, active, onSelect }: { c: Conversation; me: User; active: boolean; onSelect: () => void }) {
  const otherId = c.userAId === me.userId ? c.userBId : c.userAId;
  return (
    <button onClick={onSelect} className={`w-full text-left px-3 py-2 rounded-xl hover:bg-gray-100 transition ${active ? 'bg-gray-100' : ''}`}>
      <div className="font-medium truncate">User #{otherId}</div>
    </button>
  );
}

function MessageBubble({ m, me }: { m: Message; me: User }) {
  const mine = m.senderId === me.userId;
  return (
    <div className={`flex ${mine ? 'justify-end' : 'justify-start'}`}>
      <div className={`max-w-[75%] rounded-2xl px-4 py-2 shadow-sm ${mine ? 'bg-blue-600 text-white' : 'bg-gray-100'}`}>
        <div className="whitespace-pre-wrap break-words">{m.content}</div>
      </div>
    </div>
  );
}

function Composer({ onSend, disabled }: { onSend: (text: string) => Promise<void>; disabled: boolean }) {
  const [text, setText] = useState('');
  const [busy, setBusy] = useState(false);

  async function handleSend() {
    if (!text.trim() || busy) return;
    setBusy(true);
    try {
      await onSend(text.trim());
      setText('');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="flex gap-2 p-3 border-t">
      <textarea
        className="flex-1 border rounded-xl p-3 resize-none h-[60px] focus:outline-none focus:ring"
        placeholder="Type a message…"
        value={text}
        onChange={(e) => setText(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            void handleSend();
          }
        }}
      />
      <button disabled={disabled || busy} onClick={() => void handleSend()} className="px-4 py-2 rounded-xl bg-black text-white disabled:opacity-50">
        Send
      </button>
    </div>
  );
}

export default function ChatPage() {
  const { user } = useAuth();
  const [conversations, setConversations] = useState<Conversation[] | null>(null);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [loadingMsgs, setLoadingMsgs] = useState(false);
  const scrollRef = useRef<HTMLDivElement>(null);

  const me = user as User | null;


  const loadConversations = React.useCallback(async () => {
    const list = await getMyConversations();
    setConversations(list);
    if (list.length > 0 && (selectedId == null || !list.some(c => c.conversationId === selectedId))) {
      setSelectedId(list[0].conversationId);
    }
  }, [selectedId]);

  useEffect(() => { void loadConversations(); }, [loadConversations]);
  usePolling(loadConversations, POLL_CONVERSATIONS_MS, true);


  const currentConv = useMemo(() => conversations?.find(c => c.conversationId === selectedId) ?? null, [conversations, selectedId]);

  const loadMessages = React.useCallback(async () => {
    if (!selectedId) return;
    setLoadingMsgs(true);
    try {
      const incoming = await getMessagesByConversation(selectedId);
      setMessages(incoming);
    } finally {
      setLoadingMsgs(false);
    }
  }, [selectedId]);

  useEffect(() => { setMessages([]); if (selectedId) void loadMessages(); }, [loadMessages, selectedId]);
  usePolling(loadMessages, POLL_MESSAGES_MS, Boolean(selectedId));

  useEffect(() => {
    // autoscroll
    scrollRef.current?.scrollTo({ top: scrollRef.current.scrollHeight, behavior: 'smooth' });
  }, [messages.length]);

  async function handleSend(text: string) {
    if (!me || !currentConv) return;
    const otherId = currentConv.userAId === me.userId ? currentConv.userBId : currentConv.userAId;
    await sendMessage({ senderId: me.userId, receipientId: otherId, content: text, sentAt: new Date().toISOString(), conversationId: currentConv.conversationId });
    await loadMessages();
  }

  if (!me) {
    return (
      <div className="max-w-sm mx-auto p-6 text-sm text-gray-700">Please <Link className="underline" to="/login">log in</Link> to access your inbox.</div>
    );
  }

  return (
    <div className="h-[calc(100vh-64px)] grid grid-cols-12">
      <div className="col-span-4 border-r flex flex-col">
        <div className="p-3 border-b flex items-center justify-between">
          <div className="font-semibold">Inbox</div>
          <div className="text-xs text-gray-500">Polling every {POLL_CONVERSATIONS_MS/1000}s</div>
        </div>
        <div className="flex-1 overflow-y-auto p-2">
          {!conversations && <div className="p-3 text-sm text-gray-500">Loading…</div>}
          {conversations && conversations.length === 0 && (
            <div className="p-3 text-sm text-gray-500">No conversations yet.</div>
          )}
          {conversations?.map(c => (
            <SidebarConversationItem key={c.conversationId} c={c} me={me} active={selectedId === c.conversationId} onSelect={() => setSelectedId(c.conversationId)} />
          ))}
        </div>
      </div>

      <div className="col-span-8 flex flex-col">
        <div className="p-3 border-b flex items-center justify-between">
          {currentConv ? (
            <>
              <div className="font-semibold">Conversation #{currentConv.conversationId}</div>
              <div className="text-xs text-gray-500">Polling every {POLL_MESSAGES_MS/1000}s</div>
            </>
          ) : <div className="font-semibold">Conversation</div>}
        </div>
        <div ref={scrollRef} className="flex-1 overflow-y-auto p-4 space-y-2 bg-white">
          {!currentConv ? <EmptyState /> : (
            <>
              {messages.length === 0 && (
                <div className="text-sm text-gray-500 text-center">{loadingMsgs ? 'Loading…' : 'No messages yet.'}</div>
              )}
              {messages.map(m => <MessageBubble key={m.messageId} m={m} me={me} />)}
            </>
          )}
        </div>
        <Composer onSend={handleSend} disabled={!currentConv} />
      </div>
    </div>
  );
}
