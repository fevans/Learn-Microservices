import { AxiosInstance } from 'axios';

const BASE = process.env.REACT_APP_INVENTORY_SERVICE_URL;

export interface InventoryItem {
    catalogItemId: string;
    name: string;
    description?: string;
    quantity: number;
    acquiredDate: string;
}

export const getInventoryItems = (client: AxiosInstance, userId: string) =>
    client.get<InventoryItem[]>(`${BASE}/items?userId=${userId}`).then(r => r.data);

export const grantItems = (client: AxiosInstance, userId: string, catalogItemId: string, quantity: number) =>
    client.post(`${BASE}/items`, { userId, catalogItemId, quantity });