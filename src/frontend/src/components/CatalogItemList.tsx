import { useEffect, useState } from 'react';
import { useAuthenticatedClient } from '../hooks/useAuthenticatedClient';
import { useRoles } from '../hooks/useRoles';
import { CatalogItem, getCatalogItems, createCatalogItem } from '../api/catalogApi';

export const CatalogItemList = () => {
    const client          = useAuthenticatedClient();
    const { isAdmin }     = useRoles();
    const [items, setItems]   = useState<CatalogItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError]   = useState<string | null>(null);

    useEffect(() => {
        getCatalogItems(client)
            .then(setItems)
            .catch(() => setError('Failed to load catalog'))
            .finally(() => setLoading(false));
    }, [client]);

    if (loading) return <p>Loading...</p>;
    if (error)   return <p style={{ color: 'red' }}>{error}</p>;

    return (
        <div>
            <h2>Catalog</h2>
            <ul>
                {items.map(item => (
                    <li key={item.id}>
                        <strong>{item.name}</strong> — ${item.price.toFixed(2)}
                        {isAdmin && (
                            <button onClick={() => handleDelete(item.id)}>Delete</button>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );

    async function handleDelete(id: string) {
        await client.delete(`${process.env.REACT_APP_CATALOG_SERVICE_URL}/items/${id}`);
        setItems(prev => prev.filter(i => i.id !== id));
    }
};