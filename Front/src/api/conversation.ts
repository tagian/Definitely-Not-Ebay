import {api} from './client';
import type {Conversation} from './types';

export const getConversation = (id: number) => api < Conversation > (`/api/Conversations/${id}`);
export const createConversation = (userAId: number, userBId: number) => api < Conversation > ('/api/Conversations', {
   method: 'POST',
   body: JSON.stringify({
      userAId,
      userBId
   })
});
export const deleteConversation = (id: number) => api < void > (`/api/Conversations/${id}`, {
   method: 'DELETE'
});

export const getMyConversations = () => api < Conversation[] > (`/api/Conversations/getmine`, {
   method: 'GET'
});
