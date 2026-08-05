import { useEffect, useState } from 'react';
import { listCategories, createCategory } from '../api/categories';
import type { Category } from '../api/types';

export default function ManageCategories() {
    const [cats, setCats] = useState < Category[] > ([]);
    const [name, setName] = useState('');
    useEffect(() => {
        listCategories().then(setCats);
    }, []);
    const add = async () => {
        const c = await createCategory({
            name
        });
        setCats(a => [...a, c]);
        setName('');
    };
    return (
        <div className="p-6 space-y-3">
            <h1 className="text-2xl font-bold">Categories</h1>
            <div className="flex gap-2">
                <input className="border p-2 rounded" value={name} onChange={e=>setName(e.target.value)} placeholder="New category" />
                <button onClick={add} className="border px-3 py-2 rounded">Add</button>
            </div>
        <ul className="list-disc pl-6">
            {cats.map(c => <li key={c.categoryId}>{c.name}</li>)}
        </ul>
    </div>
    );
}
