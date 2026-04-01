import { AxiosInstance } from 'axios';

const BASE_URL = process.env.REACT_APP_CATALOG_SERVICE_URL;

export interface CatalogItem {
    id:          string;
    name:        string;
    description?: string;
    price:       number;
    createdDate: string;
}

export interface CreateCatalogItemDto {
    name:         string;
    description?: string;
    price:        number;
}

export const getCatalogItems = (client: AxiosInstance) =>
    client.get<CatalogItem[]>(`${BASE_URL}/items`).then(r => r.data);

export const createCatalogItem = (client: AxiosInstance, dto: CreateCatalogItemDto) =>
    client.post<CatalogItem>(`${BASE_URL}/items`, dto).then(r => r.data);

export const updateCatalogItem = (
    client: AxiosInstance,
    id: string,
    dto: Partial<CreateCatalogItemDto>) =>
    client.put(`${BASE_URL}/items/${id}`, dto);

export const deleteCatalogItem = (client: AxiosInstance, id: string) =>
    client.delete(`${BASE_URL}/items/${id}`);