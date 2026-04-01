// frontend/src/components/InventoryList.tsx
import { useEffect, useState } from 'react';
import { InventoryItem, getInventoryItems } from '../api/inventoryApi';
import { useAuthenticatedClient } from '../hooks/useAuthenticatedClient';

interface Props { userId: string; }

export const InventoryList = ({ userId }: Props) => {
    const client = useAuthenticatedClient();
    const [items, setItems] = useState<InventoryItem[]>([]);

    useEffect(() => {
        if (userId) getInventoryItems(client, userId).then(setItems);
    }, [client, userId]);

    return (
        <div>
            <h2>Inventory</h2>
            {items.length === 0
                ? <p>No items in inventory.</p>
                : (
                    <ul>
                        {items.map(item => (
                            <li key={item.catalogItemId}>
                                {item.name} × {item.quantity}
                            </li>
                        ))}
                    </ul>
                )
            }
        </div>
    );
};