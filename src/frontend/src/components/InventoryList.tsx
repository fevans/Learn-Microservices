// frontend/src/components/InventoryList.tsx
import { useEffect, useState } from 'react';
import { InventoryItem, getInventoryItems } from '../api/inventoryApi';

interface Props { userId: string; }

export const InventoryList = ({ userId }: Props) => {
    const [items, setItems] = useState<InventoryItem[]>([]);

    useEffect(() => {
        if (userId) getInventoryItems(userId).then(setItems);
    }, [userId]);

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