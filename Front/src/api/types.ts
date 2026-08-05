export type Role = 'Admin' | 'Seller' | 'Buyer' | 'Visitor';


export type AuthResponse = {
   token ? : string;refreshToken ? : string;expiresAt ? : string;
   message ? : string;isSuccess: boolean;errorMessage ? : string;
};


export type User = {
   userId: number;name ? : string;email ? : string;phone ? : string;
   address ? : string;city ? : string;region ? : string;postalCode ? : string;
   country ? : string;role ? : Role;createdAt ? : string;updatedAt ? : string;
};


export type Item = {
   itemId: number;name ? : string;description ? : string;price: number;
   categoryId ? : number;thumbnailPath ? : string;isActive: boolean;
   createdAt: string;updatedAt ? : string;sellerId: number; address ? : string;
   latitude ? : number;longitude ? : number;
};


export type Category = {
   categoryId: number;name ? : string;description ? : string;thumbnailPath ? : string;
   createdAt ? : string;updatedAt ? : string;
};


export type AuctionStatus = 0 | 1 | 2 | 3; //Pending = 0,Active = 1,Completed = 2,Cancelled = 3
export type Auction = {
   auctionId: number;itemId: number;winnerId ? : number | null;
   status: AuctionStatus;startingAt: string;endingAt: string;
   createdAt: string;updatedAt ? : string | null; startingPrice: number;
};


export type Bid = {
   bidId ? : number;bidderId ? : number;hit ? : number;auctionId ? : number;createdAt ? : string;
};


export type Conversation = {
   conversationId: number;userAId: number;userBId: number;createdAt: string;updatedAt: string;
};


export type Message = {
   messageId: number;senderId: number;receipientId: number;content ? : string;
   sentAt: string;isRead: boolean;readAt: string;conversationId: number;
};

export type Order = {
   orderId: number;
   buyerId: number;
   sellerId: number;
   itemId: number;
   createdAt: string;
   updatedAt ? : string;
   BuyNow: boolean;
};