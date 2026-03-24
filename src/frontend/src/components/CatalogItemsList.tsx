import { useEffect, useState } from 'react';
import { CatalogItem, getCatalogItems } from '../api/catalogApi';

export const CatalogItemList = () => {
    const [items, setItems] = useState<CatalogItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        getCatalogItems()
            .then(setItems)
            .catch(() => setError('Could not load catalog'))
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <p>Loading...</p>;
    if (error)   return <p style={{ color: 'red' }}>{error}</p>;

    return (
        <div>
            <h2>Catalog</h2>
            <ul>
                {items.map(item => (
                    <li key={item.id}>
                        <strong>{item.name}</strong> — ${item.price.toFixed(2)}
                        <br />
                        <small>{item.description}</small>
                    </li>
                ))}
            </ul>
        </div>
    );
};