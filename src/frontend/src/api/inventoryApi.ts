import axios from 'axios';

const BASE = process.env.REACT_APP_INVENTORY_SERVICE_URL;

export interface InventoryItem {
    catalogItemId: string;
    name: string;
    description?: string;
    quantity: number;
    acquiredDate: string;
}

export const getInventoryItems = (userId: string) =>
    axios.get<InventoryItem[]>(`${BASE}/items?userId=${userId}`).then(r => r.data);

export const grantItems = (userId: string, catalogItemId: string, quantity: number) =>
    axios.post(`${BASE}/items`, { userId, catalogItemId, quantity });