import {api} from './client';
import type {Message} from './types';
export const getMessage = (id: number) => api < Message > (`/api/Messages/id?id=${id}`); // per swagger
export const sendMessage = (payload: {
      senderId: number;receipientId: number;content: string;sentAt: string;conversationId: number;
   }) =>
   api < void > ('/api/Messages', {
      method: 'POST',
      body: JSON.stringify(payload)
   });

export const getMessagesByConversation =
 (conversationId: number) => api < Message[] > (`/api/Messages/GetMessageByConvo/${conversationId}`, {
    method: 'GET'}); 


 