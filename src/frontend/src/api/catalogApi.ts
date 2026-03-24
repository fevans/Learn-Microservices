import axios from 'axios';

const BASE_URL = process.env.REACT_APP_CATALOG_SERVICE_URL;

export interface CatalogItem {
    id: string;
    name: string;
    description?: string;
    price: number;
    createdDate: string;
}

export const getCatalogItems = () =>
    axios.get<CatalogItem[]>(`${BASE_URL}/items`).then(r => r.data);

export const createCatalogItem = (name: string, description: string, price: number) =>
    axios.post<CatalogItem>(`${BASE_URL}/items`, { name, description, price }).then(r => r.data);
// export const getCatalogItems = async (): Promise<CatalogItem[]> => {
//     const response = await axios.get<CatalogItem[]>(`${BASE_URL}/items`);
//     return response.data;
// };